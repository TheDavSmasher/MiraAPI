namespace MiraAPI.Events.Vanilla.Gameplay;

/// <summary>
/// Start Round event, invoked on <see cref="IntroCutscene.OnDestroy"/> and <see cref="ExileController.WrapUp"/>
/// if the <see cref="BeforeRoundStartEvent"/> isn't cancelled.
/// </summary>
public class RoundStartEvent : MiraEvent
{
    /// <summary>
    /// Gets a value indicating whether the event was triggered by the <see cref="IntroCutscene"/> or <see cref="ExileController"/>.
    /// </summary>
    public bool TriggeredByIntro { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RoundStartEvent"/> class.
    /// </summary>
    /// <param name="triggeredByIntro">Whether the event was triggered by the intro or not.</param>
    public RoundStartEvent(bool triggeredByIntro)
    {
        TriggeredByIntro = triggeredByIntro;
    }
}
