using MiraAPI.Modifiers.Types;

namespace MiraAPI.Example.Modifiers;

public class HighPriorityModifier : GameModifier
{
    public override string ModifierName => "High Priority";

    public override string GetDescription()
    {
        return "You are high priority! Idrk what this means either tbh...";
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
