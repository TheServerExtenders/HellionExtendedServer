using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using ZeroGravity;
using HellionExtendedServer.Managers;
using System.Net.Sockets;
using HellionExtendedServer.Common;
using System.Diagnostics;

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
        private MethodInfo m_closeSocketListeners;
        private HELLION m_instance;
        private Server m_server;
        private Thread serverThread;

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
                Log.Instance.Fatal(ex, "Hellion Extended Server [REFLECTION ERROR] : " + ex.Message);
            }

            try
            {
                m_closeSocketListeners = Assembly.GetType("ZeroGravity.Network.NetworkController").GetMethod("OnApplicationQuit", 
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            }
            catch (ArgumentException ex)
            {
                Log.Instance.Fatal(ex, "Hellion Extended Server [REFLECTION ERROR] : " + ex.Message);
            }

            try
            {
               // m_closeSocketListeners = Assembly.GetType("ZeroGravity.Server.NetworkController").GetMethod("OnApplicationQuit",
                    //BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.InvokeMethod);
            }
            catch (ArgumentException ex)
            {
                Log.Instance.Error(ex, "Hellion Extended Server [REFLECTION ERROR] : " + ex.Message);
            }
        }

        public void StopServer()
        {
            bool isSaving = false;

            try
            {
                Log.Instance.Info(HES.Localization.Sentences["ShuttingDown"]);
               
                if (Server.PersistenceSaveInterval > 0.0)
                {
                    //ServerInstance.Instance.Save();
                    isSaving = true;
                    Stopwatch saveTime = new Stopwatch();
                    saveTime.Start();
                    Console.WriteLine(HES.Localization.Sentences["SavingUniverse"]);
                    Persistence.Save();
                    saveTime.Stop();
                    Log.Instance.Info(
                        string.Format(HES.Localization.Sentences["SavedUniverseTime"], 
                        saveTime.Elapsed.Milliseconds, 
                        string.Format((string)Persistence.PersistanceFileName, 
                        DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss"))));
                }

                m_closeSocketListeners.Invoke(Server.Instance.NetworkController, null);

                Log.Instance.Info("Logging out all clients...");
                foreach (var client in Server.Instance.NetworkController.clientList)
                {
                    client.Value.Thread.Stop();

                    if (client.Value.Player != null)
                    {
                        client.Value.Player.LogoutDisconnectReset();
                        client.Value.Player.DiconnectFromNetworkContoller();
                    }
                }
                Server.Instance.NetworkController.clientList.Clear();

                Server.IsRunning = false;

                Dbg.Destroy();

                Log.Instance.Info("Ending the Server loop...");

                Server.MainLoopEnded.WaitOne(5000);

                Log.Instance.Info("Loop ended.");

                ServerInstance.Instance.IsRunning = false;

                GC.Collect();
                GC.WaitForPendingFinalizers();

                Log.Instance.Info(HES.Localization.Sentences["SuccessShutdown"]);
            }
            catch (Exception ex)
            {

                Log.Instance.Error(ex, "Hellion Extended Server [SHUTDOWN ERROR] : " + ex.Message);
            }
            
        }

        /// <summary>
        /// Starts Hellion Dedicated in its own thread
        /// </summary>
        /// <param name="args">command line arg passthrough</param>
        /// <returns>the thread!</returns>
        public Thread StartServer(Object args)
        {
            Log.Instance.Info(HES.Localization.Sentences["LoadingDedicated"]);

            serverThread = new Thread(new ParameterizedThreadStart(this.ThreadStart));

            serverThread.IsBackground = true;
            serverThread.CurrentCulture = CultureInfo.InvariantCulture;
            serverThread.CurrentUICulture = CultureInfo.InvariantCulture;

            try
            {
                GC.Collect();
                // Start the thread!
                serverThread.Start(args);
            }
            catch (Exception ex)
            {
                Log.Instance.Fatal(ex, "Hellion Extended Server [SERVER THREAD ERROR] : " + ex.Message);
                return null;
            }

            Log.Instance.Warn(HES.Localization.Sentences["WaitingStart"]);

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
                Log.Instance.Fatal("Hellion Extended Server [FATAL ERROR] : " + ex.ToString());
                return null;
            }

            // The server is now running, set the running field to true
            m_isRunning = m_instance.Server.WorldInitialized;

            ServerInstance.Instance.IsRunning = true;

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
                Log.Instance.Fatal("Hellion Extended Server [UNHANDLED EXCEPTION] : " + ex.ToString());
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
            }
            catch (Exception ex)
            {
                Log.Instance.Fatal(ex, "Hellion Extended Server [GAMESERVERINI PROPERTIES ERROR] :  " + ex.Message);
            }

            try
            {
                m_server = new Server();
            }
            catch (Exception ex)
            {
                Log.Instance.Fatal(ex, "Hellion Extended Server [NEW SERVER INSTANCE ERROR] :  " + ex.Message);
            }

            try
            {           
               
                m_server.MainLoop();
            }
            catch(ArgumentException ex)
            {

            }
            catch (TypeInitializationException ex)
            {
                Log.Instance.Fatal(ex, "[REPORT THE FOLLOWING TO GITHUB ISSUES] Could Not Initialize Server! : [FATAL ERROR]" + ex.ToString());
            }
            catch (Exception ex)
            {

                string inner = "";
                if (ex.InnerException != null)
                    inner = "\r\n InnerException " + ex.InnerException.StackTrace;


                Log.Instance.Fatal(ex, "Hellion Extended Server [START MAINLOOP EXCEPTION] :  " + ex.ToString() + inner);
            }
        }

        #endregion Methods
    }
}