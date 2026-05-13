using System.Collections;
using System.Collections.Generic;
using MiraAPI.GameOptions;
using MiraAPI.PluginLoading;
using UnityEngine;

namespace MiraAPI.GameModes;

/// <summary>
/// Base class for custom gamemodes.
/// </summary>
[MiraIgnore]
public abstract class AbstractGameMode : IOptionable
{
    /// <summary>
    /// Gets the game mode name.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Gets the game mode description.
    /// </summary>
    public abstract string Description { get; }

    /// <summary>
    /// Gets the game mode description.
    /// </summary>
    public virtual Color Color { get; } = Color.white;

    /// <summary>
    /// Gets or sets the game mode id.
    /// </summary>
    public uint ID { get; set; }

    /// <summary>
    /// Gets a value indicating whether a specific body type is used by the game mode.
    /// </summary>
    public virtual bool GameModeBodyTypeOverride => false;

    /// <summary>
    /// Called on GameManager.GetBodyType().
    /// </summary>
    /// <param name="player">Player to get body type from.</param>
    /// <returns>Resulting body type.</returns>
    public virtual PlayerBodyTypes GetBodyType(PlayerControl player)
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

    /// <summary>
    /// Gets a value indicating whether a custom intro sequence is implemented by the game mode.
    /// </summary>
    public virtual bool ShowGameModeIntroCutscene => false;

    /// <summary>
    /// Called when Intro Cutscene is destroyed.
    /// </summary>
    public virtual void Initialize()
    {
    }

    /// <summary>
    /// Called on HudManager.Start().
    /// </summary>
    /// <param name="instance">Instance of HudManager.</param>
    public virtual void HudStart(HudManager instance)
    {
    }

    /// <summary>
    /// Called every frame on HudManager.Update().
    /// </summary>
    /// <param name="instance">Instance of HudManager.</param>
    public virtual void HudUpdate(HudManager instance)
    {
    }

    /// <summary>
    /// Called when a player is killed.
    /// </summary>
    /// <param name="player">PlayerControl that was killed.</param>
    /// <param name="assignGhostRole">Determines whether to assign a ghost role on the player.</param>
    public virtual void OnPlayerDeath(PlayerControl player, bool assignGhostRole)
    {
        GameManager.Instance.LogicRoleSelection.OnPlayerDeath(player, assignGhostRole);
    }

    /// <summary>
    /// Calculate Game End logic.
    /// </summary>
    /// <param name="runOriginal">Should original game end logic be used.</param>
    /// <param name="instance">Instance of LogicGameFlowNormal.</param>
    public virtual void CheckGameEnd(out bool runOriginal, LogicGameFlowNormal instance)
    {
        runOriginal = true;
    }

    /// <summary>
    /// Custom role assignment.
    /// </summary>
    /// <param name="runOriginal">Should original role assignment be used.</param>
    /// <param name="instance">Instance of LogicRoleSelection.</param>
    public virtual void AssignRoles(out bool runOriginal, LogicRoleSelectionNormal instance)
    {
        runOriginal = true;
    }

    /// <summary>
    /// Custom murder logic.
    /// </summary>
    /// <param name="runOriginal">Should the original murder logic be used.</param>
    /// <param name="result">Result of custom murder logic.</param>
    /// <param name="target">Target player for murder.</param>
    public virtual void CanKill(out bool runOriginal, out bool result, PlayerControl target)
    {
        result = false;
        runOriginal = true;
    }

    /// <summary>
    /// Custom winner selection.
    /// </summary>
    /// <returns>List of winners or null.</returns>
    public virtual List<NetworkedPlayerInfo>? CalculateWinners() => null;

    /// <summary>
    /// The IEnumerator that plays the intro cutscene for this gamemode.
    /// </summary>
    /// <param name="__instance">An instance of IntroCutscene.</param>
    /// <returns>An IEnumerator to run the intro cutscene instead of the base game one.</returns>
    public virtual IEnumerator IntroCutscene(IntroCutscene __instance)
    {
        yield return new WaitForEndOfFrame();
    }

    /// <summary>
    /// Can Admin be used in this gamemode.
    /// </summary>
    /// <param name="console">Admin Console.</param>
    /// <returns>True if Admin console is enabled.</returns>
    public virtual bool CanUseMapConsole(MapConsole console) => true;

    /// <summary>
    /// Can a body be reported in this gamemode.
    /// </summary>
    /// <param name="body">Target body for reporting.</param>
    /// <returns>True if dead bodies can be reported.</returns>
    public virtual bool CanReport(DeadBody body) => true;

    /// <summary>
    /// Can system consoles be used in this gamemode.
    /// </summary>
    /// <param name="console">System Console.</param>
    /// <returns>True if system consoles are enabled in this mode.</returns>
    public virtual bool CanUseSystemConsole(SystemConsole console) => true;

    /// <summary>
    /// Can tasks be interacted with in this gamemode.
    /// </summary>
    /// <param name="console">Task console.</param>
    /// <returns>True if tasks are enabled in this mode.</returns>
    public virtual bool CanUseTasks(Console console) => true;

    /// <summary>
    /// Should the sabotage map be used when attempting to open sabotage overlay.
    /// </summary>
    /// <param name="map">MapBehaviour object.</param>
    /// <returns>True if the sabotage map should be shown.</returns>
    public virtual bool ShouldShowSabotageMap(MapBehaviour map) => true;

    /// <summary>
    /// Can a player vent in this gamemode.
    /// </summary>
    /// <param name="vent">Target vent.</param>
    /// <param name="playerInfo">Player attempting to vent.</param>
    /// <returns>True if venting is enabled in this mode.</returns>
    public virtual bool CanVent(Vent vent, NetworkedPlayerInfo playerInfo) => true;

    /// <inheritdoc/>
    public override string ToString() => Name;
}
