namespace MiraAPI.Events.Vanilla.Usables;

/// <summary>
/// Event that is invoked when a player exits a vent. This event is cancelable.
/// </summary>
public class ExitVentEvent : MiraCancelableEvent
{
    /// <summary>
    /// Gets the <see cref="PlayerControl"/> that is exiting the <see cref="Vent"/>.
    /// </summary>
    public PlayerControl Player { get; }

    /// <summary>
    /// Gets the <see cref="global::Vent"/> that the <see cref="Player"/> is exiting.
    /// </summary>
    public Vent? Vent { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExitVentEvent"/> class.
    /// </summary>
    /// <param name="player">The <see cref="PlayerControl"/> who is exiting the <paramref name="vent"/>.</param>
    /// <param name="vent">The <see cref="global::Vent"/> being exited from.</param>
    public ExitVentEvent(PlayerControl player, Vent? vent)
    {
        Player = player;
        Vent = vent;
    }
}
