using MiraAPI.Hud;

namespace MiraAPI.Events.Vanilla.Gameplay;

/// <summary>
/// Invoked when a Vanilla Button click is cancelled. Do not use for custom buttons.
/// </summary>
/// <typeparam name="T">The Vanilla Button type.</typeparam>
public sealed class VanillaButtonCancelledEvent<T> : MiraEvent where T : AbilityButton
{
    /// <summary>
    /// Gets Vanilla Button whose click was cancelled.
    /// </summary>
    public T Button { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VanillaButtonCancelledEvent{T}"/> class.
    /// </summary>
    /// <param name="button">The Mira Button whose click was cancelled.</param>
    public VanillaButtonCancelledEvent(T button)
    {
        Button = button;
    }
}

/// <summary>
/// Invoked when a Vanilla Button click is cancelled. Do not use for custom buttons.
/// </summary>
public sealed class VanillaButtonCancelledEvent : MiraEvent
{
    /// <summary>
    /// Gets Vanilla Button whose click was cancelled.
    /// </summary>
    public AbilityButton Button { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VanillaButtonCancelledEvent"/> class.
    /// </summary>
    /// <param name="button">The Vanilla Button whose click was cancelled.</param>
    public VanillaButtonCancelledEvent(AbilityButton button)
    {
        Button = button;
    }
}
