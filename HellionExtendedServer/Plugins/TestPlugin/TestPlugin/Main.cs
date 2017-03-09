using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common.Plugins;
using HellionExtendedServer.Managers.Plugins;
using ZeroGravity.Objects;

namespace TestPlugin
{
    [Plugin(API = "1.0.0",Author = "Yungtechboy1", Description = "Simple Test Plugin", Name = "Econ",Version = "1.0.0")]
    public class PluginMain : PluginBase
    {
        public PluginMain()
        {

        }

        public override void OnEnable()
        {
            Console.WriteLine("Test Command Enabled");
        }

        public override void OnCommand(Player p, string command, string[] args)
        {
            if (command.ToLower() == "test")
            {
                GetPluginHelper.SendMessageToClient(p, "Test Command Success!");
                Console.WriteLine("TEST COMMAND SENT!");
            }
        }

        public int GetMoney(String player)
        {
            return 50;
        }
    }
}
