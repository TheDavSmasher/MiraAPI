using AmongUs.GameOptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using MiraAPI.Modules;
using MiraAPI.Networking;
using MiraAPI.Utilities;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MiraAPI.GameOptions.OptionTypes;

/// <summary>
/// An option for selecting an enum value.
/// </summary>
/// <typeparam name="T">The enum type.</typeparam>
public class ModdedMultiSelectOption<T> : ModdedOption<T> where T : struct, Enum
{
    /// <summary>
    /// Gets the string values of the enum.
    /// </summary>
    public string[]? Values { get; }

    /// <summary>
    /// Gets a value indicating "all of the above".
    /// </summary>
    public T All { get; }

    /// <summary>
    /// Gets a value indicating "none of the above".
    /// </summary>
    public T None { get; }

    /// <summary>
    /// Gets the list of buttons that relate to the current instance.
    /// </summary>
    public List<MonoBehaviour> Buttons { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModdedMultiSelectOption{T}"/> class.
    /// </summary>
    /// <param name="title">The title of the option.</param>
    /// <param name="defaultValue">The default value as an int.</param>
    /// <param name="all">The all value.</param>
    /// <param name="none">The none value.</param>
    /// <param name="values">An option list of string values to use in place of the enum name.</param>
    /// <exception cref="ArgumentException">The enum type doesn't have the flags attribute.</exception>
    public ModdedMultiSelectOption(string title, T defaultValue, T all, T none, string[]? values = null) : base(title, defaultValue)
    {
        var tType = typeof(T);

        if (tType.GetCustomAttribute<FlagsAttribute>() == null)
            throw new ArgumentException($"{tType.Name} is not a flag enum type");

        Values = values ?? Enum.GetNames<T>();
        All = all;
        None = none;
        Data = ScriptableObject.CreateInstance<MultiSelectGameSetting>();
        Buttons = [];

        var data = (MultiSelectGameSetting)Data;
        data.Title = StringName;
        data.OptionName = Int32OptionNames.Invalid;
        data.Type = global::OptionTypes.MultipleChoice; // How nice of the game to provide an enum value themselves
        data.EnumNames = []; // We don't initialise the translation here, we just add one as and when needed
    }

    /// <inheritdoc />
    public override OptionBehaviour CreateOption(
        ToggleOption toggleOpt,
        NumberOption numberOpt,
        StringOption stringOpt,
        PlayerOption playerOpt,
        Transform container)
    {
        var stringOption = Object.Instantiate(stringOpt, container);

        stringOption.SetUpFromData(Data, 20);
        stringOption.OnValueChanged = (Action<OptionBehaviour>)ValueChanged;

        var toggle = Object.Instantiate(toggleOpt.GetComponentInChildren<PassiveButton>(), stringOption.transform);
        toggle.name = "Button";
        toggle.transform.DestroyChildren();
        toggle.OverrideOnClickListeners(ToggleButtons);

        var box = toggle.GetComponent<BoxCollider2D>();
        var prevColliderSize = box.size;
        prevColliderSize.x *= 4.97f;
        box.size = prevColliderSize;

        var rend = stringOption.transform.GetChild(5).GetComponent<SpriteRenderer>();
        toggle.OverrideOnMouseOverListeners(() => rend.color = Buttons.Count > 0 ? Color.white : MiraAssets.AcceptedTeal);
        toggle.OverrideOnMouseOutListeners(() => rend.color = Buttons.Count > 0 ? MiraAssets.AcceptedTeal : Color.white);

        // SetUpFromData method doesn't work correctly so we must set the values manually
        stringOption.Title = StringName;
        stringOption.Values = (Data as MultiSelectGameSetting)?.EnumNames?.Values?.ToArray() ?? new Il2CppStructArray<StringNames>(0);
        stringOption.Value = Convert.ToInt32(Value, NumberFormatInfo.InvariantInfo);

        OptionBehaviour = stringOption;

        return stringOption;
    }

    /// <inheritdoc />
    public override void ChangeGameSetting()
    {
        var stringOption = OptionBehaviour as StringOption;

        // The value box itself becomes the button
        stringOption!.PlusBtn.gameObject.SetActive(false);
        stringOption.MinusBtn.gameObject.SetActive(false);

        var background = stringOption.transform.GetChild(0);
        background.localPosition += new Vector3(-0.8f, 0f, 0f);
        background.localScale += new Vector3(1f, 0f, 0f);

        var title = stringOption.TitleText;
        title.transform.localPosition = new(-2.0466f, 0f, -2.9968f);
        title.GetComponent<RectTransform>().sizeDelta = new(5.8f, 0.458f);
        title.fontSize = 2.9f; // Why is it different for string options??

        var valueBox = stringOption.transform.GetChild(5);
        valueBox.localPosition += new Vector3(1.05f, 0f, 0f);
        valueBox.localScale += new Vector3(0.2f, 0f, 0f);
    }

    private void ToggleButtons()
    {
        if (Buttons.Count > 0)
        {
            Buttons.ForEach(x => x.gameObject.Destroy());
            return;
        }

        // TODO: Implement this
    }

    /// <inheritdoc />
    public override float GetFloatData()
    {
        return Convert.ToSingle(Value, NumberFormatInfo.InvariantInfo);
    }

    /// <inheritdoc />
    public override NetData GetNetData()
    {
        return new NetData(Id, EnumSerializer.EnumToBytes(Value));
    }

    /// <inheritdoc />
    public override void HandleNetData(byte[] data)
    {
        SetValue(EnumSerializer.EnumFromBytes<T>(data, 0));
    }

    /// <inheritdoc />
    public override T GetValueFromOptionBehaviour(OptionBehaviour optionBehaviour)
    {
        return Enum.Parse<T>(optionBehaviour.GetInt().ToString(NumberFormatInfo.InvariantInfo));
    }

    /// <inheritdoc />
    protected override void OnValueChanged(T newValue)
    {
        HudManager.Instance.Notifier.AddSettingsChangeMessage(StringName, Data!.GetValueString(Convert.ToInt32(newValue, NumberFormatInfo.InvariantInfo)), false);
        if (!OptionBehaviour)
        {
            return;
        }

        if (OptionBehaviour is StringOption opt)
        {
            opt.Value = Convert.ToInt32(newValue, NumberFormatInfo.InvariantInfo);
        }
    }
}
