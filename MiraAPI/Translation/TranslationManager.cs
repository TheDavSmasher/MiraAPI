using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using Reactor.Utilities;
using UnityEngine;

namespace MiraAPI.Translation;

public static class TranslationManager
{
    private const string LangDirectory = "mira_languages";

    private static readonly Dictionary<string, Dictionary<MiraLanguage, Dictionary<string, string>>> Translations = [];

    /// <summary>
    /// Gets the current language from Among Us settings.
    /// </summary>
    public static MiraLanguage CurrentLanguage
    {
        get
        {
            try
            {
                var langName = AmongUs.Data.DataManager.Settings.Language.CurrentLanguage.ToString();
                return Enum.TryParse<MiraLanguage>(langName, out var result) ? result : MiraLanguage.English;
            }
            catch
            {
                return MiraLanguage.English;
            }
        }
    }

    /// <summary>
    /// Registers translations for a mod. Copies embedded XML to mira_languages/{modGuid}/ on first run,
    /// then loads all XML files from that directory into memory.
    /// Call once per mod during plugin Load().
    /// </summary>
    /// <param name="modGuid">The mod GUID (e.g., "mira.example").</param>
    public static void Register(string modGuid)
    {
        var callingAssembly = Assembly.GetCallingAssembly();
        var dir = GetModLangDir(modGuid);

        // Copy embedded XML resources to disk (first-run only)
        CopyEmbeddedToDisk(callingAssembly, modGuid, dir);

        // Load all XML files from disk into memory
        LoadFromDisk(modGuid, dir);
    }

    /// <summary>
    /// Translates a key into the current language.
    /// Searches all mods, with reverse lookup fallback.
    /// </summary>
    public static string Translate(string key)
    {
        var currentLang = CurrentLanguage;

        foreach (var (_, modTranslations) in Translations)
        {
            if (modTranslations.TryGetValue(currentLang, out var langDict) &&
                langDict.TryGetValue(key, out var translated))
            {
                return translated;
            }
        }

        foreach (var (_, modTranslations) in Translations)
        {
            if (modTranslations.TryGetValue(MiraLanguage.English, out var engDict) &&
                engDict.TryGetValue(key, out var english))
            {
                return english;
            }
        }

        string? bestStructuredKey = null;
        foreach (var (_, modTranslations) in Translations)
        {
            if (modTranslations.TryGetValue(MiraLanguage.English, out var engDict))
            {
                foreach (var (structuredKey, englishValue) in engDict)
                {
                    if (englishValue == key)
                    {
                        bestStructuredKey = structuredKey;
                        break;
                    }
                }
            }

            if (bestStructuredKey != null) break;
        }

        if (bestStructuredKey != null)
        {
            foreach (var (_, modTranslations) in Translations)
            {
                if (modTranslations.TryGetValue(currentLang, out var langDict) &&
                    langDict.TryGetValue(bestStructuredKey, out var revTranslated))
                {
                    return revTranslated;
                }
            }

            foreach (var (_, modTranslations) in Translations)
            {
                if (modTranslations.TryGetValue(MiraLanguage.English, out var engDict) &&
                    engDict.TryGetValue(bestStructuredKey, out var revEnglish))
                {
                    return revEnglish;
                }
            }
        }

        return key;
    }

    private static string GetModLangDir(string modGuid)
    {
        return Path.Combine(Application.persistentDataPath, LangDirectory, modGuid);
    }

    private static void CopyEmbeddedToDisk(Assembly assembly, string modGuid, string dir)
    {
        Directory.CreateDirectory(dir);

        var resourcePrefix = $"{assembly.GetName().Name}.Resources.Translations.";

        foreach (var resourceName in assembly.GetManifestResourceNames())
        {
            if (!resourceName.StartsWith(resourcePrefix)) continue;

            var fileName = resourceName.Substring(resourcePrefix.Length);
            var diskPath = Path.Combine(dir, fileName);

            try
            {
                using var stream = assembly.GetManifestResourceStream(resourceName);
                if (stream == null) continue;

                using var fs = File.Create(diskPath);
                stream.CopyTo(fs);
                Info($"Installed translation: {modGuid}/{fileName}");
            }
            catch (Exception e)
            {
                Warning($"Failed to copy {resourceName}: {e.Message}");
            }
        }
    }

    private static void LoadFromDisk(string modGuid, string dir)
    {
        if (!Directory.Exists(dir)) return;

        if (!Translations.ContainsKey(modGuid))
        {
            Translations[modGuid] = [];
        }

        foreach (var filePath in Directory.GetFiles(dir, "*.xml"))
        {
            var fileName = Path.GetFileNameWithoutExtension(filePath);

            if (!Enum.TryParse<MiraLanguage>(fileName, out var lang)) continue;

            try
            {
                var dict = ParseXmlFile(filePath);
                Translations[modGuid][lang] = dict;
                Info($"Loaded {lang} translation for mod {modGuid} ({dict.Count} keys)");
            }
            catch (Exception e)
            {
                Error($"Failed to load translation {filePath}: {e.Message}");
            }
        }
    }

    private static Dictionary<string, string> ParseXmlFile(string filePath)
    {
        var dict = new Dictionary<string, string>();
        var doc = XDocument.Load(filePath);

        if (doc.Root == null) return dict;

        foreach (var element in doc.Root.Elements("string"))
        {
            var name = element.Attribute("name")?.Value;
            var value = element.Value;

            if (string.IsNullOrEmpty(name)) continue;

            value = value.Replace("<nl>", "\n").Replace("<and>", "&");

            dict[name] = value;
        }

        return dict;
    }
}
