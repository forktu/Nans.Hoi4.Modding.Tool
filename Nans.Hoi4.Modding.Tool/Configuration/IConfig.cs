namespace Nans.Hoi4.Modding.Tool.Configuration
{
    public interface IConfig
    {
        void Save();
        void Load();
        void LoadDefaults();
    }
}