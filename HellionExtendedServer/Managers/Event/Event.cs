using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


    }
}
