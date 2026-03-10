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

    private static readonly List<String> Queue = [];
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
        float num = -9.5f;
        CategoryHeaderMasked categoryHeaderMasked = Object.Instantiate(__instance.categoryHeaderOrigin, Vector3.zero, Quaternion.identity, __instance.settingsContainer);
        categoryHeaderMasked.SetHeader(CustomStringName.CreateAndRegister("Custom"), 20);
        categoryHeaderMasked.transform.localScale = Vector3.one * 0.63f;
        categoryHeaderMasked.transform.localPosition = new Vector3(-0.903f, num, -2f);
        OptionBehaviour = Object.Instantiate(
            __instance.stringOptionOrigin,
            Vector3.zero,
            Quaternion.identity,
            __instance.settingsContainer);
        OptionBehaviour.transform.localPosition = new Vector3(0.952f, num - 0.63f, -2f);
        OptionBehaviour.SetClickMask(__instance.ButtonClickMask);
        StringGameSetting setting = ScriptableObject.CreateInstance<StringGameSetting>();
        setting.Type = OptionTypes.MultipleChoice;
        setting.Title = CustomStringName.CreateAndRegister("Gamemode");
        setting.Index = 0;
        setting.Values = new Il2CppStructArray<StringNames>([CustomStringName.CreateAndRegister("Default")]);
        OptionBehaviour.SetUpFromData(setting, 20);
        OptionBehaviour.OnValueChanged = (Action<OptionBehaviour>) ((OptionBehaviour opt) =>
        {
            CustomGameModeManager.SetGameMode((uint)opt.GetInt());
        });
        __instance.Children.Add(OptionBehaviour);
        __instance.scrollBar.SetYBoundsMax(9f);
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
