using MiraAPI.Example.Roles;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using TMPro;
using UnityEngine;

namespace MiraAPI.Example.Options.Roles;

public class FreezerRoleSettings : AbstractOptionGroup<FreezerRole>
{
    public override OptionNotifConfiguration Configuration => new(
        Palette.Blue,
        TmpSpriteUtils.CreateSpriteAsset(
            MiraAssets.ImpostorFile.LoadAsset(),
            "ApiExample.Role.Impostor.FreezerRole"));
    public override string GroupName => "Custom Role";

    [ModdedNumberOption("Freeze Duration", 1, 15, 1, MiraNumberSuffixes.Seconds)]
    public float FreezeDuration { get; set; } = 5;

    [ModdedNumberOption("Freeze Uses", 1, 5)]
    public float FreezeUses { get; set; } = 1;
}
