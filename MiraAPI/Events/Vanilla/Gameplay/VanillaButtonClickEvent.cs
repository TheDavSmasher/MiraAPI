using MiraAPI.Hud;

namespace MiraAPI.Events.Vanilla.Gameplay;

/// <summary>
/// Button click event for Vanilla Buttons only. Do not use for custom buttons.
/// </summary>
/// <typeparam name="T">The Vanilla Button type.</typeparam>
public sealed class VanillaButtonClickEvent<T> : MiraCancelableEvent where T : AbilityButton
{
    /// <summary>
    /// Gets Vanilla Button that was clicked.
    /// </summary>
    public T Button { get; }

    /// <summary>
    /// Gets the generic click event that is invoked before this button-specific event.
    /// </summary>
    public VanillaButtonClickEvent GenericClickEvent { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VanillaButtonClickEvent{T}"/> class.
    /// </summary>
    /// <param name="button">The Mira Button that was clicked.</param>
    /// <param name="genericClickEvent">The generic click event invoked before button-specific events.</param>
    public VanillaButtonClickEvent(T button, VanillaButtonClickEvent genericClickEvent)
    {
        GenericClickEvent = genericClickEvent;
        Button = button;
    }
}

/// <summary>
/// Button click event for Vanilla Buttons only. Do not use for custom buttons.
/// </summary>
public sealed class VanillaButtonClickEvent : MiraCancelableEvent
{
    /// <summary>
    /// Gets Vanilla Button that was clicked.
    /// </summary>
    public AbilityButton Button { get; }

    /// <summary>
    /// Gets the target player, if any, of the Vanilla Button that was clicked.
    /// </summary>
    public PlayerControl? PlayerTarget { get; }

    /// <summary>
    /// Gets the target vent, if any, of the Vanilla Button that was clicked.
    /// </summary>
    public Vent? VentTarget { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VanillaButtonClickEvent"/> class.
    /// </summary>
    /// <param name="button">The Vanilla Button that was clicked.</param>
    /// <param name="playerTarget">The Player target, if any.</param>
    /// <param name="ventTarget">The Vent target, if any.</param>
    public VanillaButtonClickEvent(AbilityButton button, PlayerControl? playerTarget = null, Vent? ventTarget = null)
    {
        Button = button;
        PlayerTarget = playerTarget;
        VentTarget = ventTarget;
    }
}
