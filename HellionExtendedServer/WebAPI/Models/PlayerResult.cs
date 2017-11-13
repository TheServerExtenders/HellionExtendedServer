using System;
using System.Collections.Generic;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace HellionExtendedServer.WebAPI.Models
{
    public class PlayerResult : BaseResult
    {
        private Player _player;

        public PlayerResult(Player player, bool error = false, string message = "Success") 
            : base(error, message)
        {
            _player = player;

            Name = _player.Name;
            SteamID = _player.SteamId;
            Gender = _player.Gender;
            IsAlive = _player.IsAlive;
            Health = _player.Health;
        }

        public string Name { get; set; }

        public string SteamID { get; set; }

        public Gender Gender { get; set; }

        public bool IsAlive { get; set; }

        public float Health { get; set; }
    }

    public class PlayerResultList : BaseResult
    {

        public PlayerResultList(List<PlayerResult> players = null, bool error = false, string message = "Success")
            : base(error, message)
        {
            Players = players;
        }

        public List<PlayerResult> Players { get; set; }

        public List<PlayerResult> Add(PlayerResult result)
        {
            if (Players == null)
                Players = new List<PlayerResult>();
            
            Players.Add(result);

            return Players;
        }
    }
}