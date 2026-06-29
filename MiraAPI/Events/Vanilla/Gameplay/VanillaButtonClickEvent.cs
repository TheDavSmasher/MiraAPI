using MiraAPI.Hud;

namespace MiraAPI.Events.Vanilla.Gameplay;

/// <summary>
/// Button click event for Vanilla <see cref="AbilityButton"/>s only. Do not use for <see cref="CustomActionButton"/>s.
/// </summary>
/// <typeparam name="T">The vanilla <see cref="AbilityButton"/> type.</typeparam>
public sealed class VanillaButtonClickEvent<T> : MiraCancelableEvent where T : AbilityButton
{
    /// <summary>
    /// Gets the vanilla <see cref="AbilityButton"/> that was clicked.
    /// </summary>
    public T Button { get; }

    /// <summary>
    /// Gets the generic <see cref="VanillaButtonClickEvent"/> that is invoked before this button-specific event.
    /// </summary>
    public VanillaButtonClickEvent GenericClickEvent { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VanillaButtonClickEvent{T}"/> class.
    /// </summary>
    /// <param name="button">The vanilla <see cref="AbilityButton"/> that was clicked.</param>
    /// <param name="genericClickEvent">The generic <see cref="VanillaButtonClickEvent"/> invoked before button-specific events.</param>
    public VanillaButtonClickEvent(T button, VanillaButtonClickEvent genericClickEvent)
    {
        GenericClickEvent = genericClickEvent;
        Button = button;
    }
}

/// <summary>
/// Button click event for Vanilla <see cref="AbilityButton"/> only. Do not use for <see cref="CustomActionButton"/>s.
/// </summary>
public sealed class VanillaButtonClickEvent : MiraCancelableEvent
{
    /// <summary>
    /// Gets the Vanilla <see cref="AbilityButton"/> that was clicked.
    /// </summary>
    public AbilityButton Button { get; }

    /// <summary>
    /// Gets the target <see cref="PlayerControl"/>, if any, of the Vanilla <see cref="AbilityButton"/> that was clicked.
    /// </summary>
    public PlayerControl? PlayerTarget { get; }

    /// <summary>
    /// Gets the target <see cref="Vent"/>, if any, of the Vanilla <see cref="AbilityButton"/> that was clicked.
    /// </summary>
    public Vent? VentTarget { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VanillaButtonClickEvent"/> class.
    /// </summary>
    /// <param name="button">The Vanilla <see cref="AbilityButton"/> that was clicked.</param>
    /// <param name="playerTarget">The <see cref="PlayerControl"/> target, if any.</param>
    /// <param name="ventTarget">The <see cref="Vent"/> target, if any.</param>
    public VanillaButtonClickEvent(AbilityButton button, PlayerControl? playerTarget = null, Vent? ventTarget = null)
    {
        Button = button;
        PlayerTarget = playerTarget;
        VentTarget = ventTarget;
    }
}
