using System.Collections.Generic;
using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem;
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
        }
    }

    internal static StringOption OptionBehaviour { get; private set; } = null!;

    private static readonly List<string> Queue = [];
    // loading takes place before option creation
    internal static void AddOption(string opt)
    {
        if (OptionBehaviour == null)
        {
            Queue.Add(opt);
            return;
        }
        OptionBehaviour.Values = (Il2CppStructArray<StringNames>)OptionBehaviour.Values.Add(CustomStringName.CreateAndRegister(opt));
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
        setting.Index = 0;
        setting.Values = new Il2CppStructArray<StringNames>([CustomStringName.CreateAndRegister("Default")]);
        OptionBehaviour.SetUpFromData(setting, 20);
        OptionBehaviour.TitleText.fontSize = 3;
        OptionBehaviour.OnValueChanged = (Action<OptionBehaviour>) ((OptionBehaviour opt) =>
        {
            CustomGameModeManager.SetGameMode((uint)opt.GetInt());
        });
        num -= 0.37f; // scrollbar offset
        __instance.Children.Add(OptionBehaviour);
        __instance.scrollBar.SetYBoundsMax(-num - 1.65f);
        foreach (var str in Queue)
            AddOption(str);
        Queue.Clear();
    }

    [HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.ValueChanged))]
    [HarmonyPrefix]
    private static bool ValueChanged(GameOptionsMenu __instance, OptionBehaviour option)
    {
        return !OptionBehaviour.Equals(option);
    }
}
