using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace HellionExtendedServer.Managers.Event.Player
{
    class HESPlayerTransferEvent : Event
    {
        public ResponseResult Response = ResponseResult.Success;
        public string Message = "";
        public string CharacterData = "";
        public string CharacterName = "";
        public string SteamId;
        public long CharacterId;

        public HESPlayerTransferEvent(ResponseResult response, string msg, string chardata, string charname, string stmid, long charid) : base(EventID.SpawnEvent)
        {
            Response = response;
            Message = msg;
            CharacterData = chardata;
            CharacterName = charname;
            CharacterId = charid;
        }
    }
}
