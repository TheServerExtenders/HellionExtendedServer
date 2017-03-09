using HellionExtendedServer.Controllers;
using HellionExtendedServer.ServerWrappers;
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ZeroGravity;
using HellionExtendedServer.Components;
using System.Net.Sockets;
using ZeroGravity.Objects;

namespace HellionExtendedServer.Managers
{
    public class ServerInstance
    {
        #region Fields

        private static Thread m_serverThread;

        private Assembly m_assembly;
        private DateTime m_launchedTime;
        private Server m_server;
        private ServerWrapper m_serverWrapper;
        private GameServerIni m_gameServerIni;

        private static ServerInstance m_serverInstance;

        private bool isSaving = false;

        #endregion Fields

        #region Properties

        public TimeSpan Uptime { get { return DateTime.Now - m_launchedTime; } }
        public Boolean IsRunning { get { return ServerWrapper.HellionDedi.IsRunning; } }
        public Assembly Assembly { get { return m_assembly; } }
        public Server Server { get { return m_server; } }
        public GameServerIni Config { get { return m_gameServerIni; } set { m_gameServerIni = value; } }
        

        public static ServerInstance Instance { get { return m_serverInstance; } }

        #endregion Properties

        public ServerInstance()
        {
                       
            m_launchedTime = DateTime.MinValue;

            m_serverThread = null;
            m_serverInstance = this;

            m_assembly = Assembly.LoadFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HELLION_Dedicated.exe"));
            m_serverWrapper = new ServerWrapper(m_assembly);

            

        }

        #region Methods

        /// <summary>
        /// Saves the server on demand when ran, will not let it save while saving
        /// Starts the save in a different thread to avoid locking up HES
        /// </summary>
        public void Save(bool showToPLayer = false)
        {
            if (Server.IsRunning)
                return;
            if (this.isSaving)
            {
                Console.WriteLine(HES.Localization.Sentences["SaveAlreadyInProgress"]);
            }
            else
            {
                try
                {
                    if (showToPLayer)
                    {
                        NetworkController.Instance.MessageAllClients(HES.Localization.Sentences["SavingUniverse"], false, false);
                        NetworkController.Instance.MessageAllClients(HES.Localization.Sentences["SavedUniverse"], false, false);
                    }

                    new TaskFactory().StartNew(() =>
                    {
                        isSaving = true;
                        Stopwatch saveTime = new Stopwatch();
                        saveTime.Start();
                        Console.WriteLine(HES.Localization.Sentences["SavingUniverse"]);
                        Persistence.Save();
                        saveTime.Stop();

                        Log.Instance.Info(string.Format(HES.Localization.Sentences["SavedUniverseTime"], saveTime.Elapsed.Milliseconds, string.Format((string)Persistence.PersistanceFileName, DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss"))));
                        isSaving = false;
                    });
                }
                catch (Exception ex)
                {
                    Log.Instance.Error("Hellion Extended Server [SAVE ERROR] " + ex.ToString());
                    isSaving = false;
                }
            }
        }

        public void Test()
        {

           foreach( SpaceObjectVessel vessel in m_server.AllVessels)
            {
                Console.WriteLine(String.Format("Ship ({0}) Pos: {1} | Angles: {2} | Velocity: {3} | AngularVelocity: {4} ",vessel.GUID, vessel.Position.ToString(), vessel.Rotation.ToString(),vessel.Velocity, vessel.AngularVelocity));
                
            }

        }

        /// <summary>
        /// The main start method that loads the controllers and prints information to the console
        /// </summary>
        public void Start()
        {
            if (Server.IsRunning)
                return;

            String[] serverArgs = new String[]
                {
                    "",
                };

            m_serverThread = ServerWrapper.HellionDedi.StartServer(serverArgs);

            m_serverWrapper.Init();

            Thread.Sleep(5000);

            m_server = ServerWrapper.HellionDedi.Server;

            if (IsRunning)
            {
                Log.Instance.Info("Hellion Extended Server: World Initialized!");

                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                Thread.Sleep(1);
                stopwatch.Stop();
                long num = (long)(1000.0 / stopwatch.Elapsed.TotalMilliseconds);

                Console.WriteLine(string.Format(HES.Localization.Sentences["ServerDesc"], DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.ffff"), (Server.NetworkController.ServerID <= 0L ? "Not yet assigned" : string.Concat(Server.NetworkController.ServerID)), 64, num, (64 > num ? " WARNING: Server ticks is larger than max tick" : ""), Server.ServerName));
            }

            new NetworkController(m_server.NetworkController);

            Log.Instance.Info(HES.Localization.Sentences["ReadyForConnections"]);

            HES.PrintHelp();
        }

        public void Stop()
        {
            ServerWrapper.HellionDedi.StopServer();
            m_serverThread.Join(60000);
            m_serverThread.Abort();
        }

        #endregion Methods
    }
}