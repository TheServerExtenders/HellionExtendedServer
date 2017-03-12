using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionExtendedServer.Managers.Event.Player
{
    class HESUnsubscribeFromObject : Event
    {

        public List<long> GUIDS;

        public HESUnsubscribeFromObject(List<long> guids): base(EventID.HESUnsubscribeFromObject)
        {
            GUIDS = guids;
        }
    }
}
