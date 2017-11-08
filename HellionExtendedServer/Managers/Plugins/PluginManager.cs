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
                    //Enable 
                    //Enable Events 
                    foreach (EventListener el in Plugin.FoundEvents)
                    {
                        ServerInstance.Instance.EventHelper.RegisterEvent(el);
                    }

                    //Plugin.FoundEvents.ForEach(x => ServerInstance.Instance.EventHelper.RegisterEvent(x));

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
                    //BUG HUGE!!!!!
                    if (Plugin.MainClass != null)
                    {
                        Plugin.MainClass.DisablePlugin();
                    }
                    Plugin.MainClass = null;
                }
                catch (Exception ex)
                {
                    Log.Instance.Error("ERror!!!" + ex);
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
                        pb.DisablePlugin(false);
                        m_loadedPlugins.Remove(Plugininfo);
                        m_discoveredPlugins.Remove(Plugininfo);
                        return;
                    }

                }

            }
        }

        public List<Assembly> LoadPluginReferences(string pluginFolder)
        {
            List<Assembly> pluginReferences = new List<Assembly>();

            try
            {               
                string[] subDirectories = Directory.GetDirectories(pluginFolder);
                foreach (string path in subDirectories)
                {
                    string[] files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
                    foreach (string file in files)
                    {
                        try
                        {
                            if (IsValidAssembly(file))
                            {
                                Assembly pluginreference = Assembly.UnsafeLoadFrom(file);
                                pluginReferences.Add(pluginreference);
                            }
                            else
                            {
                                Console.WriteLine($"WARNING: '{Path.GetFileName(file)}' is not valid for plugin '{Path.GetDirectoryName(pluginFolder)}' Reference will not be loaded.");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Failed to load plugin reference assembly '{Path.GetFileName(file)}' for plugin '{Path.GetDirectoryName(pluginFolder)}' Error: " + ex.ToString());
                        }
                    }
                }
                return pluginReferences;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to load plugin references for {Path.GetDirectoryName(pluginFolder)} Error: " + ex.ToString());
            }

            return null;
        }

        public List<PluginInfo> FindAllPlugins()
        {
            List<PluginInfo> foundPlugins = new List<PluginInfo>();

            //TODO create Plugin Folder if it does not exist
            String modPath = Path.Combine(Environment.CurrentDirectory, "hes/plugins");
            String[] subDirectories = Directory.GetDirectories(modPath);

            foreach (String subDirectory in subDirectories)
            {
                // We want to load the plugins references into memory before loading the 
                // actual plugin to avoid setting off the assembly resolver for hes
                LoadPluginReferences(subDirectory);

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
                        //Permissions In Command
                        //Load Permissions
                        foreach (Attribute attribute in PluginType.GetCustomAttributes(true))
                        {
                            if (attribute is PermissionAttribute)
                            {
                                PermissionAttribute pa = attribute as PermissionAttribute;
                                //Add To plugin
                                //Onplayer Join Event Add Default Perms to player
                                ServerInstance.Instance.PermissionManager.AddPermissionAttribute(pa);
                            }
                        }
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
                if (!plug)
                {
                    //Loads Events
                    foreach (MethodInfo method in plugin.MainClassType.GetMethods())
                    {
                        Boolean isevent = false;
                        foreach (Attribute attribute in method.GetCustomAttributes(true))
                        {
                            if (attribute is HESEventAttribute)
                            {
                                HESEventAttribute hea = attribute as HESEventAttribute;

                                plugin = HandelEvent(method, plugin, hea.EventType);
                            }
                        }
                    }
                    //Load Permissions
                    foreach (Attribute attribute in plugin.GetType().GetCustomAttributes(true))
                    {
                        if (attribute is PermissionAttribute)
                        {
                            PermissionAttribute pa = attribute as PermissionAttribute;
                            //Add To plugin
                            //Onplayer Join Event Add Default Perms to player
                            ServerInstance.Instance.PermissionManager.AddPermissionAttribute(pa);
                        }
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

        public PluginInfo HandelEvent(MethodInfo method, PluginInfo plugin, EventID eid)
        {
            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length <= 0)
            {
                Log.Instance.Error("Paramater had no lenght! Method Name: " + method.Name);
                return plugin;

            }
            if (parameters[0].ParameterType.BaseType != typeof(Event.Event))
            {
                Log.Instance.Error("INVLAID Function Format! Event Expect but got " + parameters[0].Name);
                return plugin;
            }

            EventListener el = new EventListener(method, plugin.MainClassType, eid);
            plugin.FoundEvents.Add(el);
            Log.Instance.Info("Found Event Functon : " + parameters[0].ParameterType.Name + " For EventType : " + eid);
            return plugin;
        }

        // Utility method for loading plugin references
        public bool IsValidAssembly(string path)
        {
            try
            {
                var assembly = AssemblyName.GetAssemblyName(path);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion Methods
    }
}