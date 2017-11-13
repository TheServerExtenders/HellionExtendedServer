using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using ZeroGravity.Network;
using ZeroGravity.Objects;
using ZeroGravity;
using HellionExtendedServer.GUI;
using HellionExtendedServer.Managers;
using System.Collections.Generic;

namespace HellionExtendedServer.WebAPI.Models
{
    public class ServerStatusResult : BaseResult
    {

        public ServerStatusResult(ServerInstance serverInstance, bool error = false , string message = "Success")
            : base(error, message)
        {
            var _server = ServerInstance.Instance;

            ServerRunning = serverInstance.IsRunning;

            if (serverInstance.Server != null)
            {               
                ServerStartTime = _server.Server.ServerStartTime;
            }

            GameServerSettings = ServerInstance.Instance.GameServerConfig.Settings;
        }


        public bool ServerRunning { get; set; }

        public DateTime ServerStartTime { get; set; }

        public Dictionary<string, string> GameServerSettings { get; set; }

    }
}