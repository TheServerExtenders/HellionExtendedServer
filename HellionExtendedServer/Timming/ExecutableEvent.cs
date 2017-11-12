using System;
using HellionExtendedServer.Common.Plugins;

namespace HellionExtendedServer.Timming
{
    public enum TimerType
    {
        Delayed_Repeating_Task,
        Repeating_Task,
        Delayed
    }

    public class ExecutableEvent
    {
        public Maintimer Main;
        public string PluginName;
        public PluginBase Plugin;
        public TimerType Ttype;
        private long LastRun = 0;
        protected bool skipfirst = false;
        public int Delay; //20 Ticks a second


        public ExecutableEvent()
        {
            LastRun = Maintimer.CurrentTick;
        }

        public void pre_run()
        {
            if ( Maintimer.CurrentTick >= LastRun + Delay)
            {
                LastRun = Maintimer.CurrentTick;
                if (Ttype == TimerType.Delayed)
                {
                    run(Maintimer.CurrentTick);
                    RemovetimerEvent();
                }
                else if (Ttype == TimerType.Repeating_Task)run(Maintimer.CurrentTick);
                else if (Ttype == TimerType.Delayed_Repeating_Task)
                {
                    if (skipfirst)
                    {
                        skipfirst = false;
                        return;
                    }
                    run(Maintimer.CurrentTick);
                }
            }
        }

        public void RemovetimerEvent()
        {
            if (Maintimer.EList.Contains(this)) Console.WriteLine("CONNNTTTAAAIIINNNNSSS!!!!!!!!!");
            Maintimer.EList.Remove(this);
        }

        public virtual void run(int tick)
        {
            
        }
    }
}