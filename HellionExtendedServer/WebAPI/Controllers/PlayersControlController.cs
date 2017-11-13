using HellionExtendedServer.Managers;
using HellionExtendedServer.Modules;
using HellionExtendedServer.WebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ZeroGravity.Objects;

namespace HellionExtendedServer.WebAPI.Controllers
{
    public class PlayersControlController : ApiController
    {
        private List<PlayerResult> GetPlayerList()
        {
            List<PlayerResult> players = new List<PlayerResult>();

            if (players.Count == 0)
                return players;

            foreach (var client in NetworkManager.Instance.ClientList)
            {
                players.Add(new PlayerResult(client.Value.Player));
            }

            return players;
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetConnectedPlayers(int amount = 0)
        {
            if (Config.Instance.Settings.DebugMode)
                Console.WriteLine("WebAPI: GetConnectedPlayers request from " + UtilityMethods.GetUserIp(Request));

            try
            {
                List<PlayerResult> outPlayers = new List<PlayerResult>();

                var players = GetPlayerList();

                if (players.Count == 0)
                    return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, new PlayerResultList(outPlayers)));

                var responsePlayers = outPlayers;

                if (amount > 0)
                    responsePlayers = outPlayers.GetRange(0, amount);

                return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, new PlayerResultList(responsePlayers)));
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.OK, ex);
            }
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetPlayerByGUID(long guid)
        {
            if (Config.Instance.Settings.DebugMode)
                Console.WriteLine("WebAPI: GetPlayerByGUID request from " + UtilityMethods.GetUserIp(Request));

            try
            {
                var players = GetPlayerList();

                if (players.Count == 0)
                    return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, new BaseResult(true, "There are no players online")));

                if (!NetworkManager.Instance.ConnectedPlayer(guid, out Player player))
                    return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, new BaseResult(true, "That player is not connected.")));

                if (player == null)
                    return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, new BaseResult(true, "The player is null")));


                return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, new PlayerResultList().Add( new PlayerResult(player))));

            }
            catch (Exception ex)
            {
                return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, new ErrorResult(ex)));
            }
        }

        [HttpGet]
        public async Task<HttpResponseMessage> GetPlayerBySteamID(ulong steamid)
        {
            try
            {
                if (Config.Instance.Settings.DebugMode)
                    Console.WriteLine("WebAPI: GetPlayerBySteamID request from " + UtilityMethods.GetUserIp(Request));

                var players = GetPlayerList();

                if (players.Count == 0)
                    return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, "There are no players online"));

                var player = players.Where((pl) => pl.SteamID == steamid.ToString()).FirstOrDefault();

                if (player == null)
                    return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, new BaseResult(true, "The player is null")));

                return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, new PlayerResultList().Add(player)));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, new ErrorResult(ex)));
            }
        }
    }
}