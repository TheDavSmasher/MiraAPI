using HarmonyLib;
using MiraAPI.GameModes;

namespace MiraAPI.Patches.GameModes;

[HarmonyPatch]
internal static class GameLogicPatches
{
    [HarmonyPrefix, HarmonyPatch(typeof(NormalGameManager), nameof(NormalGameManager.GetBodyType))]
    public static bool BodyTypePatch(NormalGameManager __instance, PlayerControl player, ref PlayerBodyTypes __result)
    {
        if (CustomGameModeManager.ActiveMode == null || !CustomGameModeManager.ActiveMode.GameModeBodyTypeOverride)
        {
            return true;
        }
        __result = CustomGameModeManager.ActiveMode.GetBodyType(player);
        return false;
    }
}
