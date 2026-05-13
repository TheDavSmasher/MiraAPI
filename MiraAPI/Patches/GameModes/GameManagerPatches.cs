using HarmonyLib;
using MiraAPI.GameModes;

namespace MiraAPI.Patches.GameModes;

[HarmonyPatch(typeof(GameManager))]
internal static class GameManagerPatches
{
    [HarmonyPrefix, HarmonyPatch(nameof(GameManager.OnPlayerDeath))]
    public static bool OnDeathPrefix([HarmonyArgument(0)] PlayerControl player, [HarmonyArgument(1)] bool assignGhostRole)
    {
        if (CustomGameModeManager.ActiveMode != null)
        {
            CustomGameModeManager.ActiveMode.OnPlayerDeath(player, assignGhostRole);
            return false;
        }

        return true;
    }
    [HarmonyPrefix, HarmonyPatch(nameof(GameManager.ShowCrewmatesKilled))]
    public static bool ShowCrewmatesKilledPrefix(ref bool __result)
    {
        if (CustomGameModeManager.ActiveMode != null)
        {
            __result = CustomGameModeManager.ActiveMode.IsHideAndSeek;
            return false;
        }

        return true;
    }
}
