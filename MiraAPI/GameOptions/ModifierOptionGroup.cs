using System.Linq;
using MiraAPI.PluginLoading;

namespace MiraAPI.GameOptions;

#pragma warning disable CA1852
[MiraIgnore]
internal class ModifierOptionGroup : AbstractOptionGroup
#pragma warning restore CA1852
{
    public override string GroupName { get; }

    public ModifierOptionGroup(string name, IModdedOption[] options, params AbstractOptionGroup[] groups)
    {
        GroupName = name;
        Options.AddRange(options);
        Options.AddRange(groups.SelectMany(x=>x.Options));
    }
}
