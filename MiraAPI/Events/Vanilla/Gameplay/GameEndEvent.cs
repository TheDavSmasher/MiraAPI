namespace MiraAPI.Events.Vanilla.Gameplay;

/// <summary>
/// The event that is invoked when the end game screen is shown. Non cancelable.
/// </summary>
public class GameEndEvent : MiraEvent
{
    /// <summary>
    /// Gets the <see cref="global::EndGameManager"/> instance.
    /// </summary>
    public EndGameManager EndGameManager { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GameEndEvent"/> class.
    /// </summary>
    /// <param name="manager">The <see cref="global::EndGameManager"/> instance.</param>
    public GameEndEvent(EndGameManager manager)
    {
        EndGameManager = manager;
    }
}
