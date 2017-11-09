﻿using HellionExtendedServer.Common;
using HellionExtendedServer.Managers;
using HellionExtendedServer.Modules;
using NLog;
using NLog.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ZeroGravity;
using ZeroGravity.Objects;
using NetworkManager = HellionExtendedServer.Managers.NetworkManager;

namespace HellionExtendedServer
{
    public class HES
    {
        public static string ForGameVersion = "0.2.5";

        #region Fields

        private static HES m_instance;
        private static Config m_config;
        private static UpdateManager updateManager;
        private static Localization m_localization;
        private static HESGui m_form;
        private static ServerInstance m_serverInstance;
        private static EventHandler _handler;
        private static Boolean m_useGui = true;
        private static Thread uiThread;
        private static Logger mainLogger;
        private static string[] CommandLineArgs;
        private static bool debugMode;

        #endregion Fields

        #region Properties

        public static Version Version => Assembly.GetEntryAssembly().GetName().Version;

        public static String VersionString => Version.ToString(4) + $" Branch: {ThisAssembly.Git.Branch}";

        public static bool Dev
        {
            get
            {
                if (ThisAssembly.Git.Branch.ToLower() == "development") return true;
                return false;
            }
        }

        public static HES Instance => m_instance;

        public static Config Config => m_config;

        public static Localization Localization => m_localization;

        public static Server CurrentServer => m_serverInstance.Server;

        public static HESGui GUI => m_form;

        public static String WindowTitle => String.Format("HELLION EXTENDED SERVER V{0}) - Game Version: {1} - This Game Version {2}", VersionString, ForGameVersion, "0.2.5");

        #endregion Properties

        [STAThread]
        private static void Main(string[] args)
        {
            CommandLineArgs = args;
            Console.Title = WindowTitle;

            m_config = new Config();
            debugMode = m_config.Settings.DebugMode;

            AppDomain.CurrentDomain.AssemblyResolve += (sender, rArgs) =>
            {
                string assemblyName = new AssemblyName(rArgs.Name).Name;
                if (assemblyName.EndsWith(".resources"))
                    return null;

                string dllName = assemblyName + ".dll";
                string dllFullPath = Path.Combine(Path.GetFullPath("Hes\\bin"), dllName);

                if(debugMode)
                    Console.WriteLine($"The assembly '{dllName}' is missing or has been updated. Adding/Updating missing assembly.");

                // get binaries in plugin


                using (Stream s = Assembly.GetCallingAssembly().GetManifestResourceStream("HellionExtendedServer.Resources." + dllName))
                {
                    byte[] data = new byte[s.Length];
                    s.Read(data, 0, data.Length);

                    File.WriteAllBytes(dllFullPath, data);
                }

                return Assembly.LoadFrom(dllFullPath);
            };

            // This is for args that should be used before HES loads
            bool noUpdateHes = false;
            bool noUpdateHellion = false;
            bool usePrereleaseVersions = false;
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (string arg in args)
            {
                if(arg.Equals("-noupdatehes"))
                    noUpdateHes = true;

                if(arg.Equals("-noupdatehellion"))
                    noUpdateHellion = true;

                if (arg.Equals("-usedevversion"))
                    usePrereleaseVersions = true;
            }

            if (usePrereleaseVersions || Config.Settings.EnableDevelopmentVersion)
            {
                Console.WriteLine("HellionExtendedServer: (Arg: -usedevversion is set) HES Will use Pre-releases versions");
            }

            if (noUpdateHes || !Config.Settings.EnableAutomaticUpdates)
            {   
                UpdateManager.EnableAutoUpdates = false;
                Console.WriteLine("HellionExtendedServer: (Arg: -noupdatehes is set or option in HES config is enabled) HES will not be auto-updated.\r\n");
            }

            if (noUpdateHellion || !Config.Settings.EnableHellionAutomaticUpdates )
            {
                SteamCMD.AutoUpdateHellion = false;
                Console.WriteLine("HellionExtendedServer: (Arg: -noupdatehellion is set) Hellion Dedicated will not be auto-updated.");
            }

            Console.ResetColor();

            new FolderStructure().Build();
                               
            updateManager = new UpdateManager();

            var program = new HES(args);
            program.Run(args);
        }

        #region Methods

        private static void SetupGUI()
        {
            if (uiThread != null)
                return;

            uiThread = new Thread(LoadGUI);
            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();
        }

        public HES(string[] args)
        {
            m_instance = this;

            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CrashDump.CurrentDomain_UnhandledException);

                                                  
            string configPath = Globals.GetFilePath(HESFileName.NLogConfig);

            LogManager.Configuration = new XmlLoggingConfiguration(configPath);

            new Log();

            mainLogger = LogManager.GetCurrentClassLogger();

            mainLogger.Info($"Git Branch: {ThisAssembly.Git.Branch}");

            if (debugMode)
            {
                mainLogger.Info($"Git Commit: {ThisAssembly.Git.Commit}");
                mainLogger.Info($"Git SHA: {ThisAssembly.Git.Sha}");
            }

            mainLogger.Info("Hellion Extended Server Initializing....");
        }

        /// <summary>
        /// HES runs this on load, which starts the server, then starts
        /// the command reading loop which also keeps the console from closing
        /// </summary>
        /// <param name="args"></param>
        private void Run(string[] args)
        {          
            m_localization = new Localization();
            m_localization.Load(m_config.Settings.CurrentLanguage.ToString().Substring(0, 2));

            new SteamCMD().GetSteamCMD();


            mainLogger.Info("Hellion Extended Server v" + Version + " Initialized.");


            m_serverInstance = new ServerInstance();

            bool autoStart = Config.Settings.AutoStartEnable;
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (string arg in args)
            {
                if (arg.Equals("-nogui"))
                {
                    m_useGui = false;

                    if (!m_form.Visible)
                        mainLogger.Info("HellionExtendedServer: (Arg: -nogui is set) GUI Disabled, use /showgui to Enable it for this session.");
                }
                autoStart = arg.Equals("-autostart");
            }
            Console.ResetColor();

            if (m_useGui)
                SetupGUI();

            if (autoStart)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("HellionExtendedServer: Arg: -autostart or HESGui's Autostart Checkbox was Checked)");
                Console.ResetColor();
                ServerInstance.Instance.Start();
                
            }
            Console.Title = WindowTitle;

            mainLogger.Info("HellionExtendedServer: Ready! Use /help for commands to use with HES.");
           
            ReadConsoleCommands(args);
        }

        /// <summary>
        /// This contains the console commands
        /// </summary>
        public void ReadConsoleCommands(string[] commandLineArgs)
        {
            while (true)
            {
                string cmd = Console.ReadLine();

                if (cmd.Length > 1)
                {
                    if (!cmd.StartsWith("/"))
                    {
                        if (Server.IsRunning )
                            if(NetworkManager.Instance != null)
                                NetworkManager.Instance.MessageAllClients(cmd);
                        else
                            Console.WriteLine("The Server must be running to message connected clients!");

                        continue;
                    }

                    string cmmd = cmd.Split(" ".ToCharArray())[0].Replace("/", "");
                    string[] args = cmd.Split(" ".ToCharArray()).Skip(1).ToArray();

                    //if (ServerInstance.Instance.CommandManager.HandleConsoleCommand(cmmd, args)) continue;

                    string[] strArray = Regex.Split(cmd, "^/([a-z]+) (\\([a-zA-Z\\(\\)\\[\\]. ]+\\))|([a-zA-Z\\-]+)");
                    List<string> stringList = new List<string>();
                    int num = 1;

                    foreach (string str2 in strArray)
                    {
                        if (str2 != "" && str2 != " ")
                            stringList.Add(str2);
                        ++num;
                    }
                    bool flag = false;

                    if (Server.IsRunning && ServerInstance.Instance.CommandManager != null)
                    {
                        ServerInstance.Instance.CommandManager.HandleConsoleCommand(cmmd, args);
                        flag = true;
                    }

                    if (stringList[1] == "help")
                    {
                        HES.PrintHelp();
                        flag = true;
                    }

                    if (stringList[1] == "checkupdate")
                    {
                        updateManager.CheckForUpdates().GetAwaiter().GetResult();
                        flag = true;
                    }

                    if (stringList[1] == "restart")
                    {
                        Restart();
                        flag = true;
                    }

                    if (stringList[1] == "forceupdate")
                    {
                        updateManager.CheckForUpdates(true).GetAwaiter().GetResult();
                        flag = true;
                    }

                    //Different args for /players command to display the count, the full list of players (disconnected and disconnected) and the list of connected players.
                    if (stringList[1] == "players" && stringList.Count > 2 & Server.IsRunning)
                    {
                        if (stringList[2] == "-count")
                        {
                            Console.WriteLine(string.Format(HES.m_localization.Sentences["PlayersConnected"],
                                ServerInstance.Instance.Server.NetworkController.CurrentOnlinePlayers(),
                                ServerInstance.Instance.Server.MaxPlayers));
                            flag = true;
                        }
                        else if (stringList[2] == "-list")
                        {
                            Console.WriteLine(string.Format("\t-------Pseudo------- | -------SteamId-------"));
                            foreach (var client in NetworkManager.Instance.ClientList)
                            {
                                Console.WriteLine(string.Format("\t {0} \t {1}", client.Value.Player.Name,
                                    client.Value.Player.SteamId));
                            }
                            flag = true;
                        }
                        else if (stringList[2] == "-all")
                        {
                            Console.WriteLine(string.Format(m_localization.Sentences["AllPlayers"],
                                ServerInstance.Instance.Server.AllPlayers.Count));
                            Console.WriteLine(
                                string.Format(
                                    "\t-------Pseudo------- | -------SteamId------- | -------Connected-------"));
                            foreach (var player in ServerInstance.Instance.Server.AllPlayers)
                            {
                                Console.WriteLine(string.Format("\t {0} \t {1} \t {2}", player.Name, player.SteamId,
                                    NetworkManager.Instance.ClientList.Values.Contains(
                                        NetworkManager.Instance.GetClient(player))));
                            }
                            flag = true;
                        }
                        Console.WriteLine();
                    }

                    if (stringList[1] == "save" & Server.IsRunning)
                    {
                        ServerInstance.Instance.Save((stringList.Count > 2 && stringList[2] == "-show"));
                        flag = true;
                    }

                    if (stringList[1] == "msg" && stringList.Count > 2 & Server.IsRunning)
                    {
                        flag = true;
                        var msg = "";
                        if (stringList.Count > 2 && stringList[2].Contains("(") && stringList[2].Contains(")"))
                        {
                            foreach (string str2 in stringList)
                            {
                                if (stringList.IndexOf(str2) > 2)
                                    msg = msg + str2 + " ";
                            }
                            if (NetworkManager.Instance.ConnectedPlayer(stringList[3]))
                            {
                                Console.WriteLine("Server > " + stringList[3] + " : " + msg);
                                NetworkManager.Instance.MessageToClient(msg, "Server", stringList[3]);
                            }
                            else
                                Console.WriteLine(HES.m_localization.Sentences["PlayerNotConnected"]);
                        }
                        else
                            Console.WriteLine(HES.m_localization.Sentences["NoPlayerName"]);
                    }

                    if (stringList[1] == "kick" && stringList.Count > 2 & Server.IsRunning)
                    {
                        flag = true;
                        if (stringList[3].Contains("(") && stringList[3].Contains(")"))
                        {
                            if (NetworkManager.Instance.ConnectedPlayer(stringList[3], out Player player))
                            {
                                try
                                {
                                    player.DiconnectFromNetworkContoller();
                                    Console.WriteLine(string.Format(HES.m_localization.Sentences["PlayerKicked"],
                                        (object)player.Name));
                                }
                                catch (Exception ex)
                                {
                                    Log.Instance.Error(ex, "Hellion Extended Server [KICK ERROR] : " + ex.Message);
                                }
                            }
                            else
                                Console.WriteLine(HES.m_localization.Sentences["PlayerNotConnected"]);
                        }
                    }

                    if (stringList[1] == "start")
                    {
                        if (!Server.IsRunning)
                            ServerInstance.Instance.Start();
                        else
                            Console.WriteLine("The server is already running.");                      
                        flag = true;
                    }

                    if (stringList[1] == "stop")
                    {
                        if (Server.IsRunning)
                            ServerInstance.Instance.Stop();
                        else
                            Console.WriteLine("The server is not running");
                        flag = true;
                    }

                    if (stringList[1] == "opengui")
                    {
                        SetupGUI();
                        flag = true;
                    }

                    if (!flag)
                        Console.WriteLine(HES.m_localization.Sentences["BadSyntax"]);
                }
            }
        }

        internal static void Restart(bool stopServer = true)
        {
            if (Server.IsRunning && stopServer == true)
            {
                if(ServerInstance.Instance != null)
                    ServerInstance.Instance.Stop();

                SpinWait.SpinUntil(() => !ServerInstance.Instance.IsRunning, 2000);
            }

            var thisProcess = Process.GetCurrentProcess();
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = thisProcess.ProcessName;
            startInfo.Arguments = string.Join(" ", CommandLineArgs);
            startInfo.WindowStyle = thisProcess.StartInfo.WindowStyle;

            var proc = Process.Start(startInfo);

            thisProcess.Kill();
        }

        /// <summary>
        /// Loads the gui into its own thread
        /// </summary>
        [STAThread]
        private static void LoadGUI()
        {
            Console.WriteLine(HES.m_localization.Sentences["LoadingGUI"]);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (m_form == null || m_form.IsDisposed)
            {
                m_form = new HESGui();
            }
            else if (m_form.Visible)
                return;

            m_form.Text = WindowTitle + " GUI";

            Application.Run(m_form);
        }

        public static void PrintHelp()
        {
            Log.Instance.Warn("------------------------------------------------------------");
            Log.Instance.Warn(HES.m_localization.Sentences["DescHelp"]);
            Log.Instance.Warn(HES.m_localization.Sentences["HelpCommand"]);
            Log.Instance.Warn(HES.m_localization.Sentences["SaveCommand"]);
            Log.Instance.Warn(HES.m_localization.Sentences["StartCommand"]);
            Log.Instance.Warn(HES.m_localization.Sentences["StopCommand"]);
            Log.Instance.Warn(HES.m_localization.Sentences["OpenGUICommand"]);
            Log.Instance.Warn(HES.m_localization.Sentences["PlayersCommand"]);
            Log.Instance.Warn(HES.m_localization.Sentences["MsgCommand"]);
            Log.Instance.Warn(HES.m_localization.Sentences["KickCommand"]);
            Log.Instance.Warn("-------------------------------------------------------------");
        }

        #endregion Methods

        #region ConsoleHandler

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool add);

        private delegate bool EventHandler(CtrlType sig);

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6,
        }

        private static bool Handler(CtrlType sig)
        {
            if (sig == CtrlType.CTRL_C_EVENT || sig == CtrlType.CTRL_BREAK_EVENT || (sig == CtrlType.CTRL_LOGOFF_EVENT || sig == CtrlType.CTRL_SHUTDOWN_EVENT) || sig == CtrlType.CTRL_CLOSE_EVENT)
            {
                if (Server.IsRunning)
                {
                    ServerInstance.Instance.Stop();
                    Console.WriteLine("CLOSING HELLION EXTENDED SERVER");
                }
            }
            return false;
        }

        #endregion ConsoleHandler
    }
}