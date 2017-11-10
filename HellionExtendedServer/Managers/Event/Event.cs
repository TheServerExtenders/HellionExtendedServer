using System;

namespace HellionExtendedServer.Managers.Event
{
    public class Event
    {
        private EventID Type;

        private Boolean canceled = false;

        public EventID GetEventType { get { return Type; } }
        public Boolean IsCanceled { get { return canceled; } }

        public Event(EventID type)
        {
            Type = type;
        }

        public virtual void PreRun()
        {
        }

        public virtual void Run()
        {
        }

        public virtual void PostRun()
        {
        }
    }
}