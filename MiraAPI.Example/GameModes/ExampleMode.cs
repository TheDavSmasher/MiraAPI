using MiraAPI.GameModes;
using UnityEngine;

namespace MiraAPI.Example.GameModes;

public class ExampleMode : AbstractGameMode
{
    public override string Name => "Example Mode";
    public override string Description => "An example gamemode.";
    public override Color Color => Color.red;
}
