using ZeroGravity.Network;

namespace HellionExtendedServer.Managers.Event.Player
{
    public class GenericEvent : Event
    {
        public EventID EID;
        public NetworkData Data;

        public GenericEvent(EventID type, NetworkData data) : base(type)
        {
            Data = data;
            EID = type;
        }
    }
}