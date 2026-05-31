using MiraAPI.PluginLoading;

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
    public override string Description => "The classic Among Us experience";
}
