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

        public EventID GetEventType { get { return EType; } }

        public EventListener(MethodInfo function, Type tt, EventID type)
        {
            Function = function;
            EType = type;
            TType = tt;
        }

        public void Execute(Event evnt)
        {
            Function.Invoke(Activator.CreateInstance(TType), new Object[]{ evnt });
        }
        
    }
}
