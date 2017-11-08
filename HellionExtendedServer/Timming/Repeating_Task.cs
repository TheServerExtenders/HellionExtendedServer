namespace HellionExtendedServer.Timming
{
    public class Repeating_Task : ExecutableEvent
    {
        
        public Repeating_Task(int delay)
        {
            skipfirst = true;
            Ttype = TimerType.Repeating_Task;
        }

        public new void run(int tick)
        {
            base.run(tick);
        }
        
    }
}