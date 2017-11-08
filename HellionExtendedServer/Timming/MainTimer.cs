using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace HellionExtendedServer.Timming
{
    public class Maintimer
    {
        private static int m_tick = 0;
        private static bool m_run = true;
        
        public static List<ExecutableEvent> EList = new List<ExecutableEvent>();     
        public static bool Enabled => m_run;
        public static int CurrentTick => m_tick;
        
        
        public Maintimer()
        {
            run();
            RegisterEvent(new TestEvent());
        }

        public void RegisterEvent(ExecutableEvent ev)
        {
            EList.Add(ev);
        }
        
        public void run()
        {
            while (Enabled)
            {
                Thread.Sleep(50);//Pause for 1/20th of a second
                m_tick++;
                foreach (ExecutableEvent e in EList)
                {
                    e.pre_run();
                }
            }
        }
        
        [STAThread]
        public void Start()
        {
            Console.WriteLine("Starting timer Thread");
            
        }
    }
}