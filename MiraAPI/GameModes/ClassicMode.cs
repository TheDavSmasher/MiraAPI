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
    public override string Name => "Classic";

    /// <inheritdoc/>
    public override string Description => "The classic Among Us experience!\nFind the Impostors or deceive the Crew.";

    public override LoadableAsset<Sprite>? Icon => MiraAssets.ClassicGamemodeIcon;
}
