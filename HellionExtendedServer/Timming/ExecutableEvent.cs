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


        public ExecutableEvent(Maintimer m)
        {
            Main = m;
        }

        public void pre_run()
        {
            if (Ttype == TimerType.Delayed)
            {
                run();
                RemovetimerEvent();
            }
            else if (Ttype == TimerType.Repeating_Task)
            {
                Delayed_task task = (Delayed_task) this;
                if (LastRun < Maintimer.CurrentTick + task.Delay)
                {
                    LastRun = Maintimer.CurrentTick;
                    run();
                }
            }
            else if (Ttype == TimerType.Delayed_Repeating_Task)
            {
                run();
            }
        }

        public void RemovetimerEvent()
        {
            if(Maintimer.EList.Contains(this))Console.WriteLine("CONNNTTTAAAIIINNNNSSS!!!!!!!!!");
            Maintimer.EList.Remove(this);
        }
        
        public virtual void run()
        {
        }
    }
}