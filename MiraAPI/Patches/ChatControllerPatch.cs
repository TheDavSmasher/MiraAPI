using HarmonyLib;
using UnityEngine;

namespace MiraAPI.Patches;

#pragma warning disable SA1629
/// <summary>
/// Allows players to paste text into chat.
/// Source: https://github.com/CallOfCreator/NewMod/blob/main/NewMod/Patches/ClipboardPatch.cs
/// </summary>
#pragma warning restore SA1629
[HarmonyPatch(typeof(ChatController), nameof(ChatController.Update))]
public static class ChatControllerPatch
{
    public static void Prefix(ChatController __instance)
    {
        if (!HudManager.Instance.Chat.IsOpenOrOpening) return;

        var ctrlPressed = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (!ctrlPressed || !Input.GetKeyDown(KeyCode.V)) return;

        var clipboard = GUIUtility.systemCopyBuffer;

        if (string.IsNullOrWhiteSpace(clipboard)) return;
        clipboard = clipboard.Replace("<", string.Empty)
            .Replace(">", string.Empty)
            .Replace("\r", string.Empty);

        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
            clipboard = clipboard.Replace("\n", string.Empty);

        __instance.freeChatField.textArea.SetText(__instance.freeChatField.textArea.text + clipboard);
    }
}
