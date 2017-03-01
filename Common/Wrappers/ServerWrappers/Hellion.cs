using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Globalization;

using ZeroGravity;

namespace HellionExtendedServer.Common.Wrappers.ServerWrappers
{
    public class HELLION : ReflectionClassWrapper
    {
        #region Fields
        private Boolean m_isRunning;
        private MethodInfo m_entryPoint;
        private ReflectionProperty m_instanceProperty;
        private HELLION m_instance;
        private Server m_server;
        #endregion

        #region Properties
        public override String ClassName { get { return "Server"; } }

        public HELLION Instance { get {return m_instance; } }

        public Server Server { get { return m_server; } }

        public Boolean IsRunning { get { return m_isRunning; } }
        #endregion

        public HELLION(Assembly Assembly, string Namespace)
            : base(Assembly, Namespace)
        {
            m_instance = this;
            SetupReflection(Assembly);
        }

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

        public Thread StartServer(Object args)
        {

            Console.WriteLine("Hellion Extended Server: Loading HELLION Dedicated.");

            Thread serverThread = new Thread(new ParameterizedThreadStart(this.ThreadStart));

            serverThread.IsBackground = true;
            serverThread.CurrentCulture = CultureInfo.InvariantCulture;
            serverThread.CurrentUICulture = CultureInfo.InvariantCulture;

            try
            {
                serverThread.Start(args);
            }
            catch (Exception ex)
            {

                Console.WriteLine("[ERROR] Hellion Extended Server: " + ex.ToString());
                return null;
            }

            
            Console.WriteLine("Hellion Extended Server: Waiting for server to start. This may take at least 10 seconds.");

            try
            {
                // TODO: new way
                Thread.Sleep(8000);

                SpinWait.SpinUntil(() => m_instance.Server.WorldInitialized);

                m_isRunning = m_instance.Server.WorldInitialized;
            }
            catch (NullReferenceException)
            {
                // keep waiting!
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Hellion Extended Server: " + ex.ToString());
            }

            return serverThread;
        }

        private void ThreadStart(Object args)
        {
            try
            {
                ServerWrapper.HellionDedi.Start(args as Object[]);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unhandled Exception caused server to crash. Exception: " + ex.ToString());
            }
            m_isRunning = false;
        }

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
            catch (Exception ex)
            {
                Console.WriteLine("HES: Server Start Exception: " + ex.ToString());
            }
            
        }
    }
}
