using HellionExtendedServer.Controllers;
using HellionExtendedServer.Managers;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
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

            Log.Instance.Info("Hellion Extended Server Initialized.");

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

            m_serverInstance = new ServerInstance();
        }

        /// <summary>
        /// HES runs this on load, which starts the server, then starts
        /// the command reading loop which also keeps the console from closing
        /// </summary>
        /// <param name="args"></param>
        private void Run(string[] args)
        {
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
                //This correct bool is used 
                bool correct = false;
                string command = "";

                Match cmd1 = Regex.Match(cmd, @"^(/help)");
                if (cmd1.Success)
                {
                    try
                    {
                        PrintHelp();
                    }
                    catch (ArgumentException) { }
                    correct = true;
                }

                //Differents args for /players command to display the count, the full list of players (disconnected and disconnected) and the list of connected players.
                Match cmd2 = Regex.Match(cmd, @"^(/players)");
                if (cmd2.Success && cmd.Length > 9)
                {
                    correct = true;
                    Console.WriteLine();
                    command = cmd.Substring(9);
                    if (command == "-count")
                        Console.WriteLine("Players Connected: " + ServerInstance.Instance.Server.NetworkController.CurrentOnlinePlayers() + "/" + ServerInstance.Instance.Server.MaxPlayers);

                    if (command == "-list")
                    {
                        Console.WriteLine(string.Format("\t-------Pseudo------- | -------SteamId-------"));
                        foreach (var client in NetworkController.Instance.ClientList)
                        {
                            Console.WriteLine(string.Format("\t {0} \t {1}", client.Value.Player.Name, client.Value.Player.SteamId));
                        }
                    }

                    if (command == "-all")
                    {
                        Console.WriteLine(string.Format("\t-------Pseudo------- | -------SteamId------- | -------Connected-------"));
                        foreach (var player in ServerInstance.Instance.Server.AllPlayers)
                        {
                            Console.WriteLine(string.Format("\t {0} \t {1} \t {2}", player.Name, player.SteamId, NetworkController.Instance.ClientList.Values.Contains(NetworkController.Instance.GetClient(player))));
                        }
                    }
                    Console.WriteLine();
                }

                Match cmd3 = Regex.Match(cmd, @"^(/save)");
                if (cmd3.Success)
                {
                    ServerInstance.Instance.Save();
                    correct = true;
                }

                Match cmd5 = Regex.Match(cmd, @"^(/opengui)");
                if (cmd5.Success)
                {
                    LoadGUI();
                    correct = true;
                }

                // I add it to send a private message to a player (it's really usefull for adminsitrators)
                Match cmd4 = Regex.Match(cmd, @"^(/send)");
                if (cmd4.Success && cmd.Length > 5)
                {
                    correct = true;
                    command = cmd.Substring(5);
                    string target = "";
                    string text = "";
                    if (command.Contains("(") && command.Contains("("))
                    {
                        target = command.Substring(command.IndexOf('(') + 1, command.IndexOf(')') - 2);

                        if (command.Length > command.IndexOf(')') + 1)
                        {
                            text = command.Substring(command.IndexOf(')') + 2);
                        }

                        Console.WriteLine("Server > " + target + " : " + text);
                        NetworkController.Instance.MessageToClient(text, "Server", target);
                    }
                    else
                        Console.WriteLine("No player name specified");
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
                Console.WriteLine("Loading GUI (WIP)");
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
            Log.Instance.Warn("/players -count - returns the current amount of online players");
            Log.Instance.Warn("/players -list - returns the full list of connected players");
            Log.Instance.Warn("/save - forces a universe save");
            Log.Instance.Warn("/send (name) text - send a message to the specified player");
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
                Log.Instance.Info("SHUTTING DOWN SERVER");

                Server.IsRunning = false;
                if (Server.PersistenceSaveInterval > 0.0)
                    Server.SavePersistenceDataOnShutdown = true;
                Server.MainLoopEnded.WaitOne(5000);
                Console.WriteLine("CLOSING HELLION EXTENDED SERVER");
            }
            return false;
        }

        #endregion ConsoleHandler
    }
}