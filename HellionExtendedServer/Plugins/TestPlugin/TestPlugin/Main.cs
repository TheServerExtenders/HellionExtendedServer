using HellionExtendedServer.Common.Plugins;
using HellionExtendedServer.Managers.Event;
using HellionExtendedServer.Managers.Event.Player;
using HellionExtendedServer.Managers.Plugins;
using System;
using ZeroGravity;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace TestPlugin
{
    [Plugin(API = "1.0.0", Author = "Yungtechboy1", Description = "Simple Test Plugin", Name = "Econ", Version = "1.0.0")]
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
            Console.WriteLine("Test Spawn Event" + hesse.ShipItemID);
        }

        public override void OnCommand(Player p, string command, string[] args)
        {
            if (command.ToLower() == "test")
            {
                GetPluginHelper.SendMessageToClient(p, "Test Command Success!");
                Console.WriteLine("TEST COMMAND SENT!");
            }
            else if (command.ToLower() == "pos")
            {
                Console.WriteLine("Your position X:{0} Y:{1} Z:{2} ", p.LocalPosition.X, p.LocalPosition.Y,
                    p.LocalPosition.Z);
                GetPluginHelper.SendMessageToClient(p,
                    String.Format("Your position X:{0} Y:{1} Z:{2} ", p.LocalPosition.X, p.LocalPosition.Y,
                        p.LocalPosition.Z));
            }
            else if (command.ToLower() == "teleport")
            {
                Tele(p, args);
            }
        }

        public void Tele(Player p, string[] args)
        {
            Console.WriteLine("RUNNING");
            Vector3D pos = new Vector3D(0, 50, 0);
            if (args.Length == 3) pos = new Vector3D(int.Parse(args[0]), int.Parse(args[1]), int.Parse(args[2]));
            p.ModifyLocalPositionAndRotation(pos, p.LocalRotation);
            GetServer.SolarSystem.SendMovementMessage();
            Console.WriteLine("DONE");
        }

        public void ModifyLocalPositionAndRotation(Player p, Vector3D pos, QuaternionD rot)
        {
            p.LocalPosition = pos;
            p.LocalRotation = rot;
            if (p.TransformDataList == null || p.TransformDataList.Count <= 0)
                return;
            p.TransformDataList[p.TransformDataList.Count - 1].LocalPosition = p.LocalPosition.ToFloatArray();
            p.TransformDataList[p.TransformDataList.Count - 1].LocalRotation = p.LocalRotation.ToFloatArray();
        }

        public int GetMoney(String player)
        {
            return 50;
        }
    }
}