namespace MiraAPI.Events.Vanilla.Usables;

/// <summary>
/// Event that is invoked when a player enters a vent. This event is cancelable.
/// </summary>
public class EnterVentEvent : MiraCancelableEvent
{
    /// <summary>
    /// Gets the <see cref="PlayerControl"/> that is entering the <see cref="Vent"/>.
    /// </summary>
    public PlayerControl Player { get; }

    /// <summary>
    /// Gets the <see cref="global::Vent"/> that the <see cref="Player"/> is entering.
    /// </summary>
    public Vent? Vent { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EnterVentEvent"/> class.
    /// </summary>
    /// <param name="player">The <see cref="PlayerControl"/> that is entering the <paramref name="vent"/>.</param>
    /// <param name="vent">The <see cref="global::Vent"/> being entered.</param>
    public EnterVentEvent(PlayerControl player, Vent? vent)
    {
        Player = player;
        Vent = vent;
    }
}
