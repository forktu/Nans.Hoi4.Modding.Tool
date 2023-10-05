using System.Collections.Generic;

namespace Nans.Hoi4.Modding.Tool.Localization
{
    public sealed class Localization
    {
        public Localization()
        {
            Name = "Unknown";
            Author = "Unknown";
            General = new TranslationDictionary();
            Interface = new TranslationDictionary();
        }
        public string Name;
        public string Author;
        public TranslationDictionary General;
        public TranslationDictionary Interface;
        public TranslationDictionary GetDictionary(string name)
        {
            switch (name.ToLowerInvariant())
            {
                case "general":
                    return General;
                case "interface":
                    return Interface;
                default:
                    return null;
            }
        }
        public override string ToString()
        {
            return $"Localization {Name}. Author(s): {Author}";
        }

        public void AddMissingKeys(Localization from)
        {
            TranslationDictionary.AddMissingKeys(General, from.General);
            TranslationDictionary.AddMissingKeys(Interface, from.Interface);
            try
            {
            }
            catch { }
        }
    }
}
