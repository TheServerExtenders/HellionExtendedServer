namespace HellionExtendedServer.Managers.Event.Player
{
    public class HESRespawnEvent : Event
    {

        public long GUID;

        public HESRespawnEvent(long guid) : base(EventID.DeathEvent)
        {
            GUID = guid;
        }
    }
}