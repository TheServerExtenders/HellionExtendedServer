using NLog;

namespace HellionExtendedServer.Common
{
    public class Log
    {
        public static Logger Instance { get; private set; }

        public Log()
        {
            Instance = LogManager.GetCurrentClassLogger();
        }
    }
}