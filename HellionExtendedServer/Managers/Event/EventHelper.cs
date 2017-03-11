using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common;
using ZeroGravity;
using ZeroGravity.Data;
using ZeroGravity.Network;
using NetworkController = HellionExtendedServer.Controllers.NetworkController;

namespace HellionExtendedServer.Managers.Event
{

    public enum EventID
    {
        None,
        SpawnEvent,
        DeathEvent,
    };

    public class EventHelper
    {

        protected List<EventListener> RegiteredEvents = new List<EventListener>();

        public EventHelper()
        {
            //TODO Register Each Event here
            //Spawn event
            //BUG Causes Exception?!?!?!?
            //NetworkController.Instance.NetContoller.EventSystem.AddListener(typeof(PlayerSpawnRequest), new EventSystem.NetworkDataDelegate(HandelPlayerSpawnEvent));


        }

        public void HandelPlayerSpawnEvent(NetworkData data)
        {
            try
            {
                PlayerSpawnRequest playerSpawnRequest = data as PlayerSpawnRequest;


                SpawnPointLocationType SpawnType = playerSpawnRequest.SpawnType;
                long SpawPointParentID = playerSpawnRequest.SpawPointParentID;
                GameScenes.SceneID ShipItemID = playerSpawnRequest.ShipItemID;
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Hellion Extended Server [SPAWN REQUEST EVENT ERROR] : " + ex.InnerException.ToString());
            }
        }

        public void RegisterEvent(EventListener e)
        {
            RegiteredEvents.Add(e);
        }

        public void ExecuteEvent(Event e)
        {
            foreach (EventListener evnt in RegiteredEvents)
            {
                Log.Instance.Info("CHECKING "+e.GetEventType.ToString()+" =?= "+evnt.GetEventType.ToString());
                if (e.GetEventType == evnt.GetEventType)
                {
                    Log.Instance.Info("EVENTS ARE EQUAL RUNNING!");
                    evnt.Execute(e);
                    Log.Instance.Info("RAN");
                }
            }
        }
    }
}
