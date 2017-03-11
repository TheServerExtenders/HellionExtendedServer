using HellionExtendedServer.Common.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using HellionExtendedServer.Common;
using HellionExtendedServer.Managers.Commands;
using HellionExtendedServer.Managers.Event;

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
            Console.WriteLine(String.Format("Found {0} Plugins!", m_discoveredPlugins.Count));
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
                    //Now Look for Events... IN THE PLUGIN TYPE!!!!!!!
                    //Actually Just register them
                    //Events
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
                        Plugin.MainClass.DisablePlugin();
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

            //TODO create Plugin Folder if it does not exist
            String modPath = Path.Combine(Environment.CurrentDirectory, "hes/plugins");
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
                Console.WriteLine("Loading Plugin Located at " + library);
                bytes = File.ReadAllBytes(library);
                libraryAssembly = Assembly.Load(bytes);
                //Bug Guid is Glitched Right Now
                //Guid guid = new Guid(((GuidAttribute)libraryAssembly.GetCustomAttributes(typeof(GuidAttribute), true)[0]).Value);

                bool plug = true;
                PluginInfo plugin = new PluginInfo();
                //Plugin.Guid = guid;
                plugin.Assembly = libraryAssembly;

                Command[] CommandList;

                Type[] PluginTypes = libraryAssembly.GetExportedTypes();

                foreach (Type PluginType in PluginTypes)
                {
                    if (PluginType.BaseType == typeof(Command))
                    {
                        plugin.FoundCommands.Add(PluginType);
                        continue;
                    }
                    if (PluginType.GetInterface(typeof(IPlugin).FullName) != null && plug)
                    {
                        plugin.MainClassType = PluginType;
                        plug = false;
                        continue;
                    }
                }
                //B4 resturn Check for Events here
                //Now Look for Events... IN THE PLUGIN TYPE!!!!!!!
                //Events
                if (plug)
                {
                    foreach (MethodInfo method in plugin.MainClass.GetType().GetMethods())
                    {
                        Boolean isevent = false;
                        foreach (object attribute in method.GetCustomAttributes(true))
                        {
                            if (attribute is HESEventAttribute)
                            {
                                isevent = true;
                                break;
                            }
                            if (isevent) break;
                        }
                        if (!isevent) continue;//Your not an event! Get outa here!!!
                        //Check paramaters now

                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters.Length <= 0)
                        {
                            Log.Instance.Error("Paramater had no lenght! Method Name: " + method.Name);
                            continue;

                        }
                        if (parameters[0].ParameterType.BaseType != typeof(Event.Event))
                        {
                            Log.Instance.Error("INVLAID Function Format! Event Expect but got " + parameters[0].Name);
                            continue;
                        }

                        EventListener el = new EventListener(method);
                        plugin.FoundEvents.Add(el);
                        Log.Instance.Info("Found Event > "+parameters[0].Name);
                    }
                }
                return plugin;
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