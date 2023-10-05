﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;

namespace Nans.Hoi4.Modding.Tool.Localization
{
    public static class LocalizationManager
    {
        private const string SRC_PATH = @"D:\Documents\Codeing_Folder\Visual_Studios_Files\github\Repos\Nans.Hoi4.Modding.Tool\Nans.Hoi4.Modding.Tool";

        private static Localization _designLoc;
        private static Localization _curLoc = new Localization();

        public static Localization Current
        {
            get
            {
                if ((bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)
                {
                    if (_designLoc == null)
                    {
                        using (StreamReader sr = new StreamReader(Path.Combine(SRC_PATH, "Resources", "Localization", "English.json")))
                        {
                            string text = sr.ReadToEnd();
                            _designLoc = Newtonsoft.Json.JsonConvert.DeserializeObject<Localization>(text);
                        }
                    }

                    return _designLoc;
                }

                return _curLoc;
            }
            private set
            {
                _curLoc = value;
            }

        }

        public static bool IsLoaded { get; private set; } = false;
        public static IEnumerable<ELanguage> SupportedLanguages()
        {
            yield return ELanguage.English;
        }
        public static void LoadLanguage(ELanguage language)
        {
            App.Logger.Log($"[LOCALIZATION] - Loading {language} localization...");
            try
            {
                using (StreamReader sr = new StreamReader(Application.GetResourceStream(new Uri($"Resources/Localization/{language}.json", UriKind.Relative)).Stream))
                {
                    string text = sr.ReadToEnd();
                    Current = Newtonsoft.Json.JsonConvert.DeserializeObject<Localization>(text);
                    IsLoaded = true;
                }
                if (AppConfig.Instance.replaceMissingKeysWithEnglish && language != ELanguage.English)
                {
                    App.Logger.Log("[LOCALIZATION] - Replacing missing keys...");
                    using (StreamReader sr = new StreamReader(Application.GetResourceStream(new Uri($"Resources/Localization/{ELanguage.English}.json", UriKind.Relative)).Stream))
                    {
                        string text = sr.ReadToEnd();
                        var secondary = Newtonsoft.Json.JsonConvert.DeserializeObject<Localization>(text);

                        Current.AddMissingKeys(secondary);
                    }
                }
            }
            catch (Exception ex)
            {
                App.Logger.LogException($"[LOCALIZATION] - Could not load {language} localization.", ex: ex);
            }
        }
        public static string GetLanguageName(ELanguage language)
        {
            switch (language)
            {
                case ELanguage.English:
                    return "English (US)";
                default:
                    return "?";
            }
        }
        public static ELanguage GetLanguageFromCultureInfo(CultureInfo culture)
        {
            switch (culture.Name)
            {
                default:
                    return ELanguage.English;
            }
        }
    }
}
