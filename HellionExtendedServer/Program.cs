using HellionExtendedServer.Controllers;
using HellionExtendedServer.Managers;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using ZeroGravity;
using HellionExtendedServer.Components;

namespace HellionExtendedServer
{
    public class HES
    {
        #region Fields

        private static HES m_instance;
        private static Config m_config;
        private static Localization m_localization;
        private static Form1 m_form;
        private static ServerInstance m_serverInstance;
        private static HES.EventHandler _handler;

        #endregion Fields

        #region Properties

        public static String BuildBranch { get { return "Master Branch"; } }
        public static Version Version { get { return Assembly.GetEntryAssembly().GetName().Version; } }
        public static String VersionString { get { return Version.ToString(3) + " " + BuildBranch; } }
        public static Config Config { get { return m_config; } }
        public static HES Instance { get { return m_instance; } }
        public static Localization Localization { get { return m_localization; } }
        public static Server CurrentServer { get { return m_serverInstance.Server; } }

        #endregion Properties

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
            //Set the newline characters to jump to the next line
            Console.Out.NewLine = "\n";

            // Setup the handler for closing HES properly and saving
            HES._handler += new HES.EventHandler(HES.Handler);
            HES.SetConsoleCtrlHandler(HES._handler, true);

            Console.Title = String.Format("HELLION EXTENDED SERVER V{0}) - Game Patch Version: {1} ", Version, "0.1.5");
          
            Log.Instance.Info("Hellion Extended Server v" + Version + " Initialized.");

            //SetupGUI();
            m_config = new Config();
            m_config.Load();
            m_localization = new Localization();
            m_localization.Load(m_config.CurrentLanguage.ToString().Substring(0, 2));

            HES program = new HES(args);
            program.Run(args);
        }

        #region Methods

        private static void SetupGUI()
        {
            Thread uiThread = new Thread(LoadGUI);
            uiThread.SetApartmentState(ApartmentState.STA);
            uiThread.Start();
        }

        public HES(string[] args)
        {
            m_instance = this;
        }

        /// <summary>
        /// HES runs this on load, which starts the server, then starts
        /// the command reading loop which also keeps the console from closing
        /// </summary>
        /// <param name="args"></param>
        private void Run(string[] args)
        {
            m_serverInstance = new ServerInstance();
            m_serverInstance.Config = new Components.GameServerIni();
            m_serverInstance.Config.Load();

            foreach(string arg in args)
            {
                if (arg.Contains("-autostart"))
                    ServerInstance.Instance.Start();
            }

            ServerInstance.Instance.Start();

            ReadConsoleCommands();
        }

        /// <summary>
        /// This contains the regex for console commands
        /// </summary>
        public void ReadConsoleCommands()
        {
            string cmd = Console.ReadLine();

            if (!cmd.StartsWith("/"))
            {
                NetworkController.Instance.MessageAllClients(cmd);
            }
            else
            {
                string[] arguments = Regex.Split(cmd, @"^/([a-z]+) (\([a-zA-Z\(\)\[\]. ]+\))|([a-zA-Z\-]+)");
                List<string> args = new List<string>();
                int i = 1;
                foreach (var arg in arguments)
                {
                    if (arg != "" && arg != " ")
                        args.Add(arg);
                    i++;
                }
                arguments = null;
                bool correct = false;

                //Command /help
                if (args[1] == "help")
                {
                    try
                    {
                        PrintHelp();
                    }
                    catch (ArgumentException) { }
                    correct = true;
                }

                //Differents args for /players command to display the count, the full list of players (disconnected and disconnected) and the list of connected players.
                if (args[1] == "players" && args.Count > 2)
                {
                    if (args[2] == "-count")
                    {
                        Console.WriteLine(String.Format(m_localization.Sentences["PlayersConnected"],ServerInstance.Instance.Server.NetworkController.CurrentOnlinePlayers(),ServerInstance.Instance.Server.MaxPlayers));
                        correct = true;
                    }
                    else if (args[2] == "-list")
                    {
                        Console.WriteLine(string.Format("\t-------Pseudo------- | -------SteamId-------"));
                        foreach (var client in NetworkController.Instance.ClientList)
                        {
                            Console.WriteLine(string.Format("\t {0} \t {1}", client.Value.Player.Name, client.Value.Player.SteamId));
                        }
                        correct = true;
                    }
                    else if (args[2] == "-all")
                    {
                        Console.WriteLine(string.Format(m_localization.Sentences["AllPlayers"], ServerInstance.Instance.Server.AllPlayers.Count));
                        Console.WriteLine(string.Format("\t-------Pseudo------- | -------SteamId------- | -------Connected-------"));
                        foreach (var player in ServerInstance.Instance.Server.AllPlayers)
                        {
                            Console.WriteLine(string.Format("\t {0} \t {1} \t {2}", player.Name, player.SteamId, NetworkController.Instance.ClientList.Values.Contains(NetworkController.Instance.GetClient(player))));
                        }
                        correct = true;
                    }

                    Console.WriteLine();
                }

                if (args[1] == "save")
                {
                    if(args.Count > 2 && args[2] == "-show")
                        ServerInstance.Instance.Save(true);
                    else
                        ServerInstance.Instance.Save();
                    correct = true;
                }

                if (args[1] == "opengui")
                {
                    LoadGUI();
                    correct = true;
                }

                if (args[1] == "msg" && args.Count > 2)
                {
                    correct = true;
                    string text = "";
                    if (args.Count >2 && args[2].Contains("(") && args[2].Contains(")"))
                    {
                        foreach (var arg in args)
                        {
                            if (args.IndexOf(arg) > 2)
                                text += arg + " ";
                        }

                        if (NetworkController.Instance.ConnectedPlayer(args[3]))
                        {
                            Console.WriteLine("Server > " + args[3] + " : " + text);
                            NetworkController.Instance.MessageToClient(text, "Server", args[3]);
                        }
                        else
                            Console.WriteLine(m_localization.Sentences["PlayerNotConnected"]);
                    }
                    else
                        Console.WriteLine(m_localization.Sentences["NoPlayerName"]);
                }

                if (args[1] == "kick" && args.Count > 2)
                {
                    correct = true;
                    if (args[3].Contains("(") && args[3].Contains(")"))
                    {
                        ZeroGravity.Objects.Player ply = null;

                        if (NetworkController.Instance.ConnectedPlayer(args[3], out ply))
                        {
                            try
                            {
                                ply.DiconnectFromNetworkContoller();
                                Console.WriteLine(string.Format(m_localization.Sentences["PlayerKicked"], ply.Name));
                            }
                            catch (Exception ex)
                            {
                                Log.Instance.Error("Hellion Extended Server [KICK ERROR] : " + ex.ToString());
                            }
                        }
                        else
                        {
                            Console.WriteLine(m_localization.Sentences["PlayerNotConnected"]);
                        }
                    }
                }

                if (args[1] == "start")
                {
                    if (!Server.IsRunning)
                        ServerInstance.Instance.Start();

                    correct = true;
                }

                if (args[1] == "stop")
                {
                    if (Server.IsRunning)
                        ServerInstance.Instance.Stop();

                    correct = true;
                }

                if (args[1] == "opengui")
                {
                    LoadGUI();
                    correct = true;
                }

                if (!correct)
                {
                    Console.WriteLine(m_localization.Sentences["BadSynthax"]);
                }
            }

            ReadConsoleCommands();
        }

        /// <summary>
        /// Loads the gui into its own thread
        /// </summary>
        [STAThread]
        private static void LoadGUI()
        {
            if (true)
            {
                Console.WriteLine(m_localization.Sentences["LoadingGUI"]);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                if (m_form == null || m_form.IsDisposed)
                {
                    m_form = new Form1();
                }
                else if (m_form.Visible)
                    return;

                Application.Run(m_form);
            }

        }

        public static void PrintHelp()
        {
            Log.Instance.Warn("------------------------------------------------------------");
            Log.Instance.Warn(m_localization.Sentences["DescHelp"]);
            Log.Instance.Warn(m_localization.Sentences["HelpCommand"]);
            Log.Instance.Warn(m_localization.Sentences["SaveCommand"]);
            Log.Instance.Warn(m_localization.Sentences["StartCommand"]);
            Log.Instance.Warn(m_localization.Sentences["StopCommand"]);
            Log.Instance.Warn(m_localization.Sentences["OpenGUICommand"]);
            Log.Instance.Warn(m_localization.Sentences["PlayersCommand"]);
            Log.Instance.Warn(m_localization.Sentences["MsgCommand"]);
            Log.Instance.Warn(m_localization.Sentences["KickCommand"]);
            Log.Instance.Warn("-------------------------------------------------------------");
        }

        #endregion Methods

        #region ConsoleHandler

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(HES.EventHandler handler, bool add);

        private delegate bool EventHandler(HES.CtrlType sig);

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6,
        }

        private static bool Handler(HES.CtrlType sig)
        {
            if (sig == HES.CtrlType.CTRL_C_EVENT || sig == HES.CtrlType.CTRL_BREAK_EVENT || (sig == HES.CtrlType.CTRL_LOGOFF_EVENT || sig == HES.CtrlType.CTRL_SHUTDOWN_EVENT) || sig == HES.CtrlType.CTRL_CLOSE_EVENT)
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