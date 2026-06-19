using MiraAPI.Modifiers.Types;

namespace MiraAPI.Example.Modifiers;

public class HighPriorityModifier : GameModifier
{
    public override string ModifierName => "modifier.highPriority.name";

    public override string GetDescription()
    {
        return "modifier.highPriority.description";
    }

    public override int GetAssignmentChance()
    {
        return 100;
    }

    public override int GetAmountPerGame()
    {
        return 1;
    }

    public override int Priority()
    {
        return 100;
    }
}
