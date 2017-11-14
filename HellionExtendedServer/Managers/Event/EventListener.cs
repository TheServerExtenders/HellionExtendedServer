using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common;
using HellionExtendedServer.Managers.Event.Player;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace HellionExtendedServer.Managers.Event
{
    public class EventListener
    {
        private MethodInfo Function = null;
        private EventSystem.NetworkDataDelegate Delegate = null;
        private EventID EType;
        private Type TType;
        private bool IgnoreCanceled = false;

        public bool IgnoreCanceledEvent { get { return IgnoreCanceled; } }
        public EventID GetEventType { get { return EType; } }

        public EventListener(EventSystem.NetworkDataDelegate dataDelegate, EventID type)
        {
            Delegate = dataDelegate;
            EType = type;
        }
        public EventListener(MethodInfo function, Type tt, EventID type)
        {
            Function = function;
            EType = type;
            TType = tt;
        }

        public void Execute(GenericEvent evnt)
        {
            //Todo Convert to Corret Type
            if(Function != null)Function.Invoke(Activator.CreateInstance(TType), new Object[]{ evnt });
            if(Delegate.GetInvocationList().Length > 0)Delegate(evnt.Data);
            //this.networkDataGroups[data.GetType()](data);
        }
        
    }
}
