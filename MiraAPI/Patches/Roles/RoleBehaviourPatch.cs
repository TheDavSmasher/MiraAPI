using HarmonyLib;
using MiraAPI.Roles;
using UnityEngine;

namespace MiraAPI.Patches.Roles;

[HarmonyPatch(typeof(RoleBehaviour))]
public static class RoleBehaviourPatch
{
    /// <summary>
    /// Update <see cref="RoleBehaviour.TeamColor"/> text for <see cref="ICustomRole"/>s.
    /// </summary>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(RoleBehaviour.TeamColor), MethodType.Getter)]
    public static bool PrefixTeamColorGetter(RoleBehaviour __instance, ref Color __result)
    {
        if (__instance is not ICustomRole behaviour)
        {
            return true;
        }

        __result = behaviour.RoleColor;
        return false;
    }
}
