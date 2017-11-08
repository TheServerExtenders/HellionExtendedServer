namespace HellionExtendedServer.Timming
{
    public class Repeating_Delayed_Task : ExecutableEvent
    {

        public Repeating_Delayed_Task(int delay)
        {
            skipfirst = true;
            Ttype = TimerType.Delayed_Repeating_Task;
            Delay = delay;
        }

        public new void run(int tick)
        {
            base.run(tick);
        }
        
        
    }
}