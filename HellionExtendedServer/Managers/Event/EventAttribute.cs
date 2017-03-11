using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionExtendedServer.Managers.Event
{
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class HESEventAttribute : Attribute
    {
        public HESEventAttribute(EventID id)
        {
            EventType = id;
        }
        public EventID EventType;
    }
}
