using MiraAPI.Roles;
using MiraAPI.Utilities.Assets;
using UnityEngine;

namespace MiraAPI.Example.Roles;

public class FreezerRole : ImpostorRole, ICustomRole
{
    public string IdPart => "ApiExample.Role.Freezer";
    public Color RoleColor => Palette.Blue;
    public ModdedRoleTeams Team => ModdedRoleTeams.Impostor;

    public CustomRoleConfiguration Configuration => new CustomRoleConfiguration(this)
    {
        Icon = MiraAssets.ImpostorFile,
        IconTmp = TmpSpriteUtils.CreateSpriteAsset(MiraAssets.ImpostorFile.LoadAsset(), "ApiExample.Role.Impostor.FreezerRole"),
        OptionsScreenshot = ExampleAssets.Banner,
        MaxRoleCount = 2,
    };
}
