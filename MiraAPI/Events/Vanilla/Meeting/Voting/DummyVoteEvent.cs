using System;
using MiraAPI.Utilities;
using MiraAPI.Voting;

namespace MiraAPI.Events.Vanilla.Meeting.Voting;

/// <summary>
/// Invoked before the dummy selects their vote. If canceled, the dummy won't vote.
/// </summary>
public class DummyVoteEvent : MiraCancelableEvent
{
    /// <summary>
    /// Gets the dummy that needs to vote.
    /// </summary>
    public DummyBehaviour Dummy { get; }

    /// <summary>
    /// Gets the instance of the dummy's vote data.
    /// </summary>
    public PlayerVoteData VoteData { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the dummy can vote to skip the meeting.
    /// </summary>
    public bool CanSkip { get; set; }

    /// <summary>
    /// Gets or sets a predicate for validating whether the dummy can vote on a player.
    /// By default, excludes dead and disconnected players.
    /// </summary>
    public Predicate<PlayerControl> PlayerIsValid { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DummyVoteEvent"/> class.
    /// </summary>
    /// <param name="dummy">The voting dummy.</param>
    public DummyVoteEvent(DummyBehaviour dummy)
    {
        Dummy = dummy;
        VoteData = dummy.myPlayer.GetVoteData();
        CanSkip = true;
        PlayerIsValid = x => !x.Data.IsDead && !x.Data.Disconnected;
    }
}
