using AmongUs.GameOptions;
using MiraAPI.Patches.Stubs;
using MiraAPI.Roles;
using Reactor.Utilities;
using TMPro;
using UnityEngine;

namespace MiraAPI.Example.Roles;

public class ChameloenRole : CrewmateRole, ICustomRole
{
    public string RoleName => "Chamelon";
    public string RoleLongDescription => "Stay invisible while not moving.";
    public string RoleDescription => RoleLongDescription;
    public Color RoleColor => Palette.AcceptedGreen;
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;

    private bool _shouldHide;

    public CustomRoleConfiguration Configuration => new(this)
    {
        OptionsScreenshot = ExampleAssets.Banner,
        IntroSound = CustomRoleUtils.GetIntroSound(RoleTypes.Shapeshifter),
    };

    public override void Initialize(PlayerControl player)
    {
        Logger<ExamplePlugin>.Info("Initializing ChamelonRole for player: " + player.PlayerId);
        RoleBehaviourStubs.Initialize(this, player);
        _shouldHide = true;
    }

    public void FixedUpdate()
    {
        if (!Player || !_shouldHide)
        {
            return;
        }

        if (Player.MyPhysics.Velocity.magnitude > 0)
        {
            var rend = Player.cosmetics.currentBodySprite.BodySprite;
            var tmp = Player.cosmetics.nameText;
            tmp.color = Color.Lerp(tmp.color, new Color(tmp.color.r, tmp.color.g, tmp.color.b, 1), Time.deltaTime * 4f);
            rend.color = Color.Lerp(rend.color, new Color(1, 1, 1, 1), Time.deltaTime * 4f);

            foreach (var cosmetic in Player.cosmetics.transform.GetComponentsInChildren<SpriteRenderer>())
            {
                cosmetic.color = Color.Lerp(cosmetic.color, new Color(1, 1, 1, 1), Time.deltaTime * 4f);
            }
        }
        else
        {
            SpriteRenderer rend = Player.cosmetics.currentBodySprite.BodySprite;
            TextMeshPro tmp = Player.cosmetics.nameText;
            tmp.color = Color.Lerp(tmp.color, new Color(tmp.color.r, tmp.color.g, tmp.color.b, Player.AmOwner ? 0.3f : 0), Time.deltaTime * 4f);
            rend.color = Color.Lerp(rend.color, new Color(1, 1, 1, Player.AmOwner ? 0.3f : 0), Time.deltaTime * 4f);

            foreach (var cosmetic in Player.cosmetics.transform.GetComponentsInChildren<SpriteRenderer>())
            {
                cosmetic.color = Color.Lerp(cosmetic.color, new Color(1, 1, 1, Player.AmOwner ? 0.3f : 0), Time.deltaTime * 4f);
            }
        }
    }

    public override void Deinitialize(PlayerControl targetPlayer)
    {
        Logger<ExamplePlugin>.Info("Deinitializing ChamelonRole for player: " + targetPlayer.PlayerId);
        RoleBehaviourStubs.Deinitialize(this, targetPlayer);
        _shouldHide = false;
        foreach (var cosmetic in Player.cosmetics.transform.GetComponentsInChildren<SpriteRenderer>(true))
        {
            cosmetic.color = Color.white;
        }
        Player.cosmetics.currentBodySprite.BodySprite.color = Color.white;
        Player.cosmetics.nameText.color = Color.white;
    }
}
