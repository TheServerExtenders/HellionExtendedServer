using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HellionExtendedServer.Managers.Event.Player
{
    class HESTextChatMessage : Event
    {
        public long GUID;
        public bool Local;
        public string Name;
        public string MessageText;

        public HESTextChatMessage(long guid, bool local, string name, string msg): base(EventID.DeathEvent)
        {
            GUID = guid;
            Local = local;
            Name = name;
            MessageText = msg;
        }
    }
}
