using HellionExtendedServer.Common.Plugins;

namespace HellionExtendedServer.Timming
{
    public enum TimerType
    {
        Delayed_Task,
        Repeating_Task,
        Delayed
        
    }

    public class ExecutableEvent
    {
        public string PluginName;
        public PluginBase Plugin;
        public TimerType Ttype;


        public virtual void run()
        {
            if (Ttype == TimerType.Delayed)
            {
                
            } 
        }
            
            
    }
}