using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using MiraAPI.Networking;
using MiraAPI.PluginLoading;
using Reactor.Localization.Utilities;
using Reactor.Networking.Rpc;
using Reactor.Utilities;
using UnityEngine;

namespace MiraAPI.GameOptions.OptionTypes;

/// <summary>
/// Represents a modded option list.
/// </summary>
public abstract class ModdedOptionList : IModdedOptionList
{
    protected IMiraPlugin? _parentMod;

    /// <inheritdoc/>
    public int Count { get; }

    /// <inheritdoc />
    public IReadOnlyList<uint> Ids { get; }

    /// <inheritdoc />
    public string Title { get; set; }

    /// <inheritdoc/>
    public bool ZeroIndexTitle { get; set; }

    /// <inheritdoc />
    public IReadOnlyList<StringNames> StringNames { get; }

    /// <inheritdoc />
    public IReadOnlyList<BaseGameSetting> Data { get; protected set; } = [];

    /// <inheritdoc />
    public IMiraPlugin? ParentMod
    {
        get => _parentMod;
        set
        {
            if (_parentMod != null || value == null) return;
            _parentMod = value;

            OnParentModChange();
        }
    }

    protected abstract void OnParentModChange();

    /// <inheritdoc />
    public Func<int, bool> Visible { get; set; }

    /// <inheritdoc />
    public bool IncludeInPreset { get; set; }

    protected readonly OptionBehaviour?[] _optionBehaviours;

    /// <inheritdoc />
    public IReadOnlyList<OptionBehaviour?> OptionBehaviours => _optionBehaviours;

    /// <inheritdoc />
    public ConfigDefinition?[] ConfigDefinitions { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModdedOptionList"/> class.
    /// </summary>
    /// <param name="title">The options' title.</param>
    /// <param name="count">The option list's length.</param>
    /// <param name="includeInPreset">Whether to include the options in the preset.</param>
    /// <param name="zeroIndexTitle">Whether the first option's title index is 0, else 1.</param>
    protected ModdedOptionList(string title, int count, bool includeInPreset = true, bool zeroIndexTitle = false)
    {
        Count = count;
        ZeroIndexTitle = zeroIndexTitle;

        var range = Enumerable.Range(0, Count);
        var titleOffset = ZeroIndexTitle ? 0 : 1;

        Ids = range.Select(_ => ModdedOptionsManager.NextId).ToList();
        Title = title;
        StringNames = range.Select(i => CustomStringName.CreateAndRegister($"{Title}{i + titleOffset}")).ToList();
        Visible = _ => true;
        IncludeInPreset = includeInPreset;

        _optionBehaviours = range.Select<int, OptionBehaviour?>(_ => null).ToArray();
        ConfigDefinitions = range.Select<int, ConfigDefinition?>(_ => null).ToArray();
    }

    /// <inheritdoc/>
    public abstract void SaveToPreset(ConfigFile presetConfig, bool saveDefault = false);

    /// <inheritdoc/>
    public abstract void Bind(ConfigFile config);

    /// <inheritdoc/>
    public abstract void LoadFromPreset(ConfigFile presetConfig);

    /// <inheritdoc/>
    public abstract float GetFloatData(int idx);

    /// <inheritdoc/>
    public abstract NetData GetNetData(int idx);

    /// <inheritdoc/>
    public abstract void HandleNetData(int idx, byte[] data);

    /// <inheritdoc/>
    public abstract OptionBehaviour CreateOption(
        int idx,
        ToggleOption toggleOpt,
        NumberOption numberOpt,
        StringOption stringOpt,
        PlayerOption playerOpt,
        Transform container);
}
