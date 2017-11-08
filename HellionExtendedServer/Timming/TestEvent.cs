using HellionExtendedServer.Common;
using HellionExtendedServer.Managers;
using NLog;
using Octokit;

namespace HellionExtendedServer.Timming
{
    public class TestEvent : Repeating_Delayed_Task
    {
        public TestEvent() : base(20)
        {
            skipfirst = false;
        }

        public new void run(int tick)
        {
            Log.Instance.Info("Running!!!! Current Tick: "+tick);
        }
    }
}