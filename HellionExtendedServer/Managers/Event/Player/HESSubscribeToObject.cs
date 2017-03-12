using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionExtendedServer.Managers.Event.Player
{
    class HESSubscribeToObject : Event
    {

        public List<long> GUIDS;

        public HESSubscribeToObject(List<long> guids): base(EventID.HESSpawnObjects)
        {
            GUIDS = guids;
        }
    }
}
