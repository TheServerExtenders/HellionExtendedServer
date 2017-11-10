using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common;
using HellionExtendedServer.Managers.Event.Player;
using ZeroGravity;
using ZeroGravity.Network;

namespace HellionExtendedServer.Managers.Event.ServerEvents
{
    public class ServerJoinEvent : GenericEvent
    {
        public ServerJoinEvent(EventID type, NetworkData data) : base(type, data)
        {
        }

        [HESEvent(EventType = EventID.PlayerSpawnRequest)]
        public void PlayerSpawnRequest(GenericEvent evnt)
        {
            PlayerSpawnRequest data = evnt.Data as PlayerSpawnRequest;
            //Check if Permissions are Loaded
            ServerInstance.Instance.PermissionManager.GetPermission(data.Sender);
        }
    }
}
