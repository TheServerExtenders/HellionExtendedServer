using ZeroGravity;
using ZeroGravity.Objects;

namespace HellionExtendedServer.Managers.Commands.Vanilla_Commands
{
    [Permission(Default = "op", PermissionName = "HES.OP.DELPERM")]
    [Command(CommandName = "delperm", Description = "Add permission node to player", Usage = "Usage: /addperm <player> <permission.node.data>", Permission = "HES.OP.DELPERM")]
    public class DelPerms : Command
    {
        public DelPerms(Server svr) : base(svr)
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
            p.DelPerm(args[1]);
            ServerInstance.Instance.PermissionManager.SetPlayerPermission(p);
            GetPluginHelper.SendMessageToClient(sender, "Success! " + args[1] + " was removed from" + target.Name + "'s permission!");
        }

        public override void ConsolerunCommand(string[] args)
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
            p.DelPerm(args[1]);
            ServerInstance.Instance.PermissionManager.SetPlayerPermission(p);
            GetPluginHelper.GetLogger.Info("Success! " + args[1] + " was removed from" + target.Name + "'s permission!");
        }
    }
}