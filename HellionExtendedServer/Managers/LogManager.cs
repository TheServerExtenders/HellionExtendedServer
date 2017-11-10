using NLog;

namespace HellionExtendedServer
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