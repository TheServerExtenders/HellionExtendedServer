using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common;
using HellionExtendedServer.Managers.Event.Player;
using ZeroGravity;
using ZeroGravity.Data;
using ZeroGravity.Network;
using NetworkController = HellionExtendedServer.Controllers.NetworkController;

namespace HellionExtendedServer.Managers.Event
{
    public enum EventID
    {
        None = 0,
        SpawnEvent = 1,
        DeathEvent = 2,
        RemoveDeletedCharcters = 3,
        HESRespawnEvent = 4,
        HESSpawnObjects = 4,
        HESSubscribeToObject = 4,
        HESUnsubscribeFromObject = 4,
        HESTextChatMessage = 4,
    };
    public class EventHelper
    {
        public EventSystem ES2;
        protected List<EventListener> RegiteredEvents = new List<EventListener>();

        public EventHelper()
        {
            //DELETE ALL Events
            ES2 = NetworkController.Instance.NetContoller.EventSystem;//Copies Events
            NetworkController.Instance.NetContoller.EventSystem = new EventSystem2(this); //I listen First!
        }

        public void MassEventHandeler(NetworkData data)
        {
            //TDOD add all types
            if (data is TextChatMessage)
            {
                ExecuteEvent(new GenericEvent(EventID.HESTextChatMessage,data));
            }else if (data is PlayerSpawnRequest)
            {
                ExecuteEvent(new GenericEvent(EventID.SpawnEvent, data));
            }
        }


        public void RegisterEvent(EventListener e)
        {
            RegiteredEvents.Add(e);
            //TODO notify of Regerstration
        }

        public void ExecuteEvent(GenericEvent e)
        {
            foreach (EventListener evnt in RegiteredEvents)
            {
                if (e.GetEventType == evnt.GetEventType)
                {
                    if (!e.IsCanceled || e.IsCanceled && evnt.IgnoreCanceledEvent)
                    {
                        evnt.Execute(e);
                    }
                }
            }
            if(e.IsCanceled)return;
            //TODO send the message to server listeners
            ES2.Invoke(e.Data);//Now I will send info to server XD
        }
    }
}