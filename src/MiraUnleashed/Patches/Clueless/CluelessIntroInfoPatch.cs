using System.Reflection;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using TMPro;
using MiraUnleashed.Modifiers;
using TownOfUs.Events;
using TownOfUs.Modifiers.Game;
using TownOfUs.Options;
using TownOfUs.Utilities;

namespace MiraUnleashed.Patches.Clueless;

/// <summary>
/// Patches to ensure Clueless modifier shows the intro blurb instead of just "Modifier: Clueless".
/// </summary>
[HarmonyPatch]
public static class CluelessIntroInfoPatch
{
    private static readonly FieldInfo? ModifierTextField = typeof(TownOfUsEventHandlers).GetField("ModifierText", BindingFlags.NonPublic | BindingFlags.Static);

    [HarmonyPatch(typeof(TownOfUsEventHandlers), nameof(TownOfUsEventHandlers.RunModChecks))]
    [HarmonyPostfix]
    public static void RunModChecksPostfix()
    {
        var option = OptionGroupSingleton<InitialRoundOptions>.Instance.ModifierReveal;
        var uniModifier = PlayerControl.LocalPlayer.GetModifiers<UniversalGameModifier>().FirstOrDefault();

        if (uniModifier is CluelessModifier && option is ModReveal.Universal)
        {
            var modifierText = ModifierTextField?.GetValue(null) as TextMeshPro;
            if (modifierText != null)
            {
                var introBlurb = MiraLocaleManager.Get("MiraUnleashed.Modifier.Clueless.IntroBlurb");
                modifierText.text = $"<size={uniModifier.IntroSize}>{introBlurb}</size>";
                modifierText.color = MiscUtils.GetModifierColour(uniModifier);
            }
        }
    }
}
