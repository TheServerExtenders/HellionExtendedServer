using System;
using System.Collections.Generic;
using ZeroGravity.Network;

namespace HellionExtendedServer.Managers.Event.Player
{
    public class HESRemoveDeletedCharcters : Event
    {
        public ResponseResult Response = ResponseResult.Success;
        public string Message = "";
        public Dictionary<long, CharacterData> Characters = new Dictionary<long, CharacterData>();

        public HESRemoveDeletedCharcters(ResponseResult responce, String message,
            Dictionary<long, CharacterData> characters) : base(EventID.RemoveDeletedCharcters)
        {
            Response = responce;
            Message = message;
            Characters = characters;
        }

    }


}