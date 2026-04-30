using MiraAPI.PluginLoading;

namespace MiraAPI.GameModes;

/// <summary>
/// The default game mode.
/// </summary>
[MiraIgnore]
public class DefaultMode : AbstractGameMode
{
    /// <inheritdoc/>
    public override string Name => "Classic";

    /// <inheritdoc/>
    public override string Description => "The classic Among Us experience";
}
