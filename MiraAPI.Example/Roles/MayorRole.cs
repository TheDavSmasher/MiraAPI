using MiraAPI.Roles;
using UnityEngine;

namespace MiraAPI.Example.Roles;

public class MayorRole : CrewmateRole, ICustomRole
{
    public string RoleName => "mayor.name";
    public string RoleLongDescription => "mayor.description";
    public string RoleDescription => RoleLongDescription;
    public Color RoleColor => new Color32(221, 176, 152, 255);
    public ModdedRoleTeams Team => ModdedRoleTeams.Crewmate;
    public CustomRoleConfiguration Configuration => new(this);
}
