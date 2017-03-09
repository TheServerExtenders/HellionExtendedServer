﻿using HellionExtendedServer.Common.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace HellionExtendedServer.Managers.Plugins
{
    internal class PluginManager
    {
        #region Fields

        private List<PluginInfo> m_discoveredPlugins;
        private List<PluginInfo> m_loadedPlugins;
        private readonly Object _lockObj = new Object();

        #endregion Fields

        #region Properties

        public List<PluginInfo> LoadedPlugins { get { return m_loadedPlugins; } }

        #endregion Properties

        #region Methods

        public PluginManager()
        {
            m_discoveredPlugins = new List<PluginInfo>();
            m_loadedPlugins = new List<PluginInfo>();
        }

        public void InitializeAllPlugins()
        {
            m_discoveredPlugins = FindAllPlugins();
            Console.WriteLine(String.Format("Found {0} Plugins!",m_discoveredPlugins.Count));
            foreach (PluginInfo Plugin in m_discoveredPlugins)
            {
                InitializePlugin(Plugin);
            }
        }

        public void InitializePlugin(PluginInfo Plugin)
        {
            Console.WriteLine(string.Format(HES.Localization.Sentences["InitializingPlugin"], Plugin.Assembly.GetName().Name));
            bool PluginInitialized = false;

            if (Plugin.Directory == null)
                Plugin.Directory = "";

            try
            {

                Plugin.MainClass = (PluginBase)Activator.CreateInstance(Plugin.MainClassType);
                

                if (Plugin.MainClass != null)
                {
                    try
                    {
                        Plugin.MainClass.Init(Plugin.Directory);
                        PluginInitialized = true;
                    }
                    catch (MissingMethodException)
                    {
                        Console.WriteLine(string.Format(HES.Localization.Sentences["InitializationPlugin"], Plugin.Assembly.GetName().Name, Plugin.MainClassType.ToString()));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format(HES.Localization.Sentences["FailedInitPlugin"], Plugin.Assembly.GetName().Name, ex.ToString()));
                    }

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(HES.Localization.Sentences["FailedInitPlugin"], Plugin.Assembly.GetName().Name, ex.ToString()));
            }

            if (PluginInitialized && Plugin.MainClass.Enabled)
            {
                lock (_lockObj)
                {
                    //Commands
                    foreach (Type CommandType in Plugin.FoundCommands)
                    {
                        Command c = (Command)Activator.CreateInstance(CommandType, new object[] { ServerInstance.Instance.Server });
                        ServerInstance.Instance.CommandManager.AddCommand(c);
                    }
                    m_loadedPlugins.Add(Plugin);
                }
            }
        }

        public void ShutdownAllPlugins()
        {
            List<PluginInfo> loadedPlugins;
            lock (_lockObj)
            {
                loadedPlugins = new List<PluginInfo>(m_loadedPlugins);
            }
            foreach (PluginInfo Plugin in loadedPlugins)
            {
                ShutdownPlugin(Plugin);
            }
        }

        public void ShutdownPlugin(PluginInfo Plugin)
        {
            Console.WriteLine(string.Format(HES.Localization.Sentences["ShutdownPlugin"], Plugin.Assembly.GetName().Name));
            lock (_lockObj)
            {
                try
                {
                    if (Plugin.MainClass != null)
                    {
                        Plugin.MainClass.Shutdown();
                    }
                    Plugin.MainClass = null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(string.Format(HES.Localization.Sentences["ShutdownPlugin"], Plugin.Assembly.GetName().Name, ex.ToString()));
                }
                m_loadedPlugins.Remove(Plugin);
                m_discoveredPlugins.Remove(Plugin);
            }
        }


        public void ShutdownPlugin(String Plugin)
        {
            lock (_lockObj)
            {
                foreach (PluginInfo Plugininfo in m_discoveredPlugins)
                {
                    PluginBase pb = Plugininfo.MainClass;
                    if (pb == null)
                    {
                        Console.WriteLine("Error 131!");
                        return;
                    }
                    if (pb.GetName.ToLower() == Plugin)
                    {
                        Console.WriteLine(String.Format("Shutting down Plugin {0}", Plugininfo.Assembly.GetName().Name));
                        pb.DisablePlugin();
                        m_loadedPlugins.Remove(Plugininfo);
                        m_discoveredPlugins.Remove(Plugininfo);
                        return;
                    }

                }

            }
        }

        public List<PluginInfo> FindAllPlugins()
        {
            List<PluginInfo> foundPlugins = new List<PluginInfo>();

            String modPath = Path.Combine(Environment.CurrentDirectory, "Plugins");
            String[] subDirectories = Directory.GetDirectories(modPath);

            foreach (String subDirectory in subDirectories)
            {
                PluginInfo Plugin = FindPlugin(subDirectory);

                if (Plugin != null)
                {
                    foundPlugins.Add(Plugin);
                }
            }

            m_discoveredPlugins = foundPlugins;

            return foundPlugins;
        }

        public PluginInfo FindPlugin(String directory)
        {
            String[] libraries = Directory.GetFiles(directory, "*.dll");

            foreach (String library in libraries)
            {
                PluginInfo Plugin = ValidatePlugin(library);
                if (Plugin != null)
                {
                    Plugin.Directory = directory;
                    return Plugin;
                }
            }
            return null;
        }

        private PluginInfo ValidatePlugin(String library)
        {
            byte[] bytes;
            Assembly libraryAssembly;
            try
            {
                Console.WriteLine("Loading Plugin Located at "+library);
                bytes = File.ReadAllBytes(library);
                libraryAssembly = Assembly.Load(bytes);
				
                //Bug Guid is Glitched Right Now
                //Guid guid = new Guid(((GuidAttribute)libraryAssembly.GetCustomAttributes(typeof(GuidAttribute), true)[0]).Value);
                
                bool plug = true;

                PluginInfo Plugin = new PluginInfo();
                //Plugin.Guid = guid;
                Plugin.Assembly = libraryAssembly;

                Type[] PluginTypes = libraryAssembly.GetExportedTypes();

                foreach (Type PluginType in PluginTypes)
                {
                    if (PluginType.GetInterface(typeof(IPlugin).FullName) != null)
                    {
                        Plugin.MainClassType = PluginType;
                        return Plugin;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format(HES.Localization.Sentences["FailedLoadAssembly"], library, ex.ToString()));
            }
            return null;
        }

        #endregion Methods
    }
}