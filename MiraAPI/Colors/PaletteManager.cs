using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using MiraAPI.Translation;
using Reactor.Localization.Utilities;

namespace MiraAPI.Colors;

/// <summary>
/// Used to register and track <see cref="CustomColor"/>s.
/// </summary>
public static class PaletteManager
{
    /// <summary>
    /// Gets all registered <see cref="CustomColor"/>s.
    /// </summary>
    public static CustomColor[] RegisteredColors => [.. CustomColors];

    internal static readonly List<CustomColor> CustomColors = [];

    internal static void RegisterAllColors()
    {
        var colors = CustomColors.Select(x => x.MainColor).ToArray();
        var shadowColors = CustomColors.Select(x => x.ShadowColor).ToArray();
        var stringNames = CustomColors.Select(x =>
            CustomStringName.CreateAndRegister(TranslationController.Instance.GetString(x.Name).Translate())
        ).ToArray();

        Palette.PlayerColors = Palette.PlayerColors.ToArray().AddRangeToArray(colors);
        Palette.ShadowColors = Palette.ShadowColors.ToArray().AddRangeToArray(shadowColors);
        Palette.ColorNames = Palette.ColorNames.ToArray().AddRangeToArray(stringNames);

        Palette.TextColors = Palette.TextColors.ToArray().AddRangeToArray(colors);
        Palette.TextOutlineColors = Palette.TextOutlineColors.ToArray().AddRangeToArray(shadowColors);
    }
}
