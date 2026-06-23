namespace MiraAPI.Modifiers;

/// <summary>
/// Represents the assignment configuration for a <see cref="Types.GameModifier"/>.
/// </summary>
/// <param name="DefaultAmount">The default amount of players that can have this <see cref="Types.GameModifier"/> in a game.</param>
/// <param name="DefaultChance">The default chance of the <see cref="Types.GameModifier"/> being assigned to a player.</param>
/// <param name="CreateAmountOption">Whether Mira should create an amount option for the <see cref="Types.GameModifier"/>.</param>
/// <param name="CreateChanceOption">Whether Mira should create a chance option for the <see cref="Types.GameModifier"/>.</param>
public record struct AssignmentConfiguration(int DefaultAmount, int DefaultChance, bool CreateAmountOption, bool CreateChanceOption);
