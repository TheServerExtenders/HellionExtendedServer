using HellionExtendedServer.Managers.Event.Player;
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