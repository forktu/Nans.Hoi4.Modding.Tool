﻿using Nans.Hoi4.Modding.Tool.Localization;
using Nans.Hoi4.Modding.Tool.Logging;
using Nans.Hoi4.Modding.Tool.Themes;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Nans.Hoi4.Modding.Tool.Configuration
{
    public class AppConfig : IConfig
    {
        public static AppConfig Instance { get; private set; } = new AppConfig();

        public bool experimentalFeatures;
        public double scale;
        public ELanguage language;

        public void Apply(AppConfig from, out bool hasToRestart)
    }
}