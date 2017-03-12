using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HellionExtendedServer.Managers.Event.Player;
using ZeroGravity;
using ZeroGravity.Helpers;
using ZeroGravity.Network;

namespace HellionExtendedServer.Managers.Event
{
    public class EventSystem2 : EventSystem
    {
        private EventHelper EH;
        public EventSystem2(EventHelper eh)
        {
            EH = eh;
        }

        public void AddListener(Type group, EventSystem.NetworkDataDelegate function)
        {
            EH.ES2.AddListener(group, function);
        }

        public void RemoveListener(Type group, EventSystem.NetworkDataDelegate function)
        {
            EH.ES2.RemoveListener(group, function);
        }

        public void Invoke(NetworkData data)
        {
            EH.MassEventHandeler(data);
        }

        public void InvokeQueuedData()
        {
            EH.ES2.InvokeQueuedData();
        }

        public void AddListener(EventSystem.InternalEventType group, EventSystem.InternalEventsDelegate function)
        {
            EH.ES2.AddListener(group, function);
        }

        public void RemoveListener(EventSystem.InternalEventType group, EventSystem.InternalEventsDelegate function)
        {
            EH.ES2.RemoveListener(group, function);
        }

        public void Invoke(EventSystem.InternalEventData data)
        {
            EH.ES2.Invoke(data);
        }

        public class InternalEventData
        {
            public EventSystem.InternalEventType Type;
            public object[] Objects;

            public InternalEventData(EventSystem.InternalEventType type, params object[] objects)
            {
                this.Type = type;
                this.Objects = objects;
            }
        }

        public delegate void NetworkDataDelegate(NetworkData data);

        public delegate void InternalEventsDelegate(EventSystem.InternalEventData data);

        public enum InternalEventType
        {
            GetPlayer,
        }
    }
}
