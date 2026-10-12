using System.Reflection;
using System.Reflection.Emit;
using AchievementsAPI.API;

namespace MiraUnleashed.Achievements;

/// <summary>
/// Creates concrete <see cref="AchievementsTab"/> subclasses at runtime.
/// <para>
/// A compile-time subclass would put an AchievementsAPI base type into this assembly's
/// type hierarchy. Mira API (and the Achievements API itself) run
/// <see cref="Assembly.GetTypes"/> over our assembly, which would throw a
/// TypeLoadException when the optional Achievements API DLL is not installed.
/// Emitting the subclass at runtime keeps the dependency fully soft: the type only
/// exists when the API was detected beforehand.
/// </para>
/// </summary>
internal static class DynamicTabFactory
{
    private const MethodAttributes OverrideAttributes = MethodAttributes.Public | MethodAttributes.Virtual
        | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot;

    /// <summary>
    /// Creates a registered tab instance owning the given achievements as properties
    /// (the Achievements API discovers achievements by reflecting over tab properties).
    /// </summary>
    public static AchievementsTab CreateTab(int tabIndex, IReadOnlyList<(string PropName, BaseAchievement Achievement)> achievements)
    {
        var assemblyName = new AssemblyName($"MiraUnleashed.DynamicTab{tabIndex}");
        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name!);
        var typeBuilder = moduleBuilder.DefineType("MuAchievementsTab",
            TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, typeof(AchievementsTab));

        typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

        OverrideTabMembers(typeBuilder, tabIndex);

        var backingFields = new Dictionary<string, FieldBuilder>(achievements.Count, StringComparer.Ordinal);
        foreach (var (propName, achievement) in achievements)
        {
            DefineAchievementProperty(typeBuilder, propName, achievement.GetType(), backingFields);
        }

        var tabType = typeBuilder.CreateType()!;
        var tab = (AchievementsTab)Activator.CreateInstance(tabType)!;

        foreach (var (propName, achievement) in achievements)
        {
            tabType.GetField(backingFields[propName].Name, BindingFlags.NonPublic | BindingFlags.Instance)!
                .SetValue(tab, achievement);
        }

        return tab;
    }

    // Overrides Name, GetTabColor and GetIcon to call back into AchievementsRuntime.
    private static void OverrideTabMembers(TypeBuilder typeBuilder, int tabIndex)
    {
        var runtimeType = typeof(AchievementsRuntime);
        var tabType = typeof(AchievementsTab);

        OverrideMethod(typeBuilder,
            tabType.GetMethod("get_Name")!,
            runtimeType.GetMethod(nameof(AchievementsRuntime.GetTabName))!,
            tabIndex);

        OverrideMethod(typeBuilder,
            tabType.GetMethod(nameof(AchievementsTab.GetTabColor))!,
            runtimeType.GetMethod(nameof(AchievementsRuntime.GetTabColor))!,
            tabIndex);

        OverrideMethod(typeBuilder,
            tabType.GetMethod(nameof(AchievementsTab.GetIcon))!,
            runtimeType.GetMethod(nameof(AchievementsRuntime.GetTabIcon))!,
            tabIndex);
    }

    private static void OverrideMethod(TypeBuilder typeBuilder, MethodInfo baseMethod, MethodInfo callback, int tabIndex)
    {
        var methodBuilder = typeBuilder.DefineMethod(baseMethod.Name, OverrideAttributes,
            baseMethod.ReturnType, Type.EmptyTypes);

        var il = methodBuilder.GetILGenerator();
        il.Emit(OpCodes.Ldc_I4, tabIndex);
        il.Emit(OpCodes.Call, callback);
        il.Emit(OpCodes.Ret);

        typeBuilder.DefineMethodOverride(methodBuilder, baseMethod);
    }

    private static void DefineAchievementProperty(TypeBuilder typeBuilder, string name, Type achievementType,
        Dictionary<string, FieldBuilder> backingFields)
    {
        var fieldName = $"<{name}>k__BackingField";
        var field = typeBuilder.DefineField(fieldName, achievementType, FieldAttributes.Private);
        backingFields[name] = field;

        var propertyBuilder = typeBuilder.DefineProperty(name, PropertyAttributes.None, achievementType, Type.EmptyTypes);

        var getter = typeBuilder.DefineMethod($"get_{name}",
            MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig,
            achievementType, Type.EmptyTypes);

        var il = getter.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.Emit(OpCodes.Ldfld, field);
        il.Emit(OpCodes.Ret);

        propertyBuilder.SetGetMethod(getter);
    }
}
