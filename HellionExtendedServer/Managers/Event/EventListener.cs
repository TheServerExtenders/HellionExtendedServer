using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common;
using HellionExtendedServer.Managers.Event.Player;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace HellionExtendedServer.Managers.Event
{
    public class EventListener
    {
        private MethodInfo Function;

        public EventListener(MethodInfo function)
        {
            Function = function;
        }

        public void Execute(Event evnt)
        {
            Log.Instance.Debug("Starting Execute");
            Function.Invoke(this,new Object[]{ evnt });
            Log.Instance.Debug("Finished Execute");
        }

        //public delegate void SpawnListnerFunction(SpawnPointLocationType spawntype,long sppid, GameScenes.SceneID shipiid);
        public delegate void SpawnListnerFunction(HESSpawnEvent evnt);
        public delegate void ListnerFunction(Event evnt);

    }
}
