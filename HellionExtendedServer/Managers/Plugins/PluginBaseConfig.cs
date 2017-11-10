using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace HellionExtendedServer.Common.Plugins
{
    public delegate void ConfigEventHandler();

    public class PluginBaseConfig
    {
        #region Fields

        protected String m_configFile;
        protected PluginBase m_plugin;
        protected Object[] m_settings = new Object[] { };
        protected readonly Object _lockObj = new Object();

        #endregion Fields

        #region Events

        public event ConfigEventHandler ConfigLoaded;

        public event ConfigEventHandler ConfigSaved;

        #endregion Events

        #region Properties

        public virtual Object[] Settings { get { return m_settings; } set { m_settings = value; } }

        #endregion Properties

        #region Methods

        public PluginBaseConfig(PluginBase plugin)
        {
            m_plugin = plugin;
        }

        public virtual void Init()
        {
            m_configFile = Path.Combine(m_plugin.Directory, m_plugin.GetName + ".xml");
        }

        public virtual void Save()
        {
            try
            {
                XmlSerializer x = new XmlSerializer(Settings.GetType());
                TextWriter writer = new StreamWriter(m_configFile);
                lock (_lockObj)
                {
                    x.Serialize(writer, Settings);
                }
                writer.Close();
            }
            catch (Exception)
            {
                //m_plugin.PluginLog.WriteLineAndConsole("Could not save configuration: " + ex.ToString());
                return;
            }
            if (ConfigSaved != null)
            {
                ConfigSaved();
            }
        }

        public virtual void Load()
        {
            try
            {
                if (File.Exists(m_configFile))
                {
                    XmlSerializer x = new XmlSerializer(Settings.GetType());
                    XmlTextReader reader = new XmlTextReader(m_configFile);
                    lock (_lockObj)
                    {
                        Settings = (Object[])x.Deserialize(reader);
                        reader.Close();
                    }
                }
            }
            catch (Exception)
            {
                //m_plugin.PluginLog.WriteLineAndConsole("Could not load configuration: " + ex.ToString());
                return;
            }
            if (ConfigLoaded != null)
            {
                ConfigLoaded();
            }
        }

        #endregion Methods
    }
}