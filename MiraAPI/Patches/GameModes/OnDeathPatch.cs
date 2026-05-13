using HarmonyLib;
using MiraAPI.GameModes;

namespace MiraAPI.Patches.GameModes;

[HarmonyPatch(typeof(GameManager))]
internal static class OnDeathPatch
{
    [HarmonyPrefix, HarmonyPatch(nameof(GameManager.OnPlayerDeath))]
    public static bool OnDeathPostfix([HarmonyArgument(0)] PlayerControl player, [HarmonyArgument(1)] bool assignGhostRole)
    {
        if (CustomGameModeManager.ActiveMode != null)
        {
            CustomGameModeManager.ActiveMode.OnPlayerDeath(player, assignGhostRole);
            return false;
        }

        return true;
    }
}
