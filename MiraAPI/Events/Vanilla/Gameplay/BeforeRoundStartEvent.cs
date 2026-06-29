namespace MiraAPI.Events.Vanilla.Gameplay;

/// <summary>
/// Event invoked on <see cref="IntroCutscene.OnDestroy"/> and <see cref="ExileController.WrapUp"/>
/// to determine if it should run the <see cref="RoundStartEvent"/> event or not.
/// </summary>
public class BeforeRoundStartEvent : MiraCancelableEvent
{
    /// <summary>
    /// Gets a value indicating whether the event was triggered by the <see cref="IntroCutscene"/> or <see cref="ExileController"/>.
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
