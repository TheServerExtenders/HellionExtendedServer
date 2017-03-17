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
        private MethodInfo Function;
        private EventID EType;
        private Type TType;
        private bool IgnoreCanceled = false;

        public bool IgnoreCanceledEvent { get { return IgnoreCanceled; } }
        public EventID GetEventType { get { return EType; } }

        public EventListener(MethodInfo function, Type tt, EventID type)
        {
            Function = function;
            EType = type;
            TType = tt;
        }

        public void Execute(GenericEvent evnt)
        {
            //Todo Convert to Corret Type
            Function.Invoke(Activator.CreateInstance(TType), new Object[]{ evnt });
        }
        
    }
}
