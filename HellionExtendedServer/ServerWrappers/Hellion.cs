using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using ZeroGravity;
using HellionExtendedServer.Managers;

namespace HellionExtendedServer.ServerWrappers
{
    /// <summary>
    /// This contains the main reflection to properly start Hellion Dedicated
    /// </summary>
    public class HELLION : ReflectionClassWrapper
    {
        #region Fields

        private Boolean m_isRunning;
        private MethodInfo m_entryPoint;
        private ReflectionProperty m_instanceProperty;
        private HELLION m_instance;
        private Server m_server;

        #endregion Fields

        #region Properties

        public override String ClassName { get { return "Server"; } }

        public HELLION Instance { get { return m_instance; } }

        public Server Server { get { return m_server; } }

        public Boolean IsRunning { get { return m_isRunning; } }

        #endregion Properties

        public HELLION(Assembly Assembly, string Namespace)
            : base(Assembly, Namespace)
        {
            m_instance = this;
            SetupReflection(Assembly);
        }

        #region Methods

        private void SetupReflection(Assembly Assembly)
        {
            try
            {
                m_entryPoint = Assembly.EntryPoint;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.ToString());
            }

            try
            {
                m_instanceProperty = new ReflectionProperty("Instance", ClassName, m_classType);
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void StopServer()
        {
        }

        /// <summary>
        /// Starts Hellion Dedicated in its own thread
        /// </summary>
        /// <param name="args">command line arg passthrough</param>
        /// <returns>the thread!</returns>
        public Thread StartServer(Object args)
        {
            Log.Instance.Info("Hellion Extended Server: Loading HELLION Dedicated.");

            Thread serverThread = new Thread(new ParameterizedThreadStart(this.ThreadStart));

            serverThread.IsBackground = true;
            serverThread.CurrentCulture = CultureInfo.InvariantCulture;
            serverThread.CurrentUICulture = CultureInfo.InvariantCulture;

            try
            {
                // Start the thread!
                serverThread.Start(args);
            }
            catch (Exception ex)
            {
                Log.Instance.Fatal("[ERROR] Hellion Extended Server[ServerThread]: " + ex.ToString());
                return null;
            }

            Log.Instance.Warn("Hellion Extended Server: Waiting for server to start. This may take at least 10 seconds or longer depending on the size of the current save.");

            try
            {
                // wait 8 seconds before accessing the variable to make sure its not null
                Thread.Sleep(8000);

                // Waits for the world to initialize before hooking into the class
                SpinWait.SpinUntil(() => m_instance.Server.WorldInitialized);
            }
            catch (NullReferenceException)
            {
                // keep waiting! This has no other purpose other than
            }
            catch (Exception ex)
            {
                Log.Instance.Fatal("[ERROR] Hellion Extended Server: " + ex.ToString());
                return null;
            }

            // The server is now running, set the running field to true
            m_isRunning = m_instance.Server.WorldInitialized;

            return serverThread;
        }

        /// <summary>
        /// Load Hellion Dedicated into memory
        /// </summary>
        /// <param name="args"></param>
        private void ThreadStart(Object args)
        {
            try
            {
                ServerWrapper.HellionDedi.Start(args as Object[]);
            }
            catch (Exception ex)
            {
                Log.Instance.Fatal("Unhandled Exception caused server to crash. Exception: " + ex.ToString());
            }
            m_isRunning = false;
        }

        /// <summary>
        /// Starts the server by calling MainLoop in the Server class, after
        /// setting hellions log directory and loading the properties from the config file.
        /// </summary>
        /// <param name="args"></param>
        private void Start(Object[] args)
        {
            try
            {
                Server.Properties = new ZeroGravity.Properties(Server.ConfigDir + "GameServer.ini");

                m_server = new Server();

                Dbg.OutputDir = Server.ConfigDir;
                Dbg.Initialize();

                m_server.MainLoop();
            }
            catch (TypeInitializationException ex)
            {
                Log.Instance.Fatal("[FATAL ERROR] REPORT THE FOLLOWING TO GITHUB ISSUES] HES: Could Not Initialize Server! : " + ex.ToString());
            }
            catch (Exception ex)
            {
                Log.Instance.Fatal("HES: Server Start Exception: " + ex.Message);
            }
        }

        #endregion Methods
    }
}