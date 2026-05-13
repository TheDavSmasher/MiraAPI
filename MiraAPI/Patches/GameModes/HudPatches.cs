using HarmonyLib;
using MiraAPI.GameModes;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;

namespace MiraAPI.Patches.GameModes;

[HarmonyPatch(typeof(HudManager))]
internal static class HudPatches
{
    [HarmonyPostfix, HarmonyPatch(nameof(HudManager.Start))]
    public static void HudStartPatch(HudManager __instance) => CustomGameModeManager.ActiveMode?.HudStart(__instance);

    [HarmonyPostfix, HarmonyPatch(nameof(HudManager.Update))]
    public static void HudUpdatePatch(HudManager __instance) => CustomGameModeManager.ActiveMode?.HudUpdate(__instance);

    [HarmonyPatch(typeof(HudManager), nameof(HudManager.Start))]
    [HarmonyPostfix]
    public static void PostHudStart(HudManager __instance)
    {
        if (GameManager.Instance.IsHideAndSeek())
            return;
        var infoPane = __instance.gameObject.transform.FindChild("LobbyInfoPane");
        var aspect = infoPane.gameObject.transform.Find("AspectSize");
        var modeLabel = aspect!.Find("ModeLabel");
        var modeValue = aspect.Find("ModeValue");
        var modelText = modeLabel!.Find("Text_TMP").gameObject;
        var modelTextClone = Object.Instantiate(modelText, modeLabel);
        Object.Destroy(modelTextClone.GetComponent<TextTranslatorTMP>());
        modelTextClone.GetComponent<TextMeshPro>().text = "Gamemode";
        var gmText = modeValue!.Find("GameModeText").gameObject;
        var gmTextClone = Object.Instantiate(gmText, modeValue);
        gmText.SetActive(false);
        modelText.SetActive(false);
        _text = gmTextClone.GetComponent<TextMeshPro>();
        _text.text = CustomGameModeManager.ActiveMode != null ? $"<color=#{CustomGameModeManager.ActiveMode.Color.ToHtmlStringRGBA()}>{CustomGameModeManager.ActiveMode.Name}</color>" : "Classic";
    }

    private static TextMeshPro? _text;
    internal static void SetGameModeText(string text)
    {
        if (_text != null)
            _text.text = text;
    }
}
