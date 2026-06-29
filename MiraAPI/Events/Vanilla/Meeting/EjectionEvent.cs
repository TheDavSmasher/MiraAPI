namespace MiraAPI.Events.Vanilla.Meeting;

/// <summary>
/// The event that is invoked when a player is ejected. Non-cancelable.
/// </summary>
public class EjectionEvent : MiraEvent
{
    /// <summary>
    /// Gets the <see cref="global::ExileController"/> instance.
    /// </summary>
    public ExileController ExileController { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EjectionEvent"/> class.
    /// </summary>
    /// <param name="controller">The <see cref="global::ExileController"/> instance.</param>
    public EjectionEvent(ExileController controller)
    {
        ExileController = controller;
    }
}
