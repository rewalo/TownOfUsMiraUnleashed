using System.Collections;
using System.Reflection;
using Reactor.Utilities.Attributes;
using TownOfUs.Achievements;
using UnityEngine;

namespace MiraUnleashed.DebugTools;

/// <summary>
/// In-game IMGUI achievements debug tool (F8), installed only in dev builds.
/// Local-only: it never sends network traffic and never changes game state.
/// </summary>
[RegisterInIl2Cpp]
public sealed class DevDebugMenu(nint cppPtr) : MonoBehaviour(cppPtr)
{
    private static DevDebugMenu? _instance;

    private readonly HashSet<string> _loggedErrors = new();
    private Rect _windowRect = new(0, 0, 820, 820);
    private Vector2 _scroll;
    private string? _error;

    private bool _visible;
    private bool _loggedOpen;
    private bool _positioned;
    private bool _dragging;
    private bool _resizing;
    private Vector2 _dragOffset;
    private const float MinWidth = 360;
    private const float MinHeight = 240;
    private const float ResizeGrip = 22;

    private GUIStyle? _titleStyle;
    private GUIStyle? _descStyle;
    private GUIStyle? _errorStyle;
    private GUIStyle? _gripStyle;

    private List<ReflectedMod>? _reflectedTabs;

    /// <summary>
    /// Installs the debug menu. No-op in release builds and if already installed.
    /// </summary>
    public static void Install()
    {
        if (!MiraUnleashedPlugin.IsDevBuild || (_instance != null && _instance))
        {
            return;
        }

        var go = new GameObject("MiraUnleashedDevMenu");
        DontDestroyOnLoad(go);
        _instance = go.AddComponent<DevDebugMenu>();
        Info("Dev debug menu installed (F8).");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
        {
            _visible = !_visible;
            if (!_visible)
            {
                _reflectedTabs = null;
            }

            if (_visible && !_loggedOpen)
            {
                _loggedOpen = true;
                Info("Dev menu opened");
            }
        }

        if (!_visible)
        {
            _dragging = false;
            _resizing = false;
            return;
        }

        var m = Input.mousePosition;
        var gx = m.x;
        var gy = Screen.height - m.y;
        var titleBar = new Rect(_windowRect.x, _windowRect.y, _windowRect.width, 28);
        var grip = new Rect(_windowRect.xMax - ResizeGrip, _windowRect.yMax - ResizeGrip, ResizeGrip, ResizeGrip);

        if (Input.GetMouseButtonDown(0))
        {
            if (grip.Contains(new Vector2(gx, gy)))
            {
                _resizing = true;
            }
            else if (titleBar.Contains(new Vector2(gx, gy)))
            {
                _dragging = true;
                _dragOffset = new Vector2(gx - _windowRect.x, gy - _windowRect.y);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _dragging = false;
            _resizing = false;
        }

        if (_dragging && Input.GetMouseButton(0))
        {
            _windowRect.x = Mathf.Clamp(gx - _dragOffset.x, 40 - _windowRect.width, Screen.width - 40);
            _windowRect.y = Mathf.Clamp(gy - _dragOffset.y, 0, Screen.height - 40);
        }

        if (_resizing && Input.GetMouseButton(0))
        {
            _windowRect.width = Mathf.Clamp(gx - _windowRect.x, MinWidth, Screen.width - _windowRect.x);
            _windowRect.height = Mathf.Clamp(gy - _windowRect.y, MinHeight, Screen.height - _windowRect.y);
        }
    }

    private void CenterWindow()
    {
        _windowRect.width = Mathf.Min(_windowRect.width, Screen.width - 40);
        _windowRect.height = Mathf.Min(_windowRect.height, Screen.height - 40);
        _windowRect.x = (Screen.width - _windowRect.width) / 2;
        _windowRect.y = (Screen.height - _windowRect.height) / 2;
        _positioned = true;
    }

    private void OnGUI()
    {
        if (!_visible)
        {
            return;
        }

        _titleStyle ??= new GUIStyle(GUI.skin.label) { fontSize = 15, fontStyle = FontStyle.Bold };
        _descStyle ??= new GUIStyle(GUI.skin.label) { fontSize = 10, wordWrap = true };
        _errorStyle ??= new GUIStyle(GUI.skin.label)
        {
            fontSize = 10,
            wordWrap = true,
            normal = { textColor = Color.red },
        };
        _gripStyle ??= new GUIStyle(GUI.skin.label) { alignment = TextAnchor.LowerRight, fontSize = 14 };

        if (!_positioned)
        {
            CenterWindow();
        }

        GUILayout.BeginArea(_windowRect, GUI.skin.box);
        try
        {
            GUILayout.BeginHorizontal(GUILayout.Height(28));
            GUILayout.Label("Mira Unleashed Dev Menu (F8) — drag here, resize at corner", _titleStyle);
            if (GUILayout.Button("Reset position", GUILayout.Width(100)))
            {
                CenterWindow();
            }

            if (GUILayout.Button("Close", GUILayout.Width(60)))
            {
                _visible = false;
                _reflectedTabs = null;
            }

            GUILayout.EndHorizontal();

            var scrollOpen = false;
            try
            {
                _scroll = GUILayout.BeginScrollView(_scroll);
                scrollOpen = true;
                DrawAchievementsTab();
                _error = null;
            }
            finally
            {
                if (scrollOpen)
                {
                    GUILayout.EndScrollView();
                }
            }

            if (_error != null)
            {
                GUILayout.Label(_error, _errorStyle);
            }
        }
        catch (Exception ex)
        {
            _error = ex.Message;
            if (_loggedErrors.Add(ex.Message))
            {
                Error(ex.ToString());
            }
        }
        finally
        {
            GUILayout.EndArea();
        }

        if (_visible)
        {
            GUI.Label(new Rect(_windowRect.xMax - ResizeGrip, _windowRect.yMax - ResizeGrip, ResizeGrip, ResizeGrip), "◢",
                _gripStyle);
        }
    }

    // --- Achievements ---

    /// <summary>
    /// Snapshot of an AchievementsAPI category used by the dev menu list.
    /// </summary>
    private sealed record ReflectedCategory(string Id, string Name, List<ReflectedAchievement> Achievements);

    /// <summary>
    /// Snapshot of an AchievementsAPI mod used by the dev menu list.
    /// </summary>
    private sealed record ReflectedMod(string Name, List<ReflectedCategory> Categories);

    /// <summary>
    /// Snapshot of an AchievementsAPI achievement used by the dev menu list.
    /// </summary>
    private sealed record ReflectedAchievement(
        object Instance,
        string Id,
        string Name,
        string Description,
        string CategoryId,
        bool IsUnlocked,
        bool IsHidden,
        bool IsCount,
        int CurrentValue,
        int RequiredValue);

    private void DrawAchievementsTab()
    {
        if (!AchievementsAPIHelper.IsLoaded)
        {
            GUILayout.Label("AchievementsAPI is not loaded.");
            return;
        }

        if (!AllModsAchievements.IsAvailable)
        {
            GUILayout.Label("Unable to reflect AchievementsAPI state.");
            return;
        }

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Unlock all mods"))
        {
            AllModsAchievements.UnlockAll();
            _reflectedTabs = null;
        }

        if (GUILayout.Button("Reset all mods"))
        {
            AllModsAchievements.ResetAll();
            _reflectedTabs = null;
        }

        if (GUILayout.Button("Refresh list", GUILayout.Width(90)))
        {
            _reflectedTabs = null;
        }

        GUILayout.EndHorizontal();
        GUILayout.Label("Lists achievements from every loaded mod, grouped by mod then category.", _descStyle);

        _reflectedTabs ??= AllModsAchievements.GetMods().ToList();

        foreach (var mod in _reflectedTabs)
        {
            GUILayout.Label(mod.Name, _titleStyle);
            foreach (var category in mod.Categories)
            {
                GUILayout.Label($"  {category.Name}", _descStyle);
                foreach (var achievement in category.Achievements)
                {
                    DrawReflectedAchievementRow(achievement);
                }
            }
        }
    }

    private void DrawReflectedAchievementRow(ReflectedAchievement achievement)
    {
        var name = achievement.Name + (achievement.IsHidden ? " (hidden)" : string.Empty);
        if (achievement.IsCount)
        {
            name += $"  [{achievement.CurrentValue}/{achievement.RequiredValue}]";
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label(achievement.IsUnlocked ? "[x]" : "[ ]", GUILayout.Width(30));
        GUILayout.Label(name);
        GUILayout.EndHorizontal();
        GUILayout.Label(achievement.Description, _descStyle);

        GUILayout.BeginHorizontal();
        GUI.enabled = !achievement.IsUnlocked;
        if (GUILayout.Button("Unlock", GUILayout.Width(70)))
        {
            AllModsAchievements.Unlock(achievement);
            _reflectedTabs = null;
        }

        GUI.enabled = true;
        if (GUILayout.Button("Reset", GUILayout.Width(60)))
        {
            AllModsAchievements.Reset(achievement);
            _reflectedTabs = null;
        }

        if (achievement.IsCount)
        {
            if (GUILayout.Button("+1", GUILayout.Width(40)))
            {
                AllModsAchievements.Increment(achievement, 1);
                _reflectedTabs = null;
            }

            GUI.enabled = achievement.CurrentValue > 0;
            if (GUILayout.Button("-1", GUILayout.Width(40)))
            {
                AllModsAchievements.SetValue(achievement, Mathf.Max(0, achievement.CurrentValue - 1));
                _reflectedTabs = null;
            }

            GUI.enabled = true;
        }

        GUILayout.EndHorizontal();
        GUILayout.Space(6);
    }

    /// <summary>
    /// Reflection bridge to AchievementsAPI. Lets the dev menu list, lock and unlock every
    /// registered achievement from every loaded mod without MiraUnleashed taking a compile-time
    /// dependency on AchievementsAPI types outside of dev-only code.
    /// </summary>
    private static class AllModsAchievements
    {
        private const string AchievementsApiAssemblyName = "AchievementsAPI";

        private static readonly FieldInfo? TabsField;
        private static readonly MethodInfo? GetTextMethod;
        private static readonly PropertyInfo? TabNameProperty;
        private static readonly PropertyInfo? TabModNameProperty;
        private static readonly PropertyInfo? TabCategoryIdProperty;
        private static readonly MethodInfo? TabGetAchievementsMethod;
        private static readonly PropertyInfo? AchievementNameProperty;
        private static readonly PropertyInfo? AchievementDescriptionProperty;
        private static readonly FieldInfo? AchievementUnlockedField;
        private static readonly FieldInfo? AchievementHiddenField;
        private static readonly FieldInfo? AchievementIdField;
        private static readonly FieldInfo? AchievementCategoryField;
        private static readonly Type? CountAchievementType;
        private static readonly FieldInfo? CountCurrentValueField;
        private static readonly FieldInfo? CountRequiredValueField;
        private static readonly MethodInfo? UnlockMethod;
        private static readonly MethodInfo? ResetMethod;
        private static readonly MethodInfo? SaveMethod;
        private static readonly MethodInfo? CountIncrementMethod;
        private static readonly MethodInfo? CountSetValueMethod;

        static AllModsAchievements()
        {
            var assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(a => a.GetName().Name == AchievementsApiAssemblyName);
            if (assembly == null)
            {
                return;
            }

            var managerType = assembly.GetType("AchievementsAPI.API.AchievementsManager");
            var storageType = assembly.GetType("AchievementsAPI.API.AchievementStorage");
            var baseAchievementType = assembly.GetType("AchievementsAPI.API.BaseAchievement");
            var achievementsStringsType = assembly.GetType("AchievementsAPI.API.AchievementsStrings");
            var achievementsTabType = assembly.GetType("AchievementsAPI.API.AchievementsTab");
            CountAchievementType = assembly.GetType("AchievementsAPI.API.CountAchievement");

            TabsField = managerType?.GetField("Tabs", BindingFlags.Public | BindingFlags.Static);
            SaveMethod = storageType?.GetMethod("Save", BindingFlags.Public | BindingFlags.Static);
            GetTextMethod = achievementsStringsType?.GetMethod("GetText", BindingFlags.Public | BindingFlags.Static);
            TabNameProperty = achievementsTabType?.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
            TabModNameProperty = achievementsTabType?.GetProperty("ModName", BindingFlags.Public | BindingFlags.Instance);
            TabCategoryIdProperty = achievementsTabType?.GetProperty("CategoryId", BindingFlags.Public | BindingFlags.Instance);
            TabGetAchievementsMethod = achievementsTabType?.GetMethod("GetAchievements", BindingFlags.Public | BindingFlags.Instance);
            AchievementNameProperty = baseAchievementType?.GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
            AchievementDescriptionProperty = baseAchievementType?.GetProperty("Description", BindingFlags.Public | BindingFlags.Instance);
            AchievementUnlockedField = baseAchievementType?.GetField("Unlocked", BindingFlags.Public | BindingFlags.Instance);
            AchievementHiddenField = baseAchievementType?.GetField("Hidden", BindingFlags.Public | BindingFlags.Instance);
            AchievementIdField = baseAchievementType?.GetField("Id", BindingFlags.Public | BindingFlags.Instance);
            AchievementCategoryField = baseAchievementType?.GetField("Category", BindingFlags.Public | BindingFlags.Instance);
            CountCurrentValueField = CountAchievementType?.GetField("CurrentValue", BindingFlags.Public | BindingFlags.Instance);
            CountRequiredValueField = CountAchievementType?.GetField("RequiredValue", BindingFlags.Public | BindingFlags.Instance);

            UnlockMethod = baseAchievementType?.GetMethod(
                "Unlock",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(bool), typeof(bool) },
                null);
            ResetMethod = baseAchievementType?.GetMethod(
                "Reset",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(bool) },
                null);
            CountIncrementMethod = CountAchievementType?.GetMethod(
                "Increment",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(int), typeof(bool) },
                null);
            CountSetValueMethod = CountAchievementType?.GetMethod(
                "SetValue",
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(int), typeof(bool) },
                null);
        }

        public static bool IsAvailable =>
            UnlockMethod != null &&
            ResetMethod != null &&
            TabsField != null &&
            TabGetAchievementsMethod != null &&
            AchievementNameProperty != null &&
            AchievementUnlockedField != null;

        public static IEnumerable<ReflectedMod> GetMods()
        {
            if (TabsField == null) yield break;

            var tabs = TabsField.GetValue(null) as IEnumerable;
            if (tabs == null) yield break;

            var mods = new Dictionary<string, ReflectedMod>();
            foreach (var tab in tabs)
            {
                if (tab == null) continue;

                var modName = GetString(TabModNameProperty?.GetValue(tab)) ?? "Unknown mod";
                var tabCategoryId = TabCategoryIdProperty?.GetValue(tab) as string ?? "general";

                if (!mods.TryGetValue(modName, out var mod))
                {
                    mod = new ReflectedMod(modName, new List<ReflectedCategory>());
                    mods[modName] = mod;
                }

                var achList = TabGetAchievementsMethod?.Invoke(tab, null) as IEnumerable;
                if (achList == null) continue;

                foreach (var ach in achList)
                {
                    if (ach == null) continue;

                    var achievement = ReflectAchievement(ach, tabCategoryId);
                    var category = mod.Categories.FirstOrDefault(c => c.Id == achievement.CategoryId);
                    if (category == null)
                    {
                        category = new ReflectedCategory(achievement.CategoryId, FormatCategoryName(achievement.CategoryId), new List<ReflectedAchievement>());
                        mod.Categories.Add(category);
                    }

                    category.Achievements.Add(achievement);
                }
            }

            foreach (var mod in mods.Values.OrderBy(m => m.Name))
            {
                mod.Categories.Sort((a, b) => string.Compare(a.Id, b.Id, StringComparison.OrdinalIgnoreCase));
                yield return mod;
            }
        }

        private static string FormatCategoryName(string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
            {
                return "General";
            }

            return char.ToUpperInvariant(categoryId[0]) + categoryId[1..];
        }

        public static void UnlockAll()
        {
            if (UnlockMethod == null) return;

            foreach (var mod in GetMods())
            {
                foreach (var category in mod.Categories)
                {
                    foreach (var achievement in category.Achievements)
                    {
                        try
                        {
                            UnlockMethod.Invoke(achievement.Instance, new object[] { false, true });
                        }
                        catch (Exception ex)
                        {
                            Error($"Failed to unlock achievement {achievement.Id}: {ex.Message}");
                        }
                    }
                }
            }
        }

        public static void ResetAll()
        {
            if (ResetMethod == null) return;

            foreach (var mod in GetMods())
            {
                foreach (var category in mod.Categories)
                {
                    foreach (var achievement in category.Achievements)
                    {
                        try
                        {
                            ResetMethod.Invoke(achievement.Instance, new object[] { false });
                        }
                        catch (Exception ex)
                        {
                            Error($"Failed to reset achievement {achievement.Id}: {ex.Message}");
                        }
                    }
                }
            }

            try
            {
                SaveMethod?.Invoke(null, null);
            }
            catch (Exception ex)
            {
                Error($"Failed to save achievements: {ex.Message}");
            }
        }

        public static void Unlock(ReflectedAchievement achievement)
        {
            try
            {
                UnlockMethod?.Invoke(achievement.Instance, new object[] { true, true });
            }
            catch (Exception ex)
            {
                Error($"Failed to unlock achievement {achievement.Id}: {ex.Message}");
            }
        }

        public static void Reset(ReflectedAchievement achievement)
        {
            try
            {
                ResetMethod?.Invoke(achievement.Instance, new object[] { true });
            }
            catch (Exception ex)
            {
                Error($"Failed to reset achievement {achievement.Id}: {ex.Message}");
            }
        }

        public static void Increment(ReflectedAchievement achievement, int amount)
        {
            try
            {
                CountIncrementMethod?.Invoke(achievement.Instance, new object[] { amount, true });
            }
            catch (Exception ex)
            {
                Error($"Failed to increment achievement {achievement.Id}: {ex.Message}");
            }
        }

        public static void SetValue(ReflectedAchievement achievement, int value)
        {
            try
            {
                CountSetValueMethod?.Invoke(achievement.Instance, new object[] { value, true });
            }
            catch (Exception ex)
            {
                Error($"Failed to set achievement {achievement.Id} value: {ex.Message}");
            }
        }

        private static ReflectedAchievement ReflectAchievement(object instance, string tabCategoryId)
        {
            var type = instance.GetType();
            var isCount = CountAchievementType != null &&
                          (type == CountAchievementType || type.IsSubclassOf(CountAchievementType));
            var name = GetString(AchievementNameProperty?.GetValue(instance)) ?? "???";
            var description = GetString(AchievementDescriptionProperty?.GetValue(instance)) ?? string.Empty;
            var id = AchievementIdField?.GetValue(instance) as string ?? string.Empty;
            var category = AchievementCategoryField?.GetValue(instance) as string ?? tabCategoryId;
            var unlocked = AchievementUnlockedField?.GetValue(instance) is true;
            var hidden = AchievementHiddenField?.GetValue(instance) is true;

            var current = 0;
            var required = 0;
            if (isCount)
            {
                current = CountCurrentValueField?.GetValue(instance) as int? ?? 0;
                required = CountRequiredValueField?.GetValue(instance) as int? ?? 0;
            }

            return new ReflectedAchievement(instance, id, name, description, category, unlocked, hidden, isCount, current, required);
        }

        private static string? GetString(object? stringNamesValue)
        {
            if (stringNamesValue == null) return null;
            if (stringNamesValue is string str) return str;
            return GetTextMethod?.Invoke(null, new[] { stringNamesValue }) as string;
        }
    }
}
