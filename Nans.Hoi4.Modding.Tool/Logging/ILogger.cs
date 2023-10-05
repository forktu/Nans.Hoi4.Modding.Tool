using System.Threading.Tasks;

namespace Nans.Hoi4.Modding.Tool.Logging
{
    public interface ILogger
    {
        void Open();
        void Close();
        void Log(string message, ELogLevel level);
    }
    public interface IAsyncLogger : ILogger
    {
        Task LogAsync(string message, ELogLevel level);
    }
}
