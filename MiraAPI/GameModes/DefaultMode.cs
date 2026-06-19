using MiraAPI.Translation;

namespace MiraAPI.GameModes;

public class DefaultMode : CustomGameMode
{
    public override string Name => "gamemode.default.name";
    public override string Description => "gamemode.default.description".Translate();
    public override int Id => 0;
}
