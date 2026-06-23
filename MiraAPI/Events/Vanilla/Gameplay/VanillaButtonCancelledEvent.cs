using MiraAPI.Hud;

namespace MiraAPI.Events.Vanilla.Gameplay;

/// <summary>
/// Invoked when a Vanilla <see cref="AbilityButton"/> click is cancelled. Do not use for <see cref="CustomActionButton"/>.
/// </summary>
/// <typeparam name="T">The vanilla <see cref="AbilityButton"/> type.</typeparam>
public sealed class VanillaButtonCancelledEvent<T> : MiraEvent where T : AbilityButton
{
    /// <summary>
    /// Gets the Vanilla <see cref="AbilityButton"/> whose click was cancelled.
    /// </summary>
    public T Button { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VanillaButtonCancelledEvent{T}"/> class.
    /// </summary>
    /// <param name="button">The vanilla <see cref="AbilityButton"/> whose click was cancelled.</param>
    public VanillaButtonCancelledEvent(T button)
    {
        Button = button;
    }
}

/// <summary>
/// Invoked when a Vanilla <see cref="AbilityButton"/> click is cancelled. Do not use for <see cref="CustomActionButton"/>s.
/// </summary>
public sealed class VanillaButtonCancelledEvent : MiraEvent
{
    /// <summary>
    /// Gets the Vanilla <see cref="AbilityButton"/> whose click was cancelled.
    /// </summary>
    public AbilityButton Button { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VanillaButtonCancelledEvent"/> class.
    /// </summary>
    /// <param name="button">The vanilla <see cref="AbilityButton"/> whose click was cancelled.</param>
    public VanillaButtonCancelledEvent(AbilityButton button)
    {
        Button = button;
    }
}
