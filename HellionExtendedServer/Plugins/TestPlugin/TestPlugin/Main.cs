using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common.Plugins;
using HellionExtendedServer.Managers;
using HellionExtendedServer.Managers.Event;
using HellionExtendedServer.Managers.Event.Player;
using HellionExtendedServer.Managers.Plugins;
using ZeroGravity.Network;
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

        //Will Only send Events that Are Releated to this attribute below!
        [HESEvent(EventType = EventID.PlayerSpawnRequest)]
        public void TestSpawnEvent(GenericEvent evnt)
        {
            PlayerSpawnRequest hesse = evnt.Data as PlayerSpawnRequest;
            Console.WriteLine("Test Spawn Event"+ hesse.ShipItemID);
        }

        public override void OnCommand(Player p, string command, string[] args)
        {
            Console.WriteLine("OVERRIDE!!!!!!!");
            if (command.ToLower() == "test")
            {
                GetPluginHelper.SendMessageToClient(p, "Test Command Success!");
                Console.WriteLine("TEST COMMAND SENT1!");
            }else if (command.ToLower() == "pos")
            {
                GetPluginHelper.SendMessageToClient(p, "Your current Postion to string " + p.LocalPosition.ToString());
            }
            else if (command.ToLower() == "tp")
            {
                p.LocalPosition.X = 10;
                p.LocalPosition.Z = 10;
                p.LocalPosition.Y = 10;
                GetPluginHelper.SendMessageToClient(p, "Your current Postion to string " + p.LocalPosition.ToString());
                long guid = p.GUID;
                SpawnObjectsResponse spawnObjectsResponse = new SpawnObjectsResponse();
                spawnObjectsResponse.Data.Add(p.GetSpawnResponseData((Player)null));
                GetServer.NetworkController.SendToGameClient(guid, spawnObjectsResponse);
                GetPluginHelper.SendMessageToClient(p, "Your current Postion to string " + p.LocalPosition.ToString());
            }
        }

        public int GetMoney(String player)
        {
            return 50;
        }
    }
}
