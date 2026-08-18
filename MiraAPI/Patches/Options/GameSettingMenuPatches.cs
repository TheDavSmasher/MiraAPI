using System;
using System.Collections.Generic;
using HarmonyLib;
using MiraAPI.GameOptions;
using MiraAPI.Modifiers;
using MiraAPI.PluginLoading;
using MiraAPI.Presets;
using MiraAPI.Utilities.Assets;
using Reactor.Utilities.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.UI.Button;
using Object = UnityEngine.Object;

namespace MiraAPI.Patches.Options;

[HarmonyPatch(typeof(GameSettingMenu))]
internal static class GameSettingMenuPatches
{
    /// <summary>
    /// Prefix for the <see cref="GameSettingMenu.OnEnable"/> method. Sets up the custom options.
    /// </summary>
    /// <param name="__instance">The <see cref="GameSettingMenu"/> instance.</param>
    [HarmonyPrefix]
    [HarmonyPatch(nameof(GameSettingMenu.OnEnable))]
    public static void OnEnablePrefix(GameSettingMenu __instance)
    {
        // Add Menu State object
        if (!__instance.gameObject.GetComponent<MenuState>())
        {
            __instance.gameObject.AddComponent<MenuState>();
            Info("MenuState component added to GameSettingMenu.");
        }
        else
        {
            Info("MenuState component already added to GameSettingMenu.");
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(GameSettingMenu.ChangeTab))]
    public static bool ChangeTabPrefix(GameSettingMenu __instance, int tabNum, bool previewOnly)
    {
        if (MenuState.Instance)
        {
            MenuState.Instance.ChangeTabPatch(__instance, tabNum, previewOnly);
        }
        else
        {
            Error("MenuState instance is null. Cannot change tab.");
        }
        return false;
    }
}
