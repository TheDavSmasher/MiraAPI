using MiraAPI.Networking;

namespace MiraAPI.Events.Vanilla.Gameplay;

/// <summary>
/// Event that is invoked before a player is murdered. This event is cancelable.
/// </summary>
public sealed class BeforeMurderEvent : MiraCancelableEvent
{
    /// <summary>
    /// Gets the <see cref="PlayerControl"/> that is killing the <see cref="Target"/>.
    /// </summary>
    public PlayerControl Source { get; }

    /// <summary>
    /// Gets the <see cref="PlayerControl"/> that is being killed.
    /// </summary>
    public PlayerControl Target { get; }

    /// <summary>
    /// Gets the <see cref="PlayerControl"/> that is being framed, if any.
    /// </summary>
    public PlayerControl? Framed { get; }

    /// <summary>
    /// Gets a value indicating whether the murder is done indirectly.
    /// This is commonly used to prevent the <see cref="Source"/> <see cref="PlayerControl"/> from being killed.
    /// </summary>
    public bool IsIndirectAttack { get; }

    /// <summary>
    /// Gets a value indicating whether the murder should bypass protective roles or general defense, such as <see cref="GuardianAngelRole"/>.
    /// </summary>
    public bool IgnoreDefense { get; }

    /// <summary>
    /// Gets whether the murder was meant to be done in a meeting, via an enum.
    /// </summary>
    public MeetingCheck InMeeting { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BeforeMurderEvent"/> class.
    /// </summary>
    /// <param name="source">The <see cref="PlayerControl"/> that is killing the <paramref name="target"/>.</param>
    /// <param name="target">The <see cref="PlayerControl"/> that is being killed.</param>
    /// <param name="isIndirect">Whether the murder is caused indirectly.</param>
    /// <param name="ignoreDefense">Whether the murder should ignore protection.</param>
    /// <param name="inMeeting">Whether the murder is intended to be triggered in a meeting.</param>
    public BeforeMurderEvent(PlayerControl source, PlayerControl target, bool isIndirect, bool ignoreDefense = false, MeetingCheck inMeeting = MeetingCheck.Ignore)
    {
        Source = source;
        Target = target;
        Framed = null;
        IsIndirectAttack = isIndirect;
        IgnoreDefense = ignoreDefense;
        InMeeting = inMeeting;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BeforeMurderEvent"/> class.
    /// </summary>
    /// <param name="source">The <see cref="PlayerControl"/> that is killing the <paramref name="target"/>.</param>
    /// <param name="target">The <see cref="PlayerControl"/> that is being killed.</param>
    /// <param name="framed">The <see cref="PlayerControl"/> that is being framed.</param>
    /// <param name="isIndirect">Whether the murder is caused indirectly.</param>
    /// <param name="ignoreDefense">Whether the murder should ignore protection.</param>
    /// <param name="inMeeting">Whether the murder is intended to be triggered in a meeting.</param>
    public BeforeMurderEvent(PlayerControl source, PlayerControl target, PlayerControl framed, bool isIndirect = true, bool ignoreDefense = false, MeetingCheck inMeeting = MeetingCheck.Ignore)
    {
        Source = source;
        Target = target;
        Framed = framed;
        IsIndirectAttack = isIndirect;
        IgnoreDefense = ignoreDefense;
        InMeeting = inMeeting;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BeforeMurderEvent"/> class.
    /// </summary>
    /// <param name="source">The <see cref="PlayerControl"/> that is killing the <paramref name="target"/>.</param>
    /// <param name="target">The <see cref="PlayerControl"/> that is being killed.</param>
    /// <param name="inMeeting">Whether the murder is intended to be triggered in a meeting.</param>
    public BeforeMurderEvent(PlayerControl source, PlayerControl target, MeetingCheck inMeeting)
    {
        Source = source;
        Target = target;
        Framed = null;
        IsIndirectAttack = false;
        IgnoreDefense = false;
        InMeeting = inMeeting;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BeforeMurderEvent"/> class.
    /// </summary>
    /// <param name="source">The <see cref="PlayerControl"/> that is killing the <paramref name="target"/>.</param>
    /// <param name="target">The <see cref="PlayerControl"/> that is being killed.</param>
    public BeforeMurderEvent(PlayerControl source, PlayerControl target)
    {
        Source = source;
        Target = target;
        Framed = null;
        IsIndirectAttack = false;
        IgnoreDefense = false;
        InMeeting = MeetingCheck.Ignore;
    }
}
