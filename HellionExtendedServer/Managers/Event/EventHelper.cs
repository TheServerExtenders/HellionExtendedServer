using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common;
using HellionExtendedServer.Managers.Event.Player;
using ZeroGravity;
using ZeroGravity.Data;
using ZeroGravity.Helpers;
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
            ES2 = NetworkController.Instance.NetContoller.EventSystem;//Copies Events
            
            //Unregister All Listeners
            //Get All Listeners to Register
            ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate> networkDataGroups = GetCurrentListenersNetwork();
            if (networkDataGroups == null)
            {
                Log.Instance.Error("Error starting EventHandeler! Could not find all events!");
                return;
            }
            List<Type> AddedTypes = new List<Type>();
            foreach (KeyValuePair<Type, EventSystem.NetworkDataDelegate> entry in networkDataGroups)
            {
                if(AddedTypes.Contains(entry.Key))continue;
                AddedTypes.Add(entry.Key);
                NetworkController.Instance.NetContoller.EventSystem.AddListener(entry.Key, MassEventHandeler);
                //Listen for Everything!
            }
            
        }

        public ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate>
            GetCurrentListenersNetwork()
        {
            //HACK
            try
            {
                BindingFlags bf = BindingFlags.Instance | BindingFlags.NonPublic;
                FieldInfo mi =
                    NetworkController.Instance.NetContoller.EventSystem.GetType().GetField("networkDataGroups", bf);
                ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate> a =
                    (ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate>)
                    mi.GetValue(NetworkController.Instance.NetContoller.EventSystem);
                ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate> b =
                    new ThreadSafeDictionary<Type, EventSystem.NetworkDataDelegate>();
                foreach (KeyValuePair<Type, EventSystem.NetworkDataDelegate> entry in a)
                {
                    b.Add(entry.Key,entry.Value);
                }
                return b;
            }
            catch (Exception Ex)
            {
                Log.Instance.Info("ERROR!" + Ex.ToString());
            }

            return null;

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
        }
    }
}