using NLog;
using NLog.Config;
using NLog.Targets;

        /*
        <? xml version="1.0" encoding="utf-8" ?>
        <nlog xmlns = "http://www.nlog-project.org/schemas/NLog.xsd"
             xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance"
             xsi:schemaLocation="http://www.nlog-project.org/schemas/NLog.xsd NLog.xsd"
             autoReload="true"
             throwExceptions="false"
             internalLogLevel="Off" internalLogFile="c:\temp\nlog-internal.log">
         <targets>
           <target name = "console" xsi:type="ColoredConsole"
             layout="${date:format=HH\:mm\:ss}|${message}" />

           <target name = "file" xsi:type="File" fileName="${basedir}/hes/logs/errorlog.txt"
             layout="${date:format=HH\:mm\:ss}|${level}|${stacktrace}|${message}" />

           <target name = "chatlogfile" xsi:type="File" fileName="${basedir}/hes/logs/chatlog.txt"
             layout="${date:format=HH\:mm\:ss}|${message}" />
         </targets>

         <rules>
           <logger name = "*" minlevel="Debug" writeTo="console" />
           <logger name = "*" minlevel="Error" writeTo="file" />
           <logger name = "HellionExtendedServer.Controllers.NetworkController" minlevel="Info" writeTo="chatlogfile" />
         </rules>
       </nlog>
       */

namespace HellionExtendedServer.Common
{
    public class Log
    {
        public static Logger Instance { get; private set; }

        public Log()
        {
            var config = new LoggingConfiguration();

            // targets
            var consoleTarget = new ColoredConsoleTarget();
            config.AddTarget("console", consoleTarget);

            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            var chatFileTarget = new FileTarget();
            config.AddTarget("file", chatFileTarget);

            // target properties
            consoleTarget.Layout = @"${date:format=HH\:mm\:ss}|${message}";

            fileTarget.FileName = @"${basedir}/hes/logs/errorlog.txt";
            fileTarget.Layout = @"${date:format=HH\:mm\:ss}|${level}|${stacktrace}|${message}";

            chatFileTarget.FileName = @"${basedir}/hes/logs/chatlog.txt";
            chatFileTarget.Layout = @"${date:format=HH\:mm\:ss}|${message}";

            // rules
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, consoleTarget));
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Error, fileTarget));
            config.LoggingRules.Add(new LoggingRule("HellionExtendedServer.Controllers.NetworkController", LogLevel.Info, chatFileTarget));

            LogManager.Configuration = config;

            Instance = LogManager.GetCurrentClassLogger();
        }
    }
}