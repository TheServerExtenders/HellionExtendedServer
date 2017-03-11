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
    [Command(CommandName = "status", Description = "Returns the current status of the server", Usage = "Usage: /status", Permission = "HES.Status")]
    public class Status : Command
    {

        public Status(Server svr) : base(svr)
        {

        }

        public override void runCommand(Player sender, string[] args)
        {
            Console.WriteLine("TESTTTTT");
            try
            {
                var status = new List<string>()
                {
                    String.Format("~~ Status of {0} ~~", GetServer.ServerName),
                    String.Format("Server Started on {0] and has been running for {1} Days and {2} Hours",
                        GetServer.ServerStartTime, GetServer.RunTime.Days, GetServer.RunTime.Hours),
                    String.Format("Players Online: {0}/{1} | Tick Rate: {1}",
                        GetServer.NetworkController.CurrentOnlinePlayers(), GetServer.MaxPlayers),
                    String.Format("Server Object Count: {0} | Loaded Plugins Count: {1}", GetServer.AllVessels.Count,
                        GetServer.TickMilliseconds, ServerInstance.Instance.PluginManager.LoadedPlugins.Count),
                };

                Console.WriteLine("TESTTTTT");
                status.ForEach((line) => GetPluginHelper.SendMessageToClient(sender, line));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        
    }
}
