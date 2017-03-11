using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace HellionExtendedServer.Managers.Event.Player
{
    public class HESSpawnEvent : Event
    {
        public SpawnPointLocationType SpawnType;
        public long SpawPointParentID;
        public GameScenes.SceneID ShipItemID;
        public readonly int ID = 1;

        public HESSpawnEvent(SpawnPointLocationType st, long sppid, GameScenes.SceneID ShipIID)
        {
            SpawnType = st;
            SpawPointParentID = sppid;
            ShipItemID = ShipIID;
        }
    }
}
