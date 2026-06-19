using MiraAPI.Example.Options.Modifiers;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.Modifiers.Types;

using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace MiraAPI.Example.Modifiers;

public class CaptainModifier : GameModifier
{
    public override string ModifierName => "modifier.captain.name";
    public override LoadableAsset<Sprite>? ModifierIcon => ExampleAssets.CallMeetingButton;

    public override void OnDeath(DeathReason reason)
    {
        Player.RemoveModifier(this);
    }

    public override string GetDescription()
    {
        return "modifier.captain.description";
    }

    public override int GetAssignmentChance()
    {
        return (int)OptionGroupSingleton<CaptainModifierSettings>.Instance.Chance;
    }

    public override int GetAmountPerGame()
    {
        return (int)OptionGroupSingleton<CaptainModifierSettings>.Instance.Amount;
    }
}
