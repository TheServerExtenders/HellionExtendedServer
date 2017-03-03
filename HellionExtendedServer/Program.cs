using System;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

using HellionExtendedServer.Managers;
using HellionExtendedServer.Controllers;

using ZeroGravity;

namespace HellionExtendedServer
{
    public class HES
    {

        private static HES m_instance;
        private static Form1 m_form;
        private static ServerInstance m_serverInstance;
        private static HES.EventHandler _handler;

        public static String BuildBranch { get { return "Master Branch"; } }
        public static Version Version { get { return Assembly.GetEntryAssembly().GetName().Version; } }
        public static String VersionString { get { return Version.ToString(3) + " " + BuildBranch; } }

        public static HES Instance { get { return m_instance; } }

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(HES.EventHandler handler, bool add);
        private delegate bool EventHandler(HES.CtrlType sig);

        private static bool Handler(HES.CtrlType sig)
        {
            if (sig == HES.CtrlType.CTRL_C_EVENT || sig == HES.CtrlType.CTRL_BREAK_EVENT || (sig == HES.CtrlType.CTRL_LOGOFF_EVENT || sig == HES.CtrlType.CTRL_SHUTDOWN_EVENT) || sig == HES.CtrlType.CTRL_CLOSE_EVENT)
            {
                Console.WriteLine("SHUTTING DOWN SERVER");

                    Server.IsRunning = false;
                    if (Server.PersistenceSaveInterval > 0.0)
                        Server.SavePersistenceDataOnShutdown = true;
                    Server.MainLoopEnded.WaitOne(5000);
                Console.WriteLine("CLOSING HELLION EXTENDED SERVER");
            }
            return false;
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            HES._handler += new HES.EventHandler(HES.Handler);
            HES.SetConsoleCtrlHandler(HES._handler, true);

            Thread uiThread = new Thread(LoadGUI);
            uiThread.SetApartmentState(ApartmentState.STA);
            //uiThread.Start();

            Console.Title = String.Format("HELLION EXTENDED SERVER V{0}) - Game Patch Version: {1} ", Version, "0.1.4");

            Console.WriteLine("Hellion Extended Server Initialized.");

            HES program = new HES(args);
            program.Run(args);

        }


        public HES(string[] args)
        {
            m_instance = this;

            m_serverInstance = new ServerInstance();
        }

        private void Run(string[] args)
        {
            ServerInstance.Instance.Start();

            ReadConsoleCommands();
        }

        public void ReadConsoleCommands()
        {
            string cmd = Console.ReadLine();

            if (!cmd.StartsWith("/"))
            {
                NetworkController.Instance.MessageAllClients(cmd);
            }
            else
            {
                Match cmd1 = Regex.Match(cmd, @"^(/help)");
                if (cmd1.Success)
                {
                    try
                    {
                        PrintHelp();
                    }
                    catch (ArgumentException) { }
                }

                Match cmd2 = Regex.Match(cmd, @"^(/players)");
                if (cmd2.Success)
                {
                    Console.WriteLine("Players Connected: " + ServerInstance.Instance.Server.NetworkController.CurrentOnlinePlayers());
                }

                Match cmd3 = Regex.Match(cmd, @"^(/save)");
                if (cmd3.Success)
                {
                    ServerInstance.Instance.Save();  
                }
            }

            ReadConsoleCommands();
        }


        [STAThread]
        static void LoadGUI()
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

        private enum CtrlType
        {
            CTRL_C_EVENT = 0,
            CTRL_BREAK_EVENT = 1,
            CTRL_CLOSE_EVENT = 2,
            CTRL_LOGOFF_EVENT = 5,
            CTRL_SHUTDOWN_EVENT = 6,
        }

        public static void PrintHelp()
        {
            Console.WriteLine("-------------------------HELP--------------------------------");
            Console.WriteLine("Type directly into the console to chat with online players");
            Console.WriteLine("Current commands are;" + Environment.NewLine);
            Console.WriteLine("/help - this page ;)");
            Console.WriteLine("/players - returns the current amount of online players");
            Console.WriteLine("/save - forces a universe save");
            Console.WriteLine("-------------------------------------------------------------");
        }

    }
}
