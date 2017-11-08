using HellionExtendedServer.Common.Plugins;

namespace HellionExtendedServer.Timming
{
    public class Delayed_task : ExecutableEvent
    {
        public int Delay;//20 Ticks a second

        public Delayed_task(Maintimer main, int delay) :base(main)
        {

            Ttype = TimerType.Delayed;
            Delay = delay;
        }
    }
}