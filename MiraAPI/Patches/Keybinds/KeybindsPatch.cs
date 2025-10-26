using HarmonyLib;
using MiraAPI.Keybinds;
using MonoMod.Utils;
using Rewired;

namespace MiraAPI.Patches.Keybinds;

[HarmonyPatch(typeof(InputManager_Base), nameof(InputManager_Base.Awake))]
public static class KeybindMenuPatch
{
    private static bool _registered;

    [HarmonyPrefix]
    private static void AwakePrefix(InputManager_Base __instance)
    {
        if (_registered)
        {
            return;
        }

        KeybindUtils.RewiredInputManager = __instance;
        KeybindManager.RewiredInit();
        _registered = true;

        // Set these here right after input manager initializes
        KeybindManager.VanillaKeybinds = new()
        {
            { typeof(KillButton), new VanillaKeybind(8) },
            { typeof(UseButton), new VanillaKeybind(6) },
            { typeof(ReportButton), new VanillaKeybind(7) },
            { typeof(VentButton), new VanillaKeybind(50) },
            { typeof(SabotageButton), new VanillaKeybind(4) },
            { typeof(AbilityButton), new VanillaKeybind(49) },
        };
    }
}
