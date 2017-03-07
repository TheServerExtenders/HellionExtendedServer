using HellionExtendedServer.Controllers;
using HellionExtendedServer.Managers;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using ZeroGravity;

namespace HellionExtendedServer
{
    public class HES
    {
        #region Fields

        private static HES m_instance;
        private static Form1 m_form;
        private static ServerInstance m_serverInstance;
        private static HES.EventHandler _handler;

        #endregion Fields

        #region Properties

        public static String BuildBranch { get { return "Master Branch"; } }
        public static Version Version { get { return Assembly.GetEntryAssembly().GetName().Version; } }
        public static String VersionString { get { return Version.ToString(3) + " " + BuildBranch; } }
        public static HES Instance { get { return m_instance; } }
        public static Server CurrentServer { get { return m_serverInstance.Server; } }

        #endregion Properties

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main(string[] args)
        {
                  
            // Setup the handler for closing HES properly and saving
            HES._handler += new HES.EventHandler(HES.Handler);
            HES.SetConsoleCtrlHandler(HES._handler, true);

            //SetupGUI();
            Console.Title = String.Format("HELLION EXTENDED SERVER V{0}) - Game Patch Version: {1} ", Version, "0.1.5");
          
            Log.Instance.Info("Hellion Extended Server v" + Version + " Initialized.");

            SetupGUI();

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
                        Console.WriteLine("Players Connected: " + ServerInstance.Instance.Server.NetworkController.CurrentOnlinePlayers() + "/" + ServerInstance.Instance.Server.MaxPlayers);
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
                        Console.WriteLine(string.Format("{0} players already played in the server since its launching.", ServerInstance.Instance.Server.AllPlayers.Count));
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
                    ServerInstance.Instance.Save();
                    correct = true;

                    if (args.Count > 2 && args[3] == "-show")
                    {
                        NetworkController.Instance.MessageAllClients("Dedicated Server is saving the game...");
                    }
                }

                if (args[1] == "opengui")
                {
                    LoadGUI();
                    correct = true;
                }

                if (args[1] == "send" && args.Count > 2)
                {
                    correct = true;
                    string text = "";
                    if (args.Count > 3 && args[3].Contains("(") && args[3].Contains(")"))
                    {
                        foreach (var arg in args)
                        {
                            if (args.IndexOf(arg) > 2)
                                text += arg + " ";
                        }

                        if (NetworkController.Instance.PlayerConnected(args[3]))
                        {
                            Console.WriteLine("Server > " + args[3] + " : " + text);
                            NetworkController.Instance.MessageToClient(text, "Server", args[3]);
                        }
                        else
                            Console.WriteLine("This player is not connected");
                    }
                    else
                        Console.WriteLine("No player name specified");
                }

                if (args[1] == "kick" && args.Count > 2)
                {
                    correct = true;
                    if (args[3].Contains("(") && args[3].Contains(")"))
                    {
                        ZeroGravity.Objects.Player ply = null;

                        if (NetworkController.Instance.PlayerConnected(args[3], out ply))
                        {
                            try
                            {
                                ply.DiconnectFromNetworkContoller();
                                Console.WriteLine(string.Format("{0} was kicked from the server.", ply.Name));
                            }
                            catch (Exception ex)
                            {
                                Log.Instance.Error("Hellion Extended Server [KICK ERROR] " + ex.ToString());
                            }
                        }
                        else
                        {
                            Console.WriteLine(args[3] + " is not connected");
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
                    Console.WriteLine("Bas synthax ! Use /help to watch all valid commands");
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
                Console.WriteLine("(WIP)Loading GUI...");
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
            Log.Instance.Warn("-------------------------HELP--------------------------------");
            Log.Instance.Warn("Type directly into the console to chat with online players");
            Log.Instance.Warn("Current commands are;" + Environment.NewLine);
            Log.Instance.Warn("/help - this page ;)");
            Log.Instance.Warn("/save - forces a universe save");
            Log.Instance.Warn("/start - start the server");
            Log.Instance.Warn("/stop - stop the server");
            Log.Instance.Warn("/opengui - open the gui");
            Log.Instance.Warn("/players -count - returns the current amount of online players");
            Log.Instance.Warn("/players -list - returns the full list of connected players");
            Log.Instance.Warn("/players -all - returns every player that has ever been on the server. And if they're online.");
            Log.Instance.Warn("/send (name) text - send a message to the specified player");
            Log.Instance.Warn("/kick (name) - kick the specified player from the server");
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