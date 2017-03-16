using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Managers.Plugins;
using ZeroGravity;
using ZeroGravity.Objects;

namespace HellionExtendedServer.Managers.Commands.Vanilla_Commands
{
    [Permission(Default = "op",PermissionName = "HES.OP.ADDPERM")]
    [Command(CommandName = "addperm", Description = "Add permission node to player", Usage = "Usage: /addperm <player> <permission.node.data>", Permission = "HES.OP.ADDPERM")]
    public class AddPerms : Command
    {

        public AddPerms(Server svr) : base(svr)
        {

        }

        public override void runCommand(Player sender, string[] args)
        {
            if (args.Length != 2)
            {
                GetPluginHelper.SendMessageToClient(sender, "Error! Format /addperm <player> <permission.node.data>");
                return;
            }
            Player target = GetPluginHelper.GetPlayer(args[0]);
            if (target == null)
            {
                GetPluginHelper.SendMessageToClient(sender, "Error! Player not found!");
                return;
            }
            Permission p = ServerInstance.Instance.PermissionManager.GetPlayerPermission(target);
            p.AddPerm(args[1]);
            ServerInstance.Instance.PermissionManager.SetPlayerPermission(p);
            GetPluginHelper.SendMessageToClient(sender,"Success! "+args[1]+" to "+target.Name+"'s permission!");
        }


        public void ConsolerunCommand(string[] args)
        {
            if (args.Length != 2)
            {
                GetPluginHelper.GetLogger.Error("Error! Format /addperm <player> <permission.node.data>");
                return;
            }
            Player target = GetPluginHelper.GetPlayer(args[0]);
            if (target == null)
            {
                GetPluginHelper.GetLogger.Error("Error! Player not found!");
                return;
            }
            Permission p = ServerInstance.Instance.PermissionManager.GetPlayerPermission(target);
            p.AddPerm(args[1]);
            ServerInstance.Instance.PermissionManager.SetPlayerPermission(p);
        }

    }
}
