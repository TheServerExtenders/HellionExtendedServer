namespace HellionExtendedServer.Timming
{
    public class Repeating_Delayed_Task : ExecutableEvent
    {
        public int Delay;//20 Ticks a second

        public Repeating_Delayed_Task(int delay)
        {
            skipfirst = true;
            Ttype = TimerType.Delayed;
            Delay = delay;
        }

        public new void run(int tick)
        {
            base.run(tick);
        }
        
        
    }
}