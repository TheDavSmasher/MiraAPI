using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
using MiraAPI.Patches.GameModes;
using Reactor.Localization.Utilities;
using UnityEngine;
using UnityEngine.ProBuilder;
using Object = UnityEngine.Object;

namespace MiraAPI.GameModes;

/// <summary>
/// The game mode option.
/// </summary>
[HarmonyPatch]
public static class GameModeOption
{
    /// <summary>
    /// Gets or Sets the current index of the Game Mode Option
    /// For the value as an AbstractGameMode, see CustomGameModeManager.ActiveMode
    /// </summary>
    public static int Value
    {
        get =>
            OptionBehaviour != null
                ? OptionBehaviour.GetInt()
                : 0;
        set
        {
            if (OptionBehaviour == null)
                return;
            OptionBehaviour.Value = value;
            OptionBehaviour.UpdateValue();
            _lastValue = value;
        }
    }

    private static int _lastValue;
    internal static StringOption OptionBehaviour { get; private set; } = null!;

    private static readonly Dictionary<string, StringNames> Values = new()
    {
        ["Default"] = CustomStringName.CreateAndRegister("Default"),
    };

    // loading takes place before option creation
    internal static void AddOption(string opt)
    {
        if (!Values.ContainsKey(opt))
            Values.Add(opt, CustomStringName.CreateAndRegister(opt));
    }
    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.CreateSettings))]
    [HarmonyPostfix]
    private static void CreateSettingsPatch(GameOptionsMenu __instance)
    {
        if (GameManager.Instance.IsHideAndSeek())
            return;
        float num = 0.713f;
        foreach (RulesCategory rulesCategory in GameManager.Instance.GameSettingsList.AllCategories)
        {
            num -= 0.63f;
            foreach (BaseGameSetting a in rulesCategory.AllGameSettings)
                num -= 0.45f;
        }
        CategoryHeaderMasked categoryHeaderMasked = Object.Instantiate(__instance.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, __instance.settingsContainer);
        categoryHeaderMasked.SetHeader(CustomStringName.CreateAndRegister("Custom"), 20);
        categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
        categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, num, -2f);
        OptionBehaviour = Object.Instantiate(
            __instance.stringOptionOrigin,
            Vector3.zero,
            Quaternion.identity,
            __instance.settingsContainer);
        num -= 0.63f;
        OptionBehaviour.transform.localPosition = new Vector3(0.952f, num, -2f);
        OptionBehaviour.SetClickMask(__instance.ButtonClickMask);
        StringGameSetting setting = ScriptableObject.CreateInstance<StringGameSetting>();
        setting.Type = OptionTypes.MultipleChoice;
        setting.Title = CustomStringName.CreateAndRegister("Gamemode");
        setting.Index = _lastValue;
        setting.Values = new Il2CppStructArray<StringNames>([Values["Default"]]);
        OptionBehaviour.SetUpFromData(setting, 20);
        OptionBehaviour.TitleText.fontSize = 3;
        OptionBehaviour.OnValueChanged = (Action<OptionBehaviour>) ((OptionBehaviour opt) =>
        {
            _lastValue = opt.GetInt();
            CustomGameModeManager.SetGameMode((uint)_lastValue);
            HudPatches.SetGameModeText(Values.ElementAt(_lastValue).Key);
        });
        num -= 0.37f; // scrollbar offset
        __instance.Children.Add(OptionBehaviour);
        __instance.scrollBar.SetYBoundsMax(-num - 1.65f);
        for (var i = 1; i < Values.Count; i++)
            OptionBehaviour.Values = (Il2CppStructArray<StringNames>)OptionBehaviour.Values.Add(Values.ElementAt(i).Value);
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.ValueChanged))]
    [HarmonyPrefix]
    private static bool ValueChanged(GameOptionsMenu __instance, OptionBehaviour option)
    {
        return !OptionBehaviour.Equals(option);
    }
}
