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

    protected readonly BaseGameSetting[] _data;

    /// <inheritdoc />
    public IReadOnlyList<BaseGameSetting> Data => _data;

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

/// <summary>
/// Represents a modded option list.
/// </summary>
/// <typeparam name="T">The value type.</typeparam>
public abstract class ModdedOptionList<T> : ModdedOptionList
{
    protected override void OnParentModChange()
    {
        var configFile = _parentMod!.GetConfigFile();
        for (int i = 0; i < Count; i++)
        {
            var entry = configFile.Bind(ConfigDefinitions[i], DefaultValue(i));
            Values[i] = entry.Value;
        }
    }

    /// <summary>
    /// Gets the list of values of the options.
    /// </summary>
    public T[] Values { get; }

    /// <summary>
    /// Gets the list of default value of the options.
    /// </summary>
    public Func<int, T> DefaultValue { get; }

    /// <summary>
    /// Gets or sets the event that is invoked when the value of an option changes.
    /// </summary>
    public Action<int, T>? ChangedEvent { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModdedOptionList{T}"/> class.
    /// </summary>
    /// <param name="title">The options' title.</param>
    /// <param name="count">The option list's length.</param>
    /// <param name="defaultValues">The default values.</param>
    /// <param name="includeInPreset">Whether to include the options in the preset.</param>
    /// <param name="zeroIndexTitle">Whether the first option's title index is 0, else 1.</param>
    protected ModdedOptionList(string title, int count, Func<int, T> defaultValues, bool includeInPreset = true, bool zeroIndexTitle = false)
        : base(title, count, includeInPreset, zeroIndexTitle)
    {
        DefaultValue = defaultValues;
        Values = Enumerable.Range(0, Count).Select(defaultValues).ToArray();
    }

    internal void ValueChanged(int idx, OptionBehaviour optionBehaviour)
    {
        SetValue(idx, GetValueFromOptionBehaviour(idx, optionBehaviour));
    }

    /// <summary>
    /// Sets the value of the option.
    /// </summary>
    /// <param name="idx">The option's index.</param>
    /// <param name="newValue">The new value.</param>
    /// <param name="sendRpc">Whether to send the value to other players.</param>
    public void SetValue(int idx, T newValue, bool sendRpc = true)
    {
        var oldVal = Values[idx];
        Values[idx] = newValue;

        if (Values[idx]?.Equals(oldVal) == false)
        {
            ChangedEvent?.Invoke(idx, Values[idx]);
        }

        if (sendRpc && AmongUsClient.Instance.AmHost)
        {
            if (ParentMod?.GetConfigFile().TryGetEntry<T>(ConfigDefinitions[idx], out var entry) == true)
            {
                entry.Value = Values[idx];
            }

            Rpc<SyncOptionsRpc>.Instance.Send(PlayerControl.LocalPlayer, [GetNetData(idx)], true); // This might not work
        }

        OnValueChanged(idx, newValue);
    }

    /// <inheritdoc/>
    public override void SaveToPreset(ConfigFile presetConfig, bool saveDefault = false)
    {
        if (ConfigDefinitions.Any(d => d is null))
        {
            Error($"Attempted to save {Title} to preset, but some ConfigDefinitions are null.");
            return;
        }
        Bind(presetConfig);
        for (int i = 0; i < Count; i++)
        {
            presetConfig[ConfigDefinitions[i]].BoxedValue = saveDefault ? DefaultValue(i) : Values[i];
        }
    }

    /// <inheritdoc />
    public override void Bind(ConfigFile config)
    {
        for (int i = 0; i < Count; i++)
        {
            config.Bind(ConfigDefinitions[i], DefaultValue(i));
        }
    }

    /// <inheritdoc />
    public override void LoadFromPreset(ConfigFile presetConfig)
    {
        for (int i = 0; i < Count; i++)
        {
            if (presetConfig.TryGetEntry(ConfigDefinitions[i], out ConfigEntry<T> entry))
            {
                SetValue(i, entry.Value, false);
            }
            else
            {
                Error($"Attempted to load {Title} from preset, but ConfigDefinition {i} is not found in preset.");
            }
        }
    }

    /// <summary>
    /// Handles the value changed event.
    /// </summary>
    /// <param name="idx">The option's index.</param>
    /// <param name="newValue">The new value.</param>
    protected abstract void OnValueChanged(int idx, T newValue);

    /// <summary>
    /// Gets the value from the option behaviour.
    /// </summary>
    /// <param name="idx">The option's index.</param>
    /// <param name="optionBehaviour">The OptionBehaviour.</param>
    /// <returns>The value.</returns>
    public abstract T GetValueFromOptionBehaviour(int idx, OptionBehaviour optionBehaviour);

    /// <summary>
    /// Indexes the option for its value of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="idx">The option's index.</param>
    /// <returns>value of type <typeparamref name="T"/>.</returns>
    public T this[int idx] => Values[idx];
}

/// <summary>
/// Represents a modded option list.
/// </summary>
/// <typeparam name="T">The option's type.</typeparam>
public class ModdedOptionsList<T> : ModdedOptionList where T : IModdedOption
{
    protected override void OnParentModChange()
    {
        foreach (var option in Options)
        {
            option.ParentMod = _parentMod!;
        }
    }

    /// <summary>
    /// Gets the list of options.
    /// </summary>
    public T[] Options { get; }

    /// <summary>
    /// Gets the default option from its index.
    /// </summary>
    public Func<int, T> DefaultOption { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModdedOptionsList{T}"/> class.
    /// </summary>
    /// <param name="title">The options' title.</param>
    /// <param name="count">The option list's length.</param>
    /// <param name="defaultOption">The default option.</param>
    /// <param name="includeInPreset">Whether to include the options in the preset.</param>
    /// <param name="zeroIndexTitle">Whether the first option's title index is 0, else 1.</param>
    protected ModdedOptionsList(string title, int count, Func<int, T> defaultOption, bool includeInPreset = true, bool zeroIndexTitle = false)
        : base(title, count, includeInPreset, zeroIndexTitle)
    {
        DefaultOption = defaultOption;
        Options = Enumerable.Range(0, Count).Select(defaultOption).ToArray();
    }

    /// <inheritdoc/>
    public override void SaveToPreset(ConfigFile presetConfig, bool saveDefault = false)
    {
        foreach (var option in Options)
        {
            option.SaveToPreset(presetConfig, saveDefault);
        }
    }

    /// <inheritdoc/>
    public override void Bind(ConfigFile config)
    {
        foreach (var option in Options)
        {
            option.Bind(config);
        }
    }

    /// <inheritdoc/>
    public override void LoadFromPreset(ConfigFile presetConfig)
    {
        foreach (var option in Options)
        {
            option.LoadFromPreset(presetConfig);
        }
    }

    /// <inheritdoc/>
    public override float GetFloatData(int idx)
    {
        return Options[idx].GetFloatData();
    }

    /// <inheritdoc/>
    public override NetData GetNetData(int idx)
    {
        return Options[idx].GetNetData();
    }

    /// <inheritdoc/>
    public override void HandleNetData(int idx, byte[] data)
    {
        Options[idx].HandleNetData(data);
    }

    /// <inheritdoc/>
    public override OptionBehaviour CreateOption(int idx, ToggleOption toggleOpt, NumberOption numberOpt, StringOption stringOpt, PlayerOption playerOpt, Transform container)
    {
        return _optionBehaviours[idx] = Options[idx].CreateOption(toggleOpt, numberOpt, stringOpt, playerOpt, container);
    }

    /// <summary>
    /// Indexes the option of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="idx">The option's index.</param>
    /// <returns>option of type <typeparamref name="T"/>.</returns>
    public T this[int idx] => Options[idx];
}
