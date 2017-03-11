using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace HellionExtendedServer.Managers.Event
{
    public class EventListener
    {
        public SpawnPointLocationType SpawnType;
        public long SpawPointParentID;
        public GameScenes.SceneID ShipItemID;
        public readonly int ID = 1;

        public EventListener()
        {

        }

        public delegate void SpawnListnerFunction(SpawnPointLocationType spawntype,long sppid, GameScenes.SceneID shipiid);

    }
}
