using HellionExtendedServer.Common.Plugins;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using HellionExtendedServer.Managers.Commands;

namespace HellionExtendedServer.Managers.Plugins
{
    public class PluginManager
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
            foreach (PluginInfo Plugin in m_discoveredPlugins)
            {
                InitializePlugin(Plugin);
            }
        }

        public void InitializePlugin(PluginInfo Plugin)
        {
            Console.WriteLine(String.Format("Initializing Plugin: {0}", Plugin.Assembly.GetName().Name));
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
                        Console.WriteLine(
                            String.Format(
                                "Initialization of Plugin {0} failed. Could not find a public, parameterless constructor for {0}",
                                Plugin.Assembly.GetName().Name, Plugin.MainClassType.ToString()));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(String.Format("Failed initialzation of Plugin {0}. Uncaught Exception: {1}",
                            Plugin.Assembly.GetName().Name, ex.ToString()));
                    }
                }
                //Commands
                foreach(Type CommandType in Plugin.FoundCommands)
                {
                    Command commandclass = (Command) Activator.CreateInstance(CommandType, new object[] {ServerInstance.Instance.Server});
                    ServerInstance.Instance.CommandManager.AddCommand(commandclass);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Failed initialzation of Plugin {0}. Uncaught Exception: {1}", Plugin.Assembly.GetName().Name, ex.ToString()));
            }

            if (PluginInitialized)
            {
                lock (_lockObj)
                {
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
            Console.WriteLine(String.Format("Shutting down Plugin {0}", Plugin.Assembly.GetName().Name));
            lock (_lockObj)
            {
                try
                {
                    if (Plugin.MainClass != null)
                    {
                        Plugin.MainClass.DisablePlugin();
                    }
                    Plugin.MainClass = null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(String.Format("Uncaught Exception in Plugin {0}. Exception: {1}", Plugin.Assembly.GetName().Name, ex.ToString()));
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
                    if (pb.Name.ToLower() == Plugin)
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

            //TODO create Plugin Folder if it does not exist

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
                bytes = File.ReadAllBytes(library);
                libraryAssembly = Assembly.Load(bytes);
                Guid guid = new Guid(((GuidAttribute)libraryAssembly.GetCustomAttributes(typeof(GuidAttribute), true)[0]).Value);


                bool plug = true;
                PluginInfo Plugin = new PluginInfo();
                Plugin.Guid = guid;
                Plugin.Assembly = libraryAssembly;

                Command[] CommandList;

                Type[] PluginTypes = libraryAssembly.GetExportedTypes();

                foreach (Type PluginType in PluginTypes)
                {
                    if (PluginType.BaseType == typeof(Command))
                    {
                        Plugin.FoundCommands.Add(PluginType);
                        continue;
                    }
                    if (PluginType.GetInterface(typeof(IPlugin).FullName) != null && plug)
                    {
                        Plugin.MainClassType = PluginType;
                        plug = false;
                        continue;
                    }
                }
                 return Plugin;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load assembly: " + library + " Error: " + ex.ToString());
            }
            return null;
        }

        #endregion Methods
    }
}