using System;
using System.Reflection;
using System.Runtime.InteropServices;
using HellionExtendedServer.Common;


namespace HellionExtendedServer.Common.Plugins
{
    public abstract class PluginBase : IPlugin
    {
        #region Fields
        protected Guid m_PluginId;
        protected String m_name;
        protected String m_version;
        protected String m_directory;

        //protected LogInstance m_log;
        protected PluginBaseConfig m_config;
        #endregion

        #region Properties
        public virtual Guid Id { get { return m_PluginId; } }
        public virtual String Name { get { return m_name; } }
        public virtual String Version { get { return m_version; } }
        public virtual String Directory { get { return m_directory; } }

        //public virtual LogInstance PluginLog { get { return m_log; } }
        public virtual PluginBaseConfig Config { get { return m_config; } }
        #endregion

        #region Methods
        public PluginBase()
        {
            Assembly assembly = Assembly.GetCallingAssembly();
            GuidAttribute guidAttr = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            m_PluginId = new Guid(guidAttr.Value);

            AssemblyName asmName = assembly.GetName();
            m_name = asmName.Name;

            m_version = asmName.Version.ToString();
        }
        public virtual void Init(String modDirectory)
        {
            m_directory = modDirectory;
        }

        public abstract void Shutdown();
        #endregion
    }
}
