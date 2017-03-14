using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Managers.Event.Player;
using ZeroGravity;
using ZeroGravity.Network;

namespace HellionExtendedServer.Managers.Event.ServerEvents
{
    public class JoinEvent
    {
        [HESEvent(EventType = EventID.PlayerSpawnRequest)]
        public void TestSpawnEvent(GenericEvent evnt)
        {
            PlayerSpawnRequest data = evnt.Data as PlayerSpawnRequest;
            //Check if Permissions are Loaded
            ServerInstance.Instance.PermissionManager.GetPermission(data.Sender);
        }
    }
}
