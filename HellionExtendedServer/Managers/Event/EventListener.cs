using HellionExtendedServer.Managers.Event.Player;
using System;
using System.Reflection;

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
            Function.Invoke(Activator.CreateInstance(TType), new Object[] { evnt });
        }
    }
}