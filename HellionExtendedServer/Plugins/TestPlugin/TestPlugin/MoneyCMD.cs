using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Managers.Commands;
using HellionExtendedServer.Managers.Plugins;
using ZeroGravity;
using ZeroGravity.Objects;

namespace TestPlugin
{
    [Command(CommandName = "money",Description = "View your money",Usage = "/money [player]",Permission = "CyberCore.Money")]
    public class MoneyCmd : Command
    {
        
        public MoneyCmd(Server svr) :  base(svr)
        {

        }

        public override void runCommand(Player sender, string[] args)
        {
            if (GetPlugin is PluginMain)
            {
                PluginMain pm = (PluginMain) GetPlugin;
                GetPluginHelper.SendMessageToClient(sender, "You Have "+pm.GetMoney(sender.Name));

            }
            Console.WriteLine("TEST COMMAND RAN SUCCESSFULLKY!~!!!");
            GetPluginHelper.SendMessageToClient(sender, "Great TEst");
        }
    }
}
