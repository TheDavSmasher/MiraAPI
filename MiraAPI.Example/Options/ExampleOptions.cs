using MiraAPI.GameOptions;
using MiraAPI.GameOptions.Attributes;
using MiraAPI.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace MiraAPI.Example.Options;

public class ExampleOptions : AbstractOptionGroup
{
    public override string GroupName => "Example Options 1";
    public override Color GroupColor => Color.green;

    [ModdedToggleOption("Toggle Opt 1")]
    public bool ToggleOpt { get; set; } = false;

    [ModdedToggleOption("Toggle Opt 2")]
    public bool ToggleOpt2 { get; set; } = true;

    [ModdedNumberOption("Number Opt", min: 0, max: 10, increment: .25f, formatString: "0.00", suffixType: MiraNumberSuffixes.Percent)]
    public float NumberOpt { get; set; } = 4f;

    [ModdedEnumOption("Best API", typeof(BestApi), ["Mira API", "Mitochondria", "Reactor"])]
    public BestApi Opt { get; set; } = BestApi.MiraAPI;

    [ModdedEnumOptionList<ValueState>("Value State {0}", ["None", "Primary", "Secondary"])]
    public List<ValueState> ValueStates { get; } =
        [ValueState.Primary, ValueState.Secondary, ValueState.None, ValueState.None, ValueState.None];
}

public enum BestApi
{
    MiraAPI,
    Mitochondria,
    Reactor,
}

public enum ValueState
{
    None,
    Primary,
    Secondary,
}
