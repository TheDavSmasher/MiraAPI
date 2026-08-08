using AmongUs.GameOptions;
using HarmonyLib;
using Il2CppSystem.Collections.Generic;
using System.Linq;
using MiraAPI.Roles;

namespace MiraAPI.Patches.Roles;

[HarmonyPatch(typeof(LogicRoleSelectionNormal))]
public static class LogicRoleSelectionNormalPatch
{
    [HarmonyPrefix]
    [HarmonyPatch(nameof(LogicRoleSelectionNormal.AssignRolesForTeam))]
    public static bool AssignRolesForTeam(
        LogicRoleSelectionNormal __instance,
        List<NetworkedPlayerInfo> players,
        IGameOptions opts,
        RoleTeamTypes team,
        int teamMax,
        Il2CppSystem.Nullable<RoleTypes> defaultRole)
    {
        int num = 0;
        var source = RoleManager.Instance.AllRoles.ToArray()
            .Where(role => role.TeamType == team && !RoleManager.IsGhostRole(role.Role) &&
                           CustomRoleUtils.CanSpawnOnCurrentMode(role));
        List<RoleTypes> list = new List<RoleTypes>();
        IRoleOptionsCollection roleOptions = opts.RoleOptions;

        // Assign guaranteed roles first, just like the vanilla selector. This is
        // important because the list of players is shared by both team passes.
        foreach (var role in source.Where(x => roleOptions.GetChancePerGame(x.Role) == 100))
        {
            for (var i = 0; i < roleOptions.GetNumPerGame(role.Role); i++)
            {
                list.Add(role.Role);
            }
        }

        __instance.AssignRolesFromList(players, teamMax, list, ref num);

        // A 100% role was already assigned above. Including it here can consume
        // another player and, more importantly, leaves the fallback count wrong.
        list.Clear();
        foreach (var role in source.Where(x =>
                     roleOptions.GetChancePerGame(x.Role) > 0 &&
                     roleOptions.GetChancePerGame(x.Role) < 100))
        {
            for (var i = 0; i < roleOptions.GetNumPerGame(role.Role); i++)
            {
                if (HashRandom.Next(101) < roleOptions.GetChancePerGame(role.Role))
                {
                    list.Add(role.Role);
                }
            }
        }

        __instance.AssignRolesFromList(players, teamMax, list, ref num);

        // Do not read defaultRole here. The nullable argument is not marshalled
        // reliably by IL2CPP and can return Crewmate for the impostor pass.
        var defaultRole2 = team is RoleTeamTypes.Crewmate ? RoleTypes.Crewmate : RoleTypes.Impostor;

        // Assign the remaining players up to the requested team limit. The
        // vanilla list has been consumed by AssignRolesFromList at this point,
        // so checking list.Count can leave players unassigned.
        while (players.Count > 0 && num < teamMax)
        {
            list.Add(defaultRole2);
            __instance.AssignRolesFromList(players, teamMax, list, ref num);
        }

        return false;
    }
}
