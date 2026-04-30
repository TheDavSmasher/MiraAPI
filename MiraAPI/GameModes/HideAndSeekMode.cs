using MiraAPI.PluginLoading;

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
}
