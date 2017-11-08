using HellionExtendedServer.Common.Plugins;

namespace HellionExtendedServer.Timming
{
    public class Delayed_task : ExecutableEvent
    {

        public Delayed_task(Maintimer main, int delay)
        {
            Ttype = TimerType.Delayed;
            Delay = delay;
        }
    }
}