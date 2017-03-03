using System.Reflection;

namespace HellionExtendedServer.ServerWrappers
{
    public class ServerWrapper : ReflectionAssemblyWrapper
    {
        #region Fields

        private const string ServerNamespace = "ZeroGravity";

        private static HELLION m_server;

        #endregion Fields

        #region Properties

        public static HELLION HellionDedi { get { return m_server; } }

        #endregion Properties

        #region Methods

        public ServerWrapper(Assembly serverAssembly)
            : base(serverAssembly)
        {
            m_server = new HELLION(serverAssembly, ServerNamespace);
        }

        internal void Init()
        {
            m_server.Init();
        }

        #endregion Methods
    }
}