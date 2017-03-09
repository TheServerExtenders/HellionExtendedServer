using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common.Plugins;
using HellionExtendedServer.Managers.Plugins;
using ZeroGravity;
using ZeroGravity.Objects;

namespace HellionExtendedServer.Managers.Commands
{
    public abstract class Command : ICloneable
    {
        private string c_command = "unknwon";
        private string c_permission = "HEX";
        private string c_description = "Basic Plugin";
        private string c_usage_message = "/unknwon";
        private Server server = null;
        private PluginHelper c_plugin_helper;
        private String c_from_plugin = null;
        private PluginBase c_plugin = null;
        private bool c_enabled = true;

        public String UsageMessage
        {
            get { return c_usage_message; }
            internal set { c_usage_message = value; }
        }
        public String Description
        {
            get { return c_description; }
            internal set { c_description = value; }
        }
        public String Permissions
        {
            get { return c_permission; }
            internal set { c_permission = value; }
        }
        public String Command_Name
        {
            get { return c_command; }
            internal set { c_command = value; }
        }

        public String PluginName
        {
            get { return c_from_plugin == null ? "" : c_from_plugin; }
            internal set { c_from_plugin = value; }
        }

        public PluginBase GetPlugin
        {
            get
            {
                return c_plugin;
            }
            internal set
            {
                c_plugin = value;
            }
        }

        public PluginHelper GetPluginHelper { get { return c_plugin_helper; } }

        public Server GetServer { get { return server; } }

        public Command(Server svr)
        {
            server = svr;
            
            //Console Commands skip this part becuase Plugins Are not loaded yet
            if (ServerInstance.Instance.PluginManager != null)
            {
                //Search for Plugin
                foreach (PluginInfo pi in ServerInstance.Instance.PluginManager.LoadedPlugins)
                {
                    Console.WriteLine("-----");
                    if (pi.MainClass.Name.ToLower() == PluginName.ToLower())
                    {
                        Console.WriteLine("--||||---");
                        GetPlugin = pi.MainClass;
                        break;
                    }
                    Console.WriteLine("-----");
                }
            }
            Console.WriteLine("---]]]]]]--");
            c_plugin_helper = new PluginHelper(svr);
        }

        public virtual void runCommand(Player sender, string[] args)
        {

        }

        public void ConsolerunCommand(string[] args)
        {

        }

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
