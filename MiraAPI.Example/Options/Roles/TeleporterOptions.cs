using MiraAPI.Example.Roles;
using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.GameOptions.OptionTypes;
using MiraAPI.Utilities;

namespace MiraAPI.Example.Options.Roles;

public class TeleporterOptions : AbstractRoleOptionGroup<TeleporterRole>
{
    public override string GroupName => "options.teleporterOptions.name";

    public ModdedNumberOption TeleportCooldown { get; set; } = new("options.teleporterOptions.teleportCooldown", 10, 5, 60, 2.5f, MiraNumberSuffixes.Seconds);

    [ModdedNumberOption("options.teleporterOptions.teleportDuration", 5, 25, 1, MiraNumberSuffixes.Seconds)]
    public float TeleportDuration { get; set; } = 10;

    [ModdedNumberOption("options.teleporterOptions.zoomDistance", 4, 15)]
    public float ZoomDistance { get; set; } = 6;
}
