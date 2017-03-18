using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HellionExtendedServer.Common;
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
            Log.Instance.Info("PACKET FOR 1111111111111");
            EH = eh;
            Log.Instance.Info("PACKET FOR 22222222222222");
        }

        public new void AddListener(Type group, NetworkDataDelegate function)
        {

            Log.Instance.Info("PACKET FOR dddddaaaaaaaaaaaaddddddd!");
            EH.ES2.AddListener(group, function);
        }
        
        public new void RemoveListener(Type group, NetworkDataDelegate function)
        {
            Log.Instance.Info("PACKET FOR BPOIIIIIIIII");
            EH.ES2.RemoveListener(group, function);
        }

        public void Invoke(NetworkData data)
        {
            Log.Instance.Info("PACKET FOR INVOTE!");
            EH.MassEventHandeler(data);
        }
        /*
        public new void Invoke(NetworkData data)
        {
            Log.Instance.Info("PACKET FOR INVOTE!");
            EH.MassEventHandeler(data);
        }*/

        public new void InvokeQueuedData()
        {
            Log.Instance.Info("PACKET FOR INVOasdasdasdasdasdasdsaTE2");
            EH.ES2.InvokeQueuedData();
        }

        public new void AddListener(InternalEventType group, InternalEventsDelegate function)
        {

            Log.Instance.Info("PACKET FOR ADDDDDDDDDDDDD!");
            EH.ES2.AddListener(group, function);
        }

        public new void RemoveListener(InternalEventType group, InternalEventsDelegate function)
        {
            Log.Instance.Info("PACKET FOR INVOTE2ssssss");
            EH.ES2.RemoveListener(group, function);
        }

        public new void Invoke(EventSystem.InternalEventData data)
        {
            Log.Instance.Info("PACKET FOR INVOTE2");
            EH.ES2.Invoke(data);
        }
    }
}
