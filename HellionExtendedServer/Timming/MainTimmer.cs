using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;

namespace HellionExtendedServer.Timming
{
    public class MainTimmer
    {
        private static int m_tick = 0;
        private static bool m_run = true;
        private List<ExecutableEvent> = 
            
        public static bool Enabled => m_run;
        public static int CurrentTick => m_tick;
        
        
        public MainTimmer()
        {
            
        }

        public void run()
        {
            while (Enabled)
            {
                Thread.Sleep(100);//Pause for 1/10th of a second
            }
        }
        
        [STAThread]
        public void Start()
        {
            Console.WriteLine("Starting Timmer Thread");
        }
    }
}