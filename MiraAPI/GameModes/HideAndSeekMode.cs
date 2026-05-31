using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AmongUs.GameOptions;
using InnerNet;
using MiraAPI.GameOptions;
using MiraAPI.HnsReimplemented.Options;
using MiraAPI.PluginLoading;
using MiraAPI.Roles;
using MiraAPI.Utilities;
using PowerTools;
using UnityEngine;

namespace MiraAPI.GameModes;

/// <summary>
/// The vanilla Hide and Seek game mode, ported to Mira.
/// </summary>
[MiraIgnore]
public class HideAndSeekMode : AbstractGameMode
{
    /// <inheritdoc/>
    public override string Name => "Hide n Seek";

    /// <inheritdoc/>
    public override string Description => "You can run, but you can't hide!";

    public override Color Color { get; } = new Color32(255, 88, 90, 255);

    public override bool CanReport(DeadBody body) => false;
    public override bool ShouldShowSabotageMap(MapBehaviour map) => false;
    public override bool ShowGameModeIntroCutscene => true;
    public override bool GameModeBodyTypeOverride => true;
    public override bool IsHideAndSeek => true;
    public override bool ShowNormalGameSettings => false;

    /*public override void AssignRoles(out bool runOriginal, LogicRoleSelectionNormal instance)
    {
        runOriginal = false;
        List<ClientData> list = new List<ClientData>();
        AmongUsClient.Instance.GetAllClients(list);
        List<NetworkedPlayerInfo> list2 = (from c in list
            where c.Character != null
            where c.Character.Data != null
            where !c.Character.Data.Disconnected && !c.Character.Data.IsDead
            orderby c.Id
            select c.Character.Data).ToList<NetworkedPlayerInfo>();
        foreach (NetworkedPlayerInfo networkedPlayerInfo in GameData.Instance.AllPlayers)
        {
            if (networkedPlayerInfo.Object != null && networkedPlayerInfo.Object.isDummy)
            {
                list2.Add(networkedPlayerInfo);
            }
        }
        IGameOptions currentGameOptions = GameOptionsManager.Instance.CurrentGameOptions;
        int adjustedNumImpostors = GameOptionsManager.Instance.CurrentGameOptions.GetAdjustedNumImpostors(list2.Count);
        this.DebugRoleAssignments(list2, ref adjustedNumImpostors);
        GameManager.Instance.LogicRoleSelection.AssignRolesForTeam(list2, currentGameOptions, RoleTeamTypes.Impostor, adjustedNumImpostors, new RoleTypes?(RoleTypes.Impostor));
        GameManager.Instance.LogicRoleSelection.AssignRolesForTeam(list2, currentGameOptions, RoleTeamTypes.Crewmate, int.MaxValue, new RoleTypes?(RoleTypes.Crewmate));
    }
    public static bool AssignRolesForTeam(
        LogicRoleSelectionHnS __instance,
        List<NetworkedPlayerInfo> players,
        IGameOptions opts,
        RoleTeamTypes team,
        int teamMax,
        Il2CppSystem.Nullable<RoleTypes> defaultRole)
    {
        Error($"MiraAPI.Patches.Roles.LogicRoleSelectionHnsPatch - AssignRolesForTeam: Team: {team}, Max: {teamMax}, Players: {players.Count}, DefaultRole: {defaultRole}");
        int num = 0;
        IRoleOptionsCollection roleOptions = opts.RoleOptions;
        var source = RoleManager.Instance.AllRoles.ToArray()
            .Where(role => role.TeamType == team && !RoleManager.IsGhostRole(role.Role) &&
                           CustomRoleUtils.CanSpawnOnCurrentMode(role));
        var assignmentData = source.Where(x => !x.IsDead).Select(role =>
            new RoleManager.RoleAssignmentData(
                role,
                roleOptions.GetNumPerGame(role.Role),
                roleOptions.GetChancePerGame(role.Role))).ToList();
        var source2 = CustomRoleUtils.GetPossibleRoles(assignmentData, x => x.Chance == 100);
        var guaranteedRoles = source.Where(x => source2.Contains(((ushort)x.Role, 100)));
        List<RoleTypes> list = new List<RoleTypes>();
        if (team == RoleTeamTypes.Crewmate)
        {
            Error($"MiraAPI.Patches.Roles.LogicRoleSelectionHnsPatch - AssignRolesForTeam: Before Guaranteed Assignment");
            foreach (RoleManager.RoleAssignmentData roleAssignmentData in guaranteedRoles.Select((x) =>
                         new RoleManager.RoleAssignmentData(x, roleOptions.GetNumPerGame(x.Role), 100)))
            {
                while (true)
                {
                    RoleManager.RoleAssignmentData roleAssignmentData2 = roleAssignmentData;
                    int count = roleAssignmentData2.Count;
                    roleAssignmentData2.Count = count - 1;
                    if (count <= 0)
                    {
                        break;
                    }

                    list.Add(roleAssignmentData.Role.Role);
                }
            }
            Error($"MiraAPI.Patches.Roles.LogicRoleSelectionHnsPatch - AssignRolesForTeam: After Guaranteed Assignment");
            AssignRolesFromList(players, teamMax, list, ref num);

            var list2 = source.Where(x => !x.IsDead).Select(role =>
                new RoleManager.RoleAssignmentData(
                    role,
                    roleOptions.GetNumPerGame(role.Role),
                    roleOptions.GetChancePerGame(role.Role))).ToList();

            list.Clear();
            foreach (RoleManager.RoleAssignmentData roleAssignmentData3 in list2)
            {
                for (int i = 0; i < roleAssignmentData3.Count; i++)
                {
                    if (HashRandom.Next(101) < roleAssignmentData3.Chance)
                    {
                        list.Add(roleAssignmentData3.Role.Role);
                    }
                }
            }
            Error($"MiraAPI.Patches.Roles.LogicRoleSelectionHnsPatch - AssignRolesForTeam: After Potential Assignment");

            AssignRolesFromList(players, teamMax, list, ref num);
            var basicRole = RoleTypes.Engineer;
            while (list.Count < players.Count && list.Count + num < teamMax)
            {
                list.Add(basicRole);
            }

            AssignRolesFromList(players, teamMax, list, ref num);
            Error($"MiraAPI.Patches.Roles.LogicRoleSelectionHnsPatch - AssignRolesForTeam: After Fallback Assignment");
        }
        else if (team == RoleTeamTypes.Impostor)
        {
            Error($"MiraAPI.Patches.Roles.LogicRoleSelectionHnsPatch - AssignRolesForTeam: Before Guaranteed Assignment");
            var newImpostors = new List<NetworkedPlayerInfo>();
            // Specified Seeker
            if (__instance.hnsManager.LogicOptionsHnS.HasImpostorPlayerID() &&
                __instance.hnsManager.LogicOptionsHnS.ValidateImpostorPlayerID(players) &&
                !AmongUsClient.Instance.IsGamePublic)
            {
                NetworkedPlayerInfo networkedPlayerInfo = players.ToArray()
                    .First(p => p.PlayerId == __instance.hnsManager.LogicOptionsHnS.ImpostorPlayerID());
                players.Remove(networkedPlayerInfo);
                newImpostors.Add(networkedPlayerInfo);
                Error($"MiraAPI.Patches.Roles.LogicRoleSelectionHnsPatch - AssignRolesForTeam: Seeker is {networkedPlayerInfo.PlayerName}, ID: {networkedPlayerInfo.PlayerId}");
            }
            // Random Seeker
            else
            {
                int num2 = 0;
                while (num2 < teamMax && players.Count > 0)
                {
                    PseudoRandomList<NetworkedPlayerInfo> pseudoRandomList = new PseudoRandomList<NetworkedPlayerInfo>(AmongUsClient.Instance.GameId);
                    players._items.Do(x => pseudoRandomList.Add(x));
                    for (int i = 0; i < GameData.RoundsPlayedInSession; i++)
                    {
                        pseudoRandomList.PickRandom();
                    }
                    NetworkedPlayerInfo networkedPlayerInfo = pseudoRandomList.PickRandom();
                    players.Remove(networkedPlayerInfo);
                    newImpostors.Add(networkedPlayerInfo);
                    num2++;
                    Error($"MiraAPI.Patches.Roles.LogicRoleSelectionHnsPatch - AssignRolesForTeam: Seeker is {networkedPlayerInfo.PlayerName}, ID: {networkedPlayerInfo.PlayerId}");
                }
            }
            Error($"MiraAPI.Patches.Roles.LogicRoleSelectionHnsPatch - AssignRolesForTeam: After Guaranteed Assignment");
            foreach (RoleManager.RoleAssignmentData roleAssignmentData in guaranteedRoles.Select((x) =>
                         new RoleManager.RoleAssignmentData(x, roleOptions.GetNumPerGame(x.Role), 100)))
            {
                while (true)
                {
                    RoleManager.RoleAssignmentData roleAssignmentData2 = roleAssignmentData;
                    int count = roleAssignmentData2.Count;
                    roleAssignmentData2.Count = count - 1;
                    if (count <= 0)
                    {
                        break;
                    }

                    list.Add(roleAssignmentData.Role.Role);
                }
            }
            AssignRolesFromList(newImpostors, teamMax, list, ref num);

            var list2 = source.Where(x => !x.IsDead).Select(role =>
                new RoleManager.RoleAssignmentData(
                    role,
                    roleOptions.GetNumPerGame(role.Role),
                    roleOptions.GetChancePerGame(role.Role))).ToList();

            list.Clear();
            foreach (RoleManager.RoleAssignmentData roleAssignmentData3 in list2)
            {
                for (int i = 0; i < roleAssignmentData3.Count; i++)
                {
                    if (HashRandom.Next(101) < roleAssignmentData3.Chance)
                    {
                        list.Add(roleAssignmentData3.Role.Role);
                    }
                }
            }

            AssignRolesFromList(newImpostors, teamMax, list, ref num);
            var basicRole = RoleTypes.Impostor;
            while (list.Count < newImpostors.Count && list.Count + num < teamMax)
            {
                list.Add(basicRole);
            }

            AssignRolesFromList(newImpostors, teamMax, list, ref num);
        }
        return false;
    }

    public static void AssignRolesFromList(List<NetworkedPlayerInfo> players, int teamMax, List<RoleTypes> roleList, ref int rolesAssigned)
    {
        while (roleList.Count > 0 && players.Count > 0 && rolesAssigned < teamMax)
        {
            int index = HashRandom.FastNext(roleList.Count);
            RoleTypes roleType = roleList[index];
            roleList.RemoveAt(index);
            int index2 = HashRandom.FastNext(players.Count);
            players[index2].Object.RpcSetRole(roleType, false);
            players.RemoveAt(index2);
            rolesAssigned++;
        }
    }*/

    public override IEnumerator IntroCutscene(IntroCutscene __instance)
    {
        deadPlayerCount = 0;
        SoundManager.Instance.PlaySound(__instance.IntroStinger, false, 1f, null);
        ShipStatus.Instance.BreakEmergencyButton();
        Logger.GlobalInstance.Info("IntroCutscene :: CoBegin() :: Game Mode: Hide and Seek (MiraAPI)", null);
        __instance.LogPlayerRoleData();
        __instance.HideAndSeekPanels.SetActive(true);
        if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
        {
            __instance.CrewmateRules.SetActive(false);
            __instance.ImpostorRules.SetActive(true);
        }
        else
        {
            __instance.CrewmateRules.SetActive(true);
            __instance.ImpostorRules.SetActive(false);
        }

        __instance.ImpostorName.gameObject.SetActive(true);
        __instance.ImpostorTitle.gameObject.SetActive(true);
        __instance.BackgroundBar.enabled = false;
        __instance.TeamTitle.gameObject.SetActive(false);
        var impostor = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(x => x.Data.Role.IsImpostor);
        if (impostor == null)
        {
            Logger.GlobalInstance.Error("IntroCutscene :: CoBegin() :: impostor is NULL", null);
        }

        GameManager.Instance.SetSpecialCosmetics(impostor);
        if (impostor != null)
        {
            __instance.ImpostorName.text = impostor.Data.PlayerName;
        }
        else
        {
            __instance.ImpostorName.text = "???";
        }

        yield return new WaitForSecondsRealtime(0.1f);
        if (impostor != null)
        {
            __instance.ImpostorTitle.text = impostor.Data.Role.GetRoleName();
        }
        PoolablePlayer playerSlot = null;
        if (impostor != null)
        {
            playerSlot = __instance.CreatePlayer(1, 1, impostor.Data, false);
            playerSlot.SetBodyType(PlayerBodyTypes.Normal);
            playerSlot.SetFlipX(false);
            playerSlot.transform.localPosition = __instance.impostorPos;
            playerSlot.transform.localScale = Vector3.one * __instance.impostorScale;
        }

        yield return ShipStatus.Instance.CosmeticsCache.PopulateFromPlayers();
        yield return new WaitForSecondsRealtime(6f);
        if (playerSlot != null)
        {
            playerSlot.gameObject.SetActive(false);
        }

        __instance.HideAndSeekPanels.SetActive(false);
        __instance.CrewmateRules.SetActive(false);
        __instance.ImpostorRules.SetActive(false);
        /*LogicOptionsHnS logicOptionsHnS = GameManager.Instance.LogicOptions as LogicOptionsHnS;
        LogicHnSMusic logicHnSMusic = GameManager.Instance.GetLogicComponent<LogicHnSMusic>() as LogicHnSMusic;
        if (logicHnSMusic != null)
        {
            logicHnSMusic.StartMusicWithIntro();
        }*/
        var hideTimer = 10f;

        if (PlayerControl.LocalPlayer.Data.Role.IsImpostor)
        {
            __instance.HideAndSeekTimerText.gameObject.SetActive(true);
            PoolablePlayer poolablePlayer;
            AnimationClip anim;
            if (AprilFoolsMode.ShouldHorseAround())
            {
                poolablePlayer = __instance.HorseWrangleVisualSuit;
                poolablePlayer.gameObject.SetActive(true);
                poolablePlayer.SetBodyType(PlayerBodyTypes.Seeker);
                anim = __instance.HnSSeekerSpawnHorseAnim;
                __instance.HorseWrangleVisualPlayer.SetBodyType(PlayerBodyTypes.Normal);
                __instance.HorseWrangleVisualPlayer.UpdateFromPlayerData(
                    PlayerControl.LocalPlayer.Data,
                    PlayerControl.LocalPlayer.CurrentOutfitType,
                    PlayerMaterial.MaskType.None,
                    false,
                    null,
                    false);
            }
            else if (AprilFoolsMode.ShouldLongAround())
            {
                poolablePlayer = __instance.HideAndSeekPlayerVisual;
                poolablePlayer.gameObject.SetActive(true);
                poolablePlayer.SetBodyType(PlayerBodyTypes.LongSeeker);
                anim = __instance.HnSSeekerSpawnLongAnim;
            }
            else
            {
                poolablePlayer = __instance.HideAndSeekPlayerVisual;
                poolablePlayer.gameObject.SetActive(true);
                poolablePlayer.SetBodyType(PlayerBodyTypes.Seeker);
                anim = __instance.HnSSeekerSpawnAnim;
            }

            poolablePlayer.SetBodyCosmeticsVisible(false);
            poolablePlayer.UpdateFromPlayerData(
                PlayerControl.LocalPlayer.Data,
                PlayerControl.LocalPlayer.CurrentOutfitType,
                PlayerMaterial.MaskType.None,
                false,
                null,
                false);
            SpriteAnim component = poolablePlayer.GetComponent<SpriteAnim>();
            poolablePlayer.gameObject.SetActive(true);
            poolablePlayer.ToggleName(false);
            component.Play(anim, 1f);
            while (hideTimer > 0f)
            {
                __instance.HideAndSeekTimerText.text = Mathf.RoundToInt(hideTimer).ToString();
                hideTimer -= Time.deltaTime;
                yield return null;
            }
        }
        else
        {
            ShipStatus.Instance.HideCountdown = hideTimer;
            if (AprilFoolsMode.ShouldHorseAround())
            {
                if (impostor != null)
                {
                    impostor.AnimateCustom(__instance.HnSSeekerSpawnHorseInGameAnim);
                }
            }
            else if (AprilFoolsMode.ShouldLongAround())
            {
                if (impostor != null)
                {
                    impostor.AnimateCustom(__instance.HnSSeekerSpawnLongInGameAnim);
                }
            }
            else if (impostor != null)
            {
                impostor.AnimateCustom(__instance.HnSSeekerSpawnAnim);
                impostor.cosmetics.SetBodyCosmeticsVisible(false);
            }
        }
        ShipStatus.Instance.StartSFX();
        UnityEngine.Object.Destroy(__instance.gameObject);
    }

    public override PlayerBodyTypes GetBodyType(PlayerControl player)
    {
        if (player == null || player.Data == null || player.Data.Role == null)
        {
            if (AprilFoolsMode.ShouldHorseAround())
            {
                return PlayerBodyTypes.Horse;
            }
            if (AprilFoolsMode.ShouldLongAround())
            {
                return PlayerBodyTypes.Long;
            }
            return PlayerBodyTypes.Normal;
        }
        else if (AprilFoolsMode.ShouldHorseAround())
        {
            if (player.Data.Role.IsImpostor)
            {
                return PlayerBodyTypes.Normal;
            }
            return PlayerBodyTypes.Horse;
        }
        else if (AprilFoolsMode.ShouldLongAround())
        {
            if (player.Data.Role.IsImpostor)
            {
                return PlayerBodyTypes.LongSeeker;
            }
            return PlayerBodyTypes.Long;
        }
        else
        {
            if (player.Data.Role.IsImpostor)
            {
                return PlayerBodyTypes.Seeker;
            }
            return PlayerBodyTypes.Normal;
        }
    }
    private int deadPlayerCount;
    public override void OnPlayerDeath(PlayerControl player, bool assignGhostRole)
    {
        base.OnPlayerDeath(player, assignGhostRole);
        HudManager.Instance.NotifyOfDeath();
        var popup = GameManagerCreator.Instance.HideAndSeekManagerPrefab.DeathPopupPrefab;
        deadPlayerCount++;
        var item = UnityEngine.Object.Instantiate(popup, HudManager.Instance.transform.parent);
        item.Show(player, deadPlayerCount);
    }
}
