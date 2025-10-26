namespace MiraAPI.Events.Vanilla.Gameplay;

/// <summary>
/// The event that is invoked when the player's role is shown on the intro cutscene. Non cancelable.
/// </summary>
public class IntroRoleRevealEvent : MiraEvent
{
    /// <summary>
    /// Gets the instance of the IntroCutscene.
    /// </summary>
    public IntroCutscene IntroCutscene { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="IntroRoleRevealEvent"/> class.
    /// </summary>
    /// <param name="introCutscene">The intro cutscene.</param>
    public IntroRoleRevealEvent(IntroCutscene introCutscene)
    {
        IntroCutscene = introCutscene;
    }
}
