using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionExtendedServer.Managers.Event.Player
{
    class HESSpawnObjects : Event
    {
        public List<long> GUIDS;

        public HESSpawnObjects(List<long> guids): base(EventID.HESSpawnObjects)
        {
            GUIDS = guids;
        }
    }
}
