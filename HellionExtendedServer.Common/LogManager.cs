using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NLog;
using NLog.Config;
using NLog.Targets;


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
