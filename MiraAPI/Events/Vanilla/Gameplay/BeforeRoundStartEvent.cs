namespace MiraAPI.Events.Vanilla.Gameplay;

/// <summary>
/// Event invoked on IntroCutscene.OnDestroy and ExileController.WrapUp to determine if it should run the round event or not.
/// </summary>
public class BeforeRoundStartEvent : MiraCancelableEvent
{
    /// <summary>
    /// Gets a value indicating whether the event was triggered by the IntroCutscene or ExileController.
    /// </summary>
    public bool TriggeredByIntro { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BeforeRoundStartEvent"/> class.
    /// </summary>
    /// <param name="triggeredByIntro">Whether the event was triggered by the intro or not.</param>
    public BeforeRoundStartEvent(bool triggeredByIntro)
    {
        TriggeredByIntro = triggeredByIntro;
    }
}
