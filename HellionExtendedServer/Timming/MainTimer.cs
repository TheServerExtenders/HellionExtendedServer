using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using ZeroGravity.Helpers;

namespace HellionExtendedServer.Timming
{
    public class Maintimer
    {
        private static int m_tick = 0;
        private static bool m_run = true;

        public Maintimer Instance;
        
        public static ThreadSafeList<ExecutableEvent> EList = new ThreadSafeList<ExecutableEvent>();     
        public static bool Enabled => m_run;
        public static int CurrentTick => m_tick;
        
        
        public Maintimer()
        {
            Instance = this;
        }

        public void RegisterEvent(ExecutableEvent ev)
        {
            EList.Add(ev);
        }
        
        [STAThread]
        public static void run()
        {
            while (Enabled)
            {
                Thread.Sleep(50);
                //Pause for 1/20th of a second
                m_tick++;
                foreach (ExecutableEvent e in EList)
                {
                    e.pre_run();
                }
            }
        }
    }
}