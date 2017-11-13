using HellionExtendedServer.Managers;
using HellionExtendedServer.Modules;
using HellionExtendedServer.WebAPI.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace HellionExtendedServer.WebAPI.Controllers
{
    public class ServerControlController : ApiController
    {
        /// <summary>
        ///  Starts the server if its not running.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> Start()
        {
            try
            {
                if (Config.Instance.Settings.DebugMode)
                    Console.WriteLine("WebAPI: Server Start Request from " + UtilityMethods.GetUserIp(Request));

                if (!ServerInstance.Instance.IsRunning)
                {
                    ServerInstance.Instance.Start(true);

                    while (!ServerInstance.Instance.IsRunning)
                    {
                        await Task.Delay(100);
                    }
                }
                else
                {
                    return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, new BaseResult(true, "Server Is Already Running")));
                }

                return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, new BaseResult(false, "Server has been started.")));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, new ErrorResult(ex)));
            }
        }

        /// <summary>
        /// Stops the server if it's running
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> Stop()
        {
            try
            {
                if (Config.Instance.Settings.DebugMode)
                    Console.WriteLine("WebAPI: Server Stop Request from " + UtilityMethods.GetUserIp(Request));

                if (ServerInstance.Instance.IsRunning)
                {
                    ServerInstance.Instance.Stop(true);

                    while (ServerInstance.Instance.IsRunning)
                    {
                        await Task.Delay(100);
                    }
                }
                else
                {
                    return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, new BaseResult(true, "Server is already stopped.")));
                }

                return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, new BaseResult(false, "Server has been stopped")));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, new ErrorResult(ex)));
            }
        }

        /// <summary>
        /// Forces a universe save if the server is running
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage ForceSave()
        {
            try
            {
                if (Config.Instance.Settings.DebugMode)
                    Console.WriteLine("WebAPI: Server Save Request from " + UtilityMethods.GetUserIp(Request));

                if (ServerInstance.Instance.IsRunning)
                {
                    ServerInstance.Instance.Save(true);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new BaseResult(false, "Server is not running"));
                }

                return Request.CreateResponse(HttpStatusCode.OK, new BaseResult(false, "Save Completed"));
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new ErrorResult(ex));
            }
        }

        /// <summary>
        /// Returns the Status of the server and the set options in the GameServerIni
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<HttpResponseMessage> Status()
        {
            try
            {
                if (Config.Instance.Settings.DebugMode)
                    Console.WriteLine("WebAPI: Server Status Request from " + UtilityMethods.GetUserIp(Request));

                var result = new ServerStatusResult(ServerInstance.Instance);

                while (result == null)
                {
                    await Task.Delay(100);
                }

                return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, result));
            }
            catch (Exception ex)
            {
                return await Task.FromResult(Request.CreateResponse(HttpStatusCode.OK, new ErrorResult(ex)));
            }
        }
    }
}