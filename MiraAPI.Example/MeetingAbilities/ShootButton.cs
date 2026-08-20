using MiraAPI.MeetingAbilities;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace MiraAPI.Example.MeetingAbilities;

public class ShootButton : TargetedMeetingButton
{
    public override string Name => "Shooooooot";

    public override int MaxUses => 0;

    public override float Cooldown => 10;

    public override LoadableAsset<Sprite> Sprite => ExampleAssets.SharpshootButton;

    public override MeetingButtonUsesMode ButtonUsesMode => MeetingButtonUsesMode.PerGame;

    public override bool Enabled(RoleBehaviour r)
    {
        return true;
    }

    protected override void OnClick(PlayerVoteArea playerVoteArea)
    {
        playerVoteArea.SetDisabled();
    }
}