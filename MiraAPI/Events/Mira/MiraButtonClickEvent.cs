using MiraAPI.Hud;

namespace MiraAPI.Events.Mira;

/// <summary>
/// Button click event for <see cref="CustomActionButton"/>s only. Do not use for vanilla <see cref="AbilityButton"/>s.
/// </summary>
/// <typeparam name="T">The <see cref="CustomActionButton"/> type.</typeparam>
public sealed class MiraButtonClickEvent<T> : MiraCancelableEvent where T : CustomActionButton
{
    /// <summary>
    /// Gets the <see cref="CustomActionButton"/> that was clicked.
    /// </summary>
    public T Button { get; }

    /// <summary>
    /// Gets the generic <see cref="MiraButtonClickEvent"/> that is invoked before this button-specific event.
    /// </summary>
    public MiraButtonClickEvent GenericClickEvent { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiraButtonClickEvent{T}"/> class.
    /// </summary>
    /// <param name="button">The <see cref="CustomActionButton"/> that was clicked.</param>
    /// <param name="genericClickEvent">The generic <see cref="MiraButtonClickEvent"/> invoked before button-specific events.</param>
    public MiraButtonClickEvent(T button, MiraButtonClickEvent genericClickEvent)
    {
        GenericClickEvent = genericClickEvent;
        Button = button;
    }
}

/// <summary>
/// Button click event for <see cref="CustomActionButton"/>s only. Do not use for vanilla <see cref="AbilityButton"/>s.
/// </summary>
public sealed class MiraButtonClickEvent : MiraCancelableEvent
{
    /// <summary>
    /// Gets the <see cref="CustomActionButton"/> that was clicked.
    /// </summary>
    public CustomActionButton Button { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MiraButtonClickEvent"/> class.
    /// </summary>
    /// <param name="button">The <see cref="CustomActionButton"/> that was clicked.</param>
    public MiraButtonClickEvent(CustomActionButton button)
    {
        Button = button;
    }
}
