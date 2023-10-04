using Nans.Hoi4.Modding.Tool.Localization;
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

        public void Save()
        {
            throw new NotImplementedException();
        }

        public void Load()
        {
            throw new NotImplementedException();
        }

        public void LoadDefaults()
        {
            throw new NotImplementedException();
        }
    }
}
