using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Windows;

namespace Nans.Hoi4.Modding.Tool.Localization
{
    public static class LocalizationManager
    {
        private const string SRC_PATH = @"D:\Documents\Codeing_Folder\Visual_Studios_Files\Nans.Hoi4.Modding.Tool-1.0";

        private static Localization _curLoc = new Localization();

        public static Localization Current
        {
            get
            {
                if ((bool)DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue)
                {
                    {
                        using (StreamReader sr = new StreamReader(Path.Combine(SRC_PATH, "Resources", "Localization", "English.json")))
                        {
                            string text = sr.ReadToEnd();
                        }
                    }
                }

                return _curLoc;
            }
            private set
            {
                _curLoc = value;
            }

        }
    }
}