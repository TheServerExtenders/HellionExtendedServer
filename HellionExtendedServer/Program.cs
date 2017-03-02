using System;
using System.Windows.Forms;
using System.Threading;
using System.Reflection;
using System.Text.RegularExpressions;


using HellionExtendedServer.Common;
using HellionExtendedServer.Managers;
using HellionExtendedServer.Controllers;

namespace HellionExtendedServer
{
    public class HES
    {

        private static HES m_instance;
        private static Form1 m_form;
        private static ServerInstance m_serverInstance;

        public static String BuildBranch { get { return "Master Branch"; } }
        public static Version Version { get { return Assembly.GetEntryAssembly().GetName().Version; } }
        public static String VersionString { get { return Version.ToString(3) + " " + BuildBranch; } }

        public static HES Instance { get { return m_instance; } }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {

            Thread uiThread = new Thread(LoadGUI);
            uiThread.SetApartmentState(ApartmentState.STA);
            //uiThread.Start(); //form disabled for now

            Console.Title = "HES ( HELLION EXTENDED SERVER V0.1 ) ";

            Console.WriteLine("Hellion Extended Server V0.1 starting...");

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
                        Console.WriteLine("Current commands are;");
                        Console.WriteLine("/help - this page ;)");
                    }
                    catch (ArgumentException)
                    {
                        Console.WriteLine("Missing message to send!");
                    }
                }

                Match cmd2 = Regex.Match(cmd, @"^(/players)");
                if (cmd2.Success)
                {
                    Console.WriteLine("Players Connected: " + ServerInstance.Instance.Server.AllPlayers.Count);
                }
            }

            ReadConsoleCommands();
        }


        [STAThread]
        static void LoadGUI()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (m_form == null || m_form.IsDisposed)
            	m_form = new Form1();
            else if (m_form.Visible)
            	return;
        }

    }
}
