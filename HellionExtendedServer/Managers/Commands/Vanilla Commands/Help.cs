using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common;
using HellionExtendedServer.Managers;
using HellionExtendedServer.Managers.Commands;
using HellionExtendedServer.Managers.Plugins;
using NLog;
using ZeroGravity;
using ZeroGravity.Objects;

namespace HellionExtendedServer.Managers.Commands.Vanilla_Commands
{
    [Permission(Default = "default", PermissionName = "HES.help")]
    [Command(CommandName = "help", Description = "Returns the current status of the server",
        Usage = "Usage: /help [page]",
        Permission = "HES.help")]
    public class Help : Command
    {

        public Help(Server svr) : base(svr)
        {

        }

        public override void ConsolerunCommand(string[] args)
        {
            Log.Instance.Info("asdas asd asd!");
            Dictionary<string, Type> cd = ServerInstance.Instance.CommandManager.GetCommandDictionary();
            List<String> list = cd.Keys.ToList();
            list.Sort();

            List<CommandAttribute> commands = new List<CommandAttribute>();
            foreach (String cmd in list)
            {
                Type t = cd[cmd];
                CommandAttribute ca = Command.GetCommandAttribute(t);
                commands.Add(ca);
            }

            GetPluginHelper.GetLogger.Info(String.Format("==== Help Page ===="));

            int i = 1;
            foreach (CommandAttribute command1 in commands)
            {
                GetPluginHelper.GetLogger.Info("/" + command1.CommandName + ": " + command1.Description);
            }
        }

        public override void runCommand(Player sender, string[] args)
        {
            Dictionary<string, Type> cd = ServerInstance.Instance.CommandManager.GetCommandDictionary();
            List<String> list = cd.Keys.ToList();
            list.Sort();
            int pageNumber = 1;
            int pageHeight = 5;

            if (args.Length != 0) int.TryParse(args[0], out pageNumber);

            List<CommandAttribute> commands = new List<CommandAttribute>();
            foreach (String cmd in list)
            {
                Type t = cd[cmd];
                CommandAttribute ca = Command.GetCommandAttribute(t);
                String perm = ca.Permission;
                if (perm != null && !ServerInstance.Instance.PermissionManager.PlayerHasPerm(sender, perm))
                    continue;
                commands.Add(ca);
            }
            int totalPage = commands.Count % pageHeight == 0
                ? commands.Count / pageHeight
                : commands.Count / pageHeight + 1;
            pageNumber = Math.Min(pageNumber, totalPage);
            if (pageNumber < 1) pageNumber = 1;

            GetPluginHelper.SendMessageToClient(sender,
                String.Format("==== Help Page {0} of {1}", pageNumber, totalPage));

            int i = 1;
            foreach (CommandAttribute command1 in commands)
            {
                if (i >= (pageNumber - 1) * pageHeight + 1 && i <= Math.Min(commands.Count, pageNumber * pageHeight))
                {
                    GetPluginHelper.SendMessageToClient(sender,
                        "/" + command1.CommandName + ": " + command1.Description);
                }
                i++;
            }
        }
    }
}

