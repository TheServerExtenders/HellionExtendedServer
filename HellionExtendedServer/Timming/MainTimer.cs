using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using ZeroGravity;
using ZeroGravity.Helpers;

namespace HellionExtendedServer.Timming
{
    public class Maintimer
    {
        private static int m_tick = 0;
        private static bool m_run = true;

        public static Maintimer Instance;
        private static readonly object Locker = new object();

        public static ThreadSafeList<ExecutableEvent> EList = new ThreadSafeList<ExecutableEvent>();

        public static bool Enabled
        {
            get { return m_run; }
            set { m_run = value; }
        }

        public static int CurrentTick
        {
            get { return m_tick; }
            set { m_tick = value; }
        }


        public Maintimer()
        {
            Instance = this;
        }

        public void RegisterEvent(ExecutableEvent ev)
        {
            lock (Locker)
            {
                EList.Add(ev);
            }
        }

        [STAThread]
        public static void run()
        {
            Thread.Sleep(2000);
            while (Enabled)
            {
                Thread.Sleep(50);
                //Pause for 1/20th of a second
                CurrentTick++;
                lock (Locker)
                {
                    foreach (ExecutableEvent e in EList)
                    {
                        e.pre_run();
                    }
                }
            }
        }
    }
}