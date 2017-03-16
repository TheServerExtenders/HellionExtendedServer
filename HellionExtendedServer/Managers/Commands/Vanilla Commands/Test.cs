using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common.Plugins;
using HellionExtendedServer.Managers.Plugins;
using ZeroGravity;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace HellionExtendedServer.Managers.Commands.Vanilla_Commands
{
    [Permission(Description = "Unused var at the moment!",Default = "OP",PermissionName = "HES.Test.OP")]
    [Command(CommandName = "test", Description = "Testing the functionality the code", Usage = "Simply use /test", Permission = "HES.Test.OP")]
    public class Test : Command
    {
        public Test(Server svr) :  base(svr)
        {

        }

        public override void runCommand(Player sender, string[] args)
        {
            Console.WriteLine("TEST COMMAND RAN SUCCESSFULLKY!~!!!");
            GetPluginHelper.SendMessageToClient(sender, "Great TEst");
        }
    }
}
