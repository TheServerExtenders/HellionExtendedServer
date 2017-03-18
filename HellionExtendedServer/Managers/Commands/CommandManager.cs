using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BulletSharp.SoftBody;
using HellionExtendedServer.Common;
using HellionExtendedServer.Common.Plugins;
using HellionExtendedServer.Managers.Commands.Vanilla_Commands;
using HellionExtendedServer.Managers.Plugins;
using ZeroGravity;
using ZeroGravity.Objects;

namespace HellionExtendedServer.Managers.Commands
{
    public class CommandManager
    {
        private Dictionary<string, Type> commandDictionary = new Dictionary<string, Type>();

        public Dictionary<string, Type> GetCommandDictionary()
        {
            return commandDictionary;
        }

        public void AddCommand(Command cmdclass, PluginBase plugin)
        {
            if (cmdclass == null) return;
            //TODO Notify of Override
            if (!cmdclass.GetType().IsDefined(typeof(CommandAttribute), true))
            {
                Console.WriteLine("Error Loading Command! Missing Correct Syntax! Command : " +
                                  cmdclass.GetType().FullName);
                return;
            }
            PermissionAttribute pa = Attribute.GetCustomAttribute(cmdclass.GetType(), typeof(PermissionAttribute), true) as PermissionAttribute;
            if (pa != null)
            {
                    ServerInstance.Instance.PermissionManager.AddPermissionAttribute(pa);
            }
            CommandAttribute pluginAttribute = Attribute.GetCustomAttribute(cmdclass.GetType(), typeof(CommandAttribute), true) as CommandAttribute;
            if (pluginAttribute != null)
            {
                cmdclass.Command_Name = pluginAttribute.CommandName;
                cmdclass.Description = pluginAttribute.Description;
                cmdclass.UsageMessage = pluginAttribute.Usage;
                cmdclass.Permissions = pluginAttribute.Permission;
                cmdclass.PluginName = pluginAttribute.Plugin;
                cmdclass.ReloadPlugin();
            }
            else //MAYBE if (cmdclass.Permissions == null || cmdclass.Command_Name == null || cmdclass.Description == null || cmdclass.UsageMessage == null)//Skip Console Commands!
            {
                Console.WriteLine("Error Loading Command! Error with Syntax! Command : " + cmdclass.GetType().FullName);
                return;
            }
            Console.WriteLine("Loaded Command /" + cmdclass.Command_Name);
            commandDictionary[cmdclass.Command_Name] =  cmdclass.GetType();
        }
        public void AddCommand(Command cmdclass)
        {
            AddCommand(cmdclass, null);
        }
        public void RemoveCommand(String cmdclass)
        {
            if (cmdclass == null) return;
            commandDictionary.Remove(cmdclass);
        }

        public bool HandleConsoleCommand(string cmd, string[] args)
        {
            Console.WriteLine(String.Format("Handeling Console Cmd /{0} with arge: {1}", cmd, String.Join(" ", args)));
            //TODO check Permmissions
            if (!commandDictionary.ContainsKey(cmd)) return false;
            Command c = (Command)Activator.CreateInstance(commandDictionary[cmd], new object[] { ServerInstance.Instance.Server });
            if (c == null) return false;
            /*CommandAttribute pluginAttribute = Attribute.GetCustomAttribute(c.GetType(), typeof(CommandAttribute), true) as CommandAttribute;
            if (pluginAttribute != null)
            {
                c.Command_Name = pluginAttribute.CommandName;
                c.Description = pluginAttribute.Description;
                c.UsageMessage = pluginAttribute.Usage;
                c.Permissions = pluginAttribute.Permission;
                c.PluginName = pluginAttribute.Plugin;
                c.ReloadPlugin();
            }*/
            c.ConsolerunCommand(args);
            Log.Instance.Debug("Console Command Ran!");
            return true;
        }
        public void HandlePlayerCommand(string cmd, string[] args, Player sender)
        {
            Console.WriteLine(String.Format("Handeling String /{0} with arge: {1}", cmd, String.Join(" ",args)));
            //TODO check Permmissions
            if (!commandDictionary.ContainsKey(cmd)) return;
            Command c = (Command)Activator.CreateInstance(commandDictionary[cmd], new object[] { ServerInstance.Instance.Server});
            if (c == null) return;
            CommandAttribute pluginAttribute = Attribute.GetCustomAttribute(c.GetType(), typeof(CommandAttribute), true) as CommandAttribute;
            if (pluginAttribute != null)
            {
                c.Command_Name = pluginAttribute.CommandName;
                c.Description = pluginAttribute.Description;
                c.UsageMessage = pluginAttribute.Usage;
                c.Permissions = pluginAttribute.Permission;
                c.PluginName = pluginAttribute.Plugin;
                c.ReloadPlugin();
            }
            //TODO Send error!
            //Checking Perms
            //Todo make sure this is working correctly
            if (!ServerInstance.Instance.PermissionManager.PlayerHasPerm(sender,c.Permissions))
            {
                //Error!
                new PluginHelper(Server.Instance).SendMessageToClient(sender,"Error! you do not have permission to access that command!");
                return;
            }
            c.runCommand(sender, args);
        }

        public CommandManager()
        {
            AddCommand(new Test(ServerInstance.Instance.Server));
            AddCommand(new Status(ServerInstance.Instance.Server));
            AddCommand(new Help(ServerInstance.Instance.Server));
            AddCommand(new Help2(ServerInstance.Instance.Server));
            AddCommand(new AddPerms(ServerInstance.Instance.Server));
            AddCommand(new DelPerms(ServerInstance.Instance.Server));
        }

    }
}
