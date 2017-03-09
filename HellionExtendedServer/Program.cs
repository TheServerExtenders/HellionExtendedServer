using HellionExtendedServer.Components;
using HellionExtendedServer.Controllers;
using HellionExtendedServer.Managers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ZeroGravity;
using ZeroGravity.Objects;
using static ZeroGravity.Network.NetworkController;

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
        private static EventHandler _handler;

        #endregion Fields

        #region Properties

        public static String BuildBranch { get { return "Master Branch"; } }
        public static Version Version { get { return Assembly.GetEntryAssembly().GetName().Version; } }
        public static String VersionString { get { return Version.ToString(3) + " " + BuildBranch; } }
        public static HES Instance { get { return m_instance; } }
        public static Config Config { get { return m_config; } }
        public static Localization Localization { get { return m_localization; } }
        public static Server CurrentServer { get { return m_serverInstance.Server; } }

        #endregion Properties

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
                  
            // Setup the handler for closing HES properly and saving
            _handler += new EventHandler(Handler);
            SetConsoleCtrlHandler(_handler, true);

            //SetupGUI();
            Console.Title = String.Format("HELLION EXTENDED SERVER V{0}) - Game Patch Version: {1} ", Version, "0.1.5");
          
            Log.Instance.Info("Hellion Extended Server v" + Version + " Initialized.");

            m_config = new Config();
            m_config.Load();
            m_localization = new Localization();
            m_localization.Load(m_config.CurrentLanguage.ToString().Substring(0, 2));

            //SetupGUI();

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

            //ServerInstance.Instance.Start();

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


                if (stringList[1] == "help")
                {
                    try
                    {
                        HES.PrintHelp();
                    }
                    catch (ArgumentException ex)
                    {
                    }
                    flag = true;
                }


                if (stringList[1] == "players" && stringList.Count > 2)
                {
                    if (stringList[2] == "-count")
                    {
                        Console.WriteLine(string.Format(HES.m_localization.Sentences["PlayersConnected"], ServerInstance.Instance.Server.NetworkController.CurrentOnlinePlayers(), ServerInstance.Instance.Server.MaxPlayers));
                        flag = true;
                    }
                    else if (stringList[2] == "-list")
                    {
                        Console.WriteLine(string.Format("\t-------Pseudo------- | -------SteamId-------"));
                        foreach (var client in NetworkController.Instance.ClientList)
                        {
                            Console.WriteLine(string.Format("\t {0} \t {1}", client.Value.Player.Name, client.Value.Player.SteamId));
                        }
                        flag = true;
                    }
                    else if (stringList[2] == "-all")
                    {
                        Console.WriteLine(string.Format(m_localization.Sentences["AllPlayers"], ServerInstance.Instance.Server.AllPlayers.Count));
                        Console.WriteLine(string.Format("\t-------Pseudo------- | -------SteamId------- | -------Connected-------"));
                        foreach (var player in ServerInstance.Instance.Server.AllPlayers)
                        {
                            Console.WriteLine(string.Format("\t {0} \t {1} \t {2}", player.Name, player.SteamId, NetworkController.Instance.ClientList.Values.Contains(NetworkController.Instance.GetClient(player))));
                        }
                        flag = true;
                    }
                    Console.WriteLine();
                }


                if (stringList[1] == "save")
                {
                    if (stringList.Count > 2 && stringList[2] == "-show")
                        ServerInstance.Instance.Save(true);
                    else
                        ServerInstance.Instance.Save(false);
                    flag = true;
                }


                if (stringList[1] == "opengui")
                {
                    HES.LoadGUI();
                    flag = true;
                }


                if (stringList[1] == "msg" && stringList.Count > 2)
                {
                    flag = true;
                    string msg = "";
                    if (stringList.Count > 2 && stringList[2].Contains("(") && stringList[2].Contains(")"))
                    {
                        foreach (string str2 in stringList)
                        {
                            if (stringList.IndexOf(str2) > 2)
                                msg = msg + str2 + " ";
                        }
                        if (NetworkController.Instance.ConnectedPlayer(stringList[3]))
                        {
                            Console.WriteLine("Server > " + stringList[3] + " : " + msg);
                            NetworkController.Instance.MessageToClient(msg, "Server", stringList[3]);
                        }
                        else
                            Console.WriteLine(HES.m_localization.Sentences["PlayerNotConnected"]);
                    }
                    else
                        Console.WriteLine(HES.m_localization.Sentences["NoPlayerName"]);
                }


                if (stringList[1] == "kick" && stringList.Count > 2)
                {
                    flag = true;
                    if (stringList[3].Contains("(") && stringList[3].Contains(")"))
                    {
                        Player player = null;
                        if (NetworkController.Instance.ConnectedPlayer(stringList[3], out player))
                        {
                            try
                            {
                                player.DiconnectFromNetworkContoller();
                                Console.WriteLine(string.Format(HES.m_localization.Sentences["PlayerKicked"], (object)player.Name));
                            }
                            catch (Exception ex)
                            {
                                Log.Instance.Error("Hellion Extended Server [KICK ERROR] : " + ex.ToString());
                            }
                        }
                        else
                            Console.WriteLine(HES.m_localization.Sentences["PlayerNotConnected"]);
                    }
                }

                if (stringList[1] == "start")
                {
                    if (Server.IsRunning)
                        ServerInstance.Instance.Start();
                    flag = true;
                }

                if (stringList[1] == "stop")
                {
                    if ((bool)Server.IsRunning)
                        ServerInstance.Instance.Stop();
                    flag = true;
                }

                if (stringList[1] == "opengui")
                {
                    HES.LoadGUI();
                    flag = true;
                }

                if (!flag)
                    Console.WriteLine(HES.m_localization.Sentences["BadSynthax"]);
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
                Console.WriteLine(HES.m_localization.Sentences["LoadingGUI"]);
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