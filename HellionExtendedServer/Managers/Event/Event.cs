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

        public EventID GetEventType { get { return Type; } }

        public Event(EventID type)
        {
            Type = type;
        }

    }
}
