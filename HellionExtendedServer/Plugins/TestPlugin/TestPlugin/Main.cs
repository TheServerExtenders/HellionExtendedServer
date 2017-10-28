using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common.Plugins;
using HellionExtendedServer.Managers;
using HellionExtendedServer.Managers.Event;
using HellionExtendedServer.Managers.Event.Player;
using HellionExtendedServer.Managers.Plugins;
using ZeroGravity;
using ZeroGravity.Helpers;
using ZeroGravity.Math;
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
            else if (command.ToLower() == "tp2")
            {
                MovementMessage movementMessage1 = new MovementMessage();
                movementMessage1.SolarSystemTime = GetServer.SolarSystem.CurrentTime;
                movementMessage1.Timestamp = (float) Server.Instance.RunTime.TotalSeconds;
                movementMessage1.Transforms = new List<ObjectTransform>();
                ObjectTransform objectTransform = new ObjectTransform();
                ModifyLocalPositionAndRotation(p, new Vector3D(50,50,50),new QuaternionD(50,50,50,0) );
                CharacterMovementMessage characterMovementMessage = p.GetCharacterMovementMessage();
                if (characterMovementMessage != null)
                    objectTransform.CharactersMovement.Add(characterMovementMessage);
                movementMessage1.Transforms.Add(objectTransform);
                p.LastMovementMessageSolarSystemTime = GetServer.SolarSystem.CurrentTime;
                p.UpdateArtificialBodyMovement.Clear();
                Server.Instance.NetworkController.SendToGameClient(p.GUID, movementMessage1);
                
            }
            else if (command.ToLower() == "tp")
            {
                p.LocalPosition.X = 10;
                p.LocalPosition.Z = 10;
                p.LocalPosition.Y = 10;
                GetPluginHelper.SendMessageToClient(p, "Your current Postion to string " + p.LocalPosition.ToString());
                long guid = p.GUID;
                SpawnObjectsResponse spawnObjectsResponse = new SpawnObjectsResponse();
                spawnObjectsResponse.Data.Add(p.GetSpawnResponseData(p));
                GetServer.NetworkController.SendToGameClient(guid, spawnObjectsResponse);
                GetPluginHelper.SendMessageToClient(p, "Your current Postion to string " + p.LocalPosition.ToString());
            }
        }
        
        public void ModifyLocalPositionAndRotation(Player p, Vector3D locPos, QuaternionD locRot)
        {
            p.LocalPosition = p.LocalPosition + locPos;
            p.LocalRotation = p.LocalRotation * locRot;
            p.TransformDataList.Clear();
            p.TransformDataList[0].LocalPosition = p.LocalPosition.ToFloatArray();
            p.TransformDataList[0].LocalRotation = p.LocalRotation.ToFloatArray();

        }

        public int GetMoney(String player)
        {
            return 50;
        }
    }
}
