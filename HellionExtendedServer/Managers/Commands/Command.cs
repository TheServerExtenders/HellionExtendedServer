﻿using System;
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

            ReloadPlugin();

            c_plugin_helper = new PluginHelper(svr);
        }

        public virtual void runCommand(Player sender, string[] args)
        {

        }

        /**
         * Gets Main calss associated with Command
         */
        public void ReloadPlugin()
        {
            //Console Commands skip this part becuase Plugins Are not loaded yet
            if (ServerInstance.Instance.PluginManager != null)
            {
                //Search for Plugin
                foreach (PluginInfo pi in ServerInstance.Instance.PluginManager.LoadedPlugins)
                {
                    //Console.WriteLine(pi.MainClass.GetName.ToLower() + "-----" + PluginName.ToLower());
                    if (pi.MainClass.GetName.ToLower() == PluginName.ToLower())
                    {
                        GetPlugin = pi.MainClass;
                        break;
                    }
                }
            }
        }
        public virtual void ConsolerunCommand(string[] args)
        {

        }

        public static String GetCommandPermission(Type commandtype)
        {
            foreach (Attribute attribute in commandtype.GetCustomAttributes(true))
            {
                if (attribute is CommandAttribute)
                {
                    CommandAttribute ca = attribute as CommandAttribute;
                    //Add To plugin
                    //Onplayer Join Event Add Default Perms to player
                    return ca.Permission;
                }
            }
            return null;
        }
        public static CommandAttribute GetCommandAttribute(Type commandtype)
        {
            foreach (Attribute attribute in commandtype.GetCustomAttributes(true))
            {
                if (attribute is CommandAttribute)return attribute as CommandAttribute;
            }
            return null;
        }

        #region ICloneable Members

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion
    }
}
