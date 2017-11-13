using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HellionExtendedServer.Modules;
using System.Net;
using System.Web.Http;
using System.Web.Http.SelfHost;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Threading;

namespace HellionExtendedServer.WebAPI
{
    public class HESSelfHost
    {
        private HttpSelfHostServer m_apiServer;
        private bool apiStarted;

        private int apiStartLimit;
        private int startLimit = 0;

        public HESSelfHost()
        {
            apiStarted = false;
            apiStartLimit = 3;
        }

        

        public void Start()
        {

            if (startLimit >= apiStartLimit)
            {
                Log.Instance.Error("WebAPI: Could not start the WebAPI, Start limit excedded.");
                return;
            }
                

            if (apiStarted)
                return;

            try
            {
                string ip = Config.Instance.Settings.WebAPIIP;
                var port = Config.Instance.Settings.WebAPIPort;
                string endPointName = Config.Instance.Settings.WebAPIEndpointName;

                if (!Config.Instance.Settings.EnableWebAPI)
                {
                    Log.Instance.Info("WebAPI: WebAPI Is disabled.: " + ip);
                    return;
                }

                string url = string.Format("http://{0}:{1}/", ip, port);

                if (IPAddress.TryParse(ip, out IPAddress address))
                {
                    var config = new HttpSelfHostConfiguration(url);

                    config.Routes.MapHttpRoute(
                        "DefaultAPI", endPointName + "/{controller}/{action}/{id}",
                        new { id = RouteParameter.Optional });

                    //config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));

                    m_apiServer = new HttpSelfHostServer(config);

                    try
                    {
                        m_apiServer.OpenAsync().Wait();
                        apiStarted = true;
                        startLimit += 1;
                    }
                    catch (AggregateException ex)
                    {
                        if (!ex.InnerException.Message.Contains("HTTP could not register URL"))
                            throw;

                        startLimit += 1;

                        Log.Instance.Warn(ex, "WebAPI: You must run HellionExtendedServer in Administrator to use the WebAPIHost\r\n");

                        /*

                        Log.Instance.Info("WebAPI: HES can add the address using netsh for you, this will let the WebAPI work without needing administrator mode.");

                        Log.Instance.Info("WebAPI: Would you like HES to run NETSH for you? (y/n)\r\n");
                        var key = Console.ReadKey();

                        if (key.Key == ConsoleKey.Y)
                        {
                            Log.Instance.Warn("WebAPI: Running NetSH");

                            if (AddAddress(url))
                            {
                                Log.Instance.Info("WebAPI: Address Added with NETSH!");

                                Log.Instance.Info("WebAPI: Would you like to try to start the WebAPI again? (y/n)\r\n");
                                var key2 = Console.ReadKey();
                                if (key2.Key == ConsoleKey.Y)
                                {
                                    Start();
                                }
                            }
                            else
                            {
                                Log.Instance.Error(ex, "WebAPI: Could not Add the address with NETSH");
                                Log.Instance.Warn(ex, "WebAPI: You must run HellionExtendedServer in Administrator mode.");
                            }
                        }
                        */
                    }

                    if (apiStarted)
                        Log.Instance.Info(string.Format("WebAPI: Server is running on {0}:{1}/api/{2}/.", ip, port, endPointName));
                }
                else
                {
                    Log.Instance.Warn("WebAPI: Invalid IP set in HES Configuration: " + ip);
                }
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "HellionExtendedServer(WebAPI.Start) Error: " + ex.Message);
            }

          

        }

        public void Stop()
        {
            m_apiServer.CloseAsync().Wait();
            m_apiServer.Dispose();
            m_apiServer = null;
        }

        public static bool AddAddress(string address)
        {
            try
            {
                string args = string.Format(@"http add urlacl url={0} sddl=D:(A;;GX;;;S-1-1-0)", address);

                ProcessStartInfo psi = new ProcessStartInfo("netsh", args);
                psi.Verb = "runas";
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardOutput = true;
                psi.StandardOutputEncoding = Encoding.ASCII;

                var proc = Process.Start(psi);

                while (!proc.HasExited)
                {
                    var line = proc.StandardOutput.ReadLine();

                    Console.WriteLine(line);

                    if (line == "URL reservation successfully added." || 
                        line == "Url reservation add failed, Error: 183")
                    {
                        return true;
                    }

                    
                    Thread.Sleep(100);
                }
         
            }
            catch (Exception ex)
            {
                Log.Instance.Error(ex, "WebApi: NETSH AddAddress error: " + ex.ToString());
                return false;
            }


            return false;
        }

        public static bool DeleteAddress(string address)
        {
            try
            {
                string args = string.Format(@"http delete urlacl url={0}", address);

                ProcessStartInfo psi = new ProcessStartInfo("netsh", args);
                psi.Verb = "runas";
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                psi.UseShellExecute = true;


                var proc = Process.Start(psi);

                proc.WaitForExit();
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }
    }


}
