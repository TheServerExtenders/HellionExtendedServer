using HellionExtendedServer.Common;
using HellionExtendedServer.Managers;
using HellionExtendedServer.Managers.Plugins;
using NLog;
using Octokit;
using ZeroGravity;

namespace HellionExtendedServer.Timming
{
    public class TestEvent : Repeating_Delayed_Task
    {
        public TestEvent() : base(20*45)//Every 45 Seconds
        {
            skipfirst = false;
        }

        public override void run(int tick)
        {
            new PluginHelper(Server.Instance).SendMessageToServer("We Are Proudly Running Hellion Extended Server!");
        }
    }
}