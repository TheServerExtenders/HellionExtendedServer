using HellionExtendedServer.Common;
using HellionExtendedServer.GUI;
using HellionExtendedServer.Managers.Commands;
using HellionExtendedServer.Managers.Event;
using HellionExtendedServer.Managers.Event.ServerEvents;
using HellionExtendedServer.Managers.Plugins;
using HellionExtendedServer.ServerWrappers;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using ZeroGravity;
using ZeroGravity.Network;
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
        private PluginManager m_pluginManager = null;
        private CommandManager m_commandManager;
        private PermissionManager m_permissionmanager;
        private EventHelper m_eventhelper = null;

        private static ServerInstance m_serverInstance;

        private bool isSaving = false;
        private Boolean m_isRunning;

        #endregion Fields

        #region Properties

        public TimeSpan Uptime { get { return DateTime.Now - m_launchedTime; } }
        public Assembly Assembly { get { return m_assembly; } }
        public Server Server { get { return m_server; } }
        public GameServerIni GameServerConfig => m_gameServerIni;
        public PluginManager PluginManager { get { return m_pluginManager; } }
        public CommandManager CommandManager { get { return m_commandManager; } }
        public EventHelper EventHelper { get { return m_eventhelper; } }
        public PermissionManager PermissionManager { get { return m_permissionmanager; } }

        public static ServerInstance Instance { get { return m_serverInstance; } }

        public Boolean IsRunning
        {
            get { return m_isRunning; }
            set
            {
                if (m_isRunning == value)
                {
                    return;
                }
                m_isRunning = value;
                if (m_isRunning)
                {
                    OnServerRunning?.Invoke(m_server);
                }
                else
                {
                    OnServerStopped?.Invoke(m_server);
                }
            }
        }

        #endregion Properties

        #region Events

        public delegate void ServerRunningEvent(Server server);

        public event ServerRunningEvent OnServerRunning;

        public event ServerRunningEvent OnServerStopped;

        #endregion Events

        public ServerInstance()
        {
            m_launchedTime = DateTime.MinValue;

            m_serverThread = null;
            m_serverInstance = this;

            string gameExePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "HELLION_Dedicated.exe");

            if (System.IO.File.Exists(gameExePath))
            {
                m_assembly = Assembly.LoadFile(gameExePath);
                m_serverWrapper = new ServerWrapper(m_assembly);
            }
            else
                Console.WriteLine($"HELLION_Dedicated.exe not detected at {gameExePath}.\r\n Press any key to close.");

            //m_gameServerProperties = new GameServerProperties();
            //m_gameServerProperties.Load();

            m_gameServerIni = new GameServerIni();
            m_gameServerIni.Load();
        }

        #region Methods

        /// <summary>
        /// Saves the server on demand when ran, will not let it save while saving
        /// Starts the save in a different thread to avoid locking up HES
        /// </summary>
        public void Save(bool showToPLayer = false)
        {
            if (!Server.IsRunning)
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
                        NetworkManager.Instance.MessageAllClients(HES.Localization.Sentences["SavingUniverse"], false, false);
                        NetworkManager.Instance.MessageAllClients(HES.Localization.Sentences["SavedUniverse"], false, false);
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

        // Test method, please don't change ;)
        public void Test()
        {
            foreach (SpaceObjectVessel vessel in m_server.AllVessels)
            {
                Console.WriteLine(String.Format("Ship ({0}) Pos: {1} | Angles: {2} | Velocity: {3} | AngularVelocity: {4} ", vessel.GUID, vessel.Position.ToString(), vessel.Rotation.ToString(), vessel.Velocity, vessel.AngularVelocity));
            }
        }

        /// <summary>
        /// The main start method that loads the controllers and prints information to the console
        /// </summary>
        public async void Start()
        {
            if (m_assembly == null)
            {
                Console.WriteLine($"HELLION_Dedicated.exe does not exist.\r\n Cannot start the server.");
                return;
            }

            if (Server.IsRunning)
                return;

            String[] serverArgs = new String[]
                {
                    "",
                };

            await ServerWrapper.HellionDedi.StartServer(serverArgs);
            m_serverWrapper.Init();

            while (ServerWrapper.HellionDedi.Server == null)
            {
                await Task.Delay(25);
            }

            m_server = ServerWrapper.HellionDedi.Server;
            OnServerRunning?.Invoke(m_server);

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

            Server.NetworkController.EventSystem.RemoveListener(typeof(TextChatMessage), new EventSystem.NetworkDataDelegate(Server.TextChatMessageListener));//Deletes Old Listener
            Server.NetworkController.EventSystem.AddListener(typeof(TextChatMessage), new EventSystem.NetworkDataDelegate(this.TextChatMessageListener));//Referances New Listener

            new NetworkManager(m_server.NetworkController);
            //Load Permission
            m_permissionmanager = new PermissionManager();
            //Load Events
            m_eventhelper = new EventHelper();
            //Load Commands
            m_commandManager = new CommandManager();
            //Load Plugins!
            m_pluginManager = new PluginManager();
            PluginManager.InitializeAllPlugins();
            //TODO load Server Event Listeners
            EventHelper.RegisterEvent(new EventListener(typeof(JoinEvent).GetMethod("PlayerSpawnRequest"), typeof(JoinEvent), EventID.PlayerSpawnRequest));
            //Command Listner

            Log.Instance.Info(HES.Localization.Sentences["ReadyForConnections"]);

            HES.PrintHelp();

            HES.KeyPressSimulator();
        }

        public void Stop()
        {
            if (PluginManager.LoadedPlugins != null)
                foreach (var plugin in PluginManager.LoadedPlugins)
                    PluginManager.ShutdownPlugin(plugin);

            try
            {
                PermissionManager.Save();
                ServerWrapper.HellionDedi.StopServer();
                m_serverThread.Join(60000);
                m_serverThread.Abort();
            }
            catch (Exception)
            {
            }
        }

        public void TextChatMessageListener(NetworkData data)
        {
            TextChatMessage textChatMessage = data as TextChatMessage;
            String Msg = textChatMessage.MessageText;

            if (Msg.StartsWith("/"))
            {
                Player player1 = Server.GetPlayer(textChatMessage.Sender);
                textChatMessage.Name = player1.Name;

                string[] chatCommandArray = Msg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string command = chatCommandArray.First();
                //Send to COmmand Manager
                if (chatCommandArray.Length > 1)
                {
                    CommandManager.HandlePlayerCommand(chatCommandArray.First().Replace("/", ""), chatCommandArray.Skip(1).ToArray(), player1);
                }
                else
                {
                    CommandManager.HandlePlayerCommand(chatCommandArray.First().Replace("/", ""), new String[] { }, player1);
                }

                return;
            }
            Server.TextChatMessageListener(data);
        }

        #endregion Methods
    }
}