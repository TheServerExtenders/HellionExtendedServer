namespace HellionExtendedServer.Timming
{
    public class Repeating_Task : ExecutableEvent
    {
        
        public Repeating_Task(int delay)
        {
            skipfirst = true;
            Ttype = TimerType.Delayed;
        }

        public new void run(int tick)
        {
            base.run(tick);
        }
        
    }
}