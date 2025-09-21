using HarmonyLib;
using UnityEngine;

namespace MiraAPI.Patches.LocalSettings;

[HarmonyPatch(typeof(ToggleButtonBehaviour))]
public static class ToggleBehaviourPatch
{
    /// <summary>
    /// ResetText was inlined ofc.
    /// Skill issue.
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(ToggleButtonBehaviour.UpdateText))]
    public static bool UpdateTextPrefix(ToggleButtonBehaviour __instance, bool on)
    {
        var color = on ? new Color(0f, 1f, 0.16470589f, 1f) : Color.white;
        __instance.onState = on;
        __instance.Background.color = color;

        // Used to mask the text
        __instance.Text.richText = true;
        __instance.Text.text = $"<font=\"LiberationSans SDF\" material=\"LiberationSans SDF - Chat Message Masked\">" +
                               $"{DestroyableSingleton<TranslationController>.Instance.GetString(__instance.BaseText)}: <b>" +
                               $"{DestroyableSingleton<TranslationController>.Instance.GetString(__instance.onState ? StringNames.SettingsOn : StringNames.SettingsOff)}</b></font>";
        __instance.Text.sortingOrder = 151;
        __instance.Text.renderer.sortingLayerName = "Default";
        __instance.Text.renderer.sortingOrder = 151;

        if (__instance.Rollover)
        {
            __instance.Rollover.ChangeOutColor(color);
        }

        return false;
    }
}
