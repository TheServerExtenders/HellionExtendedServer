using HellionExtendedServer.Common.Plugins;

namespace HellionExtendedServer.Timming
{
    public class Delayed_task : ExecutableEvent
    {
        public int Delay;

        public Delayed_task(int delay)
        {
            Delay = delay;
        }
    }
}