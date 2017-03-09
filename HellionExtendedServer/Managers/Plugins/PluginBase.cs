﻿using System;
using System.Reflection;
using System.Runtime.InteropServices;
using HellionExtendedServer.Common;
using HellionExtendedServer.Managers;
using HellionExtendedServer.Managers.Commands;
using HellionExtendedServer.Managers.Plugins;
using ZeroGravity;
using ZeroGravity.Objects;


namespace HellionExtendedServer.Common.Plugins
{
    public abstract class PluginBase : IPlugin
    {
        #region Fields
        protected String m_directory;
        protected Server m_server;
        protected PluginHelper m_plugin_helper;
        protected Boolean isenabled = false;

        //protected LogInstance m_log;
        protected PluginBaseConfig m_config;
        #endregion

        #region Properties
        public virtual Boolean Enabled { get { return isenabled; } internal set { isenabled = value; } }
        public virtual Guid Id { get { return Id; } internal set { Id = value; } }
        public virtual String Name { get { return Name; } internal set { Name = value; } }
        public virtual String Version { get { return Version; } internal set { Version = value; } }
        public virtual String Description { get { return Description; } internal set { Version = value; } }
        public virtual String Author { get { return Author; } internal set { Version = value; } }
        public virtual String Directory { get { return m_directory; } internal set { Version = value; } }
        public virtual String API { get { return API; } internal set { API = value; } }
        public virtual Server GetServer { get { return m_server; } }
        public virtual PluginHelper GetPluginHelper { get { return m_plugin_helper; } }

        //public virtual LogInstance PluginLog { get { return m_log; } }
        public virtual PluginBaseConfig Config { get { return m_config; } }
        #endregion

        #region Methods
        public PluginBase()
        {
            m_server = ServerInstance.Instance.Server;
            //Create an Attribute for this!
            //Assembly assembly = Assembly.GetCallingAssembly(); Console.WriteLine("ssssssssss");
            //GuidAttribute guidAttr = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
            //this.Id = new Guid(guidAttr.Value);

            //AssemblyName asmName = assembly.GetName(); Console.WriteLine("ssssssssss");
            //BUG Cause a Stack Overflow
            //Name = asmName.Name; Console.WriteLine("ssssssssss");

            //Version = asmName.Version.ToString(); Console.WriteLine("ssssss2222ssss");

            m_plugin_helper = new PluginHelper(m_server);
        }
        public virtual void Init(String modDirectory)
        {
            Enabled = true;
            m_directory = modDirectory;
            m_server = ServerInstance.Instance.Server;
            if (m_server == null)
            {
                //ERROR! No Server Running!
                Console.WriteLine("No Running Server found!");
                OnDisable();
            }

            PluginAttribute pluginAttribute = Attribute.GetCustomAttribute(GetType(), typeof(PluginAttribute), true) as PluginAttribute;
            if (pluginAttribute != null)
            {
                Name = pluginAttribute.Name;
                Version = pluginAttribute.Version;
                Description = pluginAttribute.Description;
                Author = pluginAttribute.Author;
                API = pluginAttribute.API;
            }
            else
            {
                Enabled = false;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error! No Plugin Attribute Found!");
                Console.ResetColor();
                return;
            }

            Console.WriteLine("Plugin Loaded!");
            //#TODO add Logger
        }

        public void Shutdown()
        {
            DisablePlugin();
        }
        public void OnLoad()
        {

        }

        public void DisablePlugin()
        {
            Enabled = true;
            OnDisable();
            ServerInstance.Instance.PluginManager.ShutdownPlugin(Name);
        }
        public virtual void OnEnable()
        {

        }
        public virtual void OnDisable()
        {

        }
        public virtual void OnCommand(Player p, String command, String[] args)
        {

        }

        #endregion
    }
}
