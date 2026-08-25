using MiraAPI.PluginLoading;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace MiraAPI.GameModes;

/// <summary>
/// The classic game mode.
/// </summary>
[MiraIgnore]
public class ClassicMode : AbstractGameMode
{
    /// <inheritdoc/>
    public override string Name => "gamemode.classic";

    /// <inheritdoc/>
    public override string Description => "gamemode.classic.description";

    public override LoadableAsset<Sprite>? Icon => MiraAssets.ClassicGamemodeIcon;
}
