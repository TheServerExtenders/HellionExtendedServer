using System;
using System.Collections.Generic;
using ZeroGravity;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace HellionExtendedServer.Managers
{
    public class PlayerManager
    {
        /// <summary>
        /// This method allow to get a specific player with his ingame name or steamID
        /// </summary>
        /// <param name="name">Name or steamID (steamId=true requiered)</param>
        /// <param name="steamId">True if name is the steamID</param>
        /// <returns></returns>
        public Player FindPlayer(string name, bool steamId = false)
        {
            if (name == "")
                return null;

            Dictionary<long, Player>.ValueCollection players = HES.CurrentServer.AllPlayers;

            foreach (var player in players)
                if (player.Name == name)
                    return player;

            return null;
        }
    }
}
