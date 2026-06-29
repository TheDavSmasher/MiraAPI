using System.Collections.Generic;
using MiraAPI.Voting;

namespace MiraAPI.Events.Vanilla.Meeting.Voting;

/// <summary>
/// Called in <see cref="VotingUtils.HandlePopulateResults"/>.
/// Cancelling is NOT advised but if you do plan on cancelling, please ensure you account for displaying ALL player's votes.
/// </summary>
public class PopulateResultsEvent : MiraCancelableEvent
{
    /// <summary>
    /// Gets a list of networked <see cref="CustomVote"/>s.
    /// </summary>
    public List<CustomVote> Votes { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PopulateResultsEvent"/> class.
    /// </summary>
    /// <param name="votes">The list of <see cref="CustomVote"/>s.</param>
    public PopulateResultsEvent(List<CustomVote> votes)
    {
        Votes = votes;
    }
}
