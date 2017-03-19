using System;
using System.IO;
using System.Net;

namespace HellionExtendedServer.Managers.Plugins.Browser
{
    internal static class PluginBrowser
    {
        public static void DownloadPlugin(string fileName, string url)
        {
            try
            {
                Console.WriteLine("Trying to download the file..");

                var request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";
                request.Proxy = null;
                request.CookieContainer = new CookieContainer();
                request.UserAgent = nameof(HellionExtendedServer);

                long received = 0;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    var buffer = new byte[1024];

                    var fileStream = File.OpenWrite(fileName);
                    using (Stream input = response.GetResponseStream())
                    {
                        var size = input.Read(buffer, 0, buffer.Length);
                        while (size > 0)
                        {
                            fileStream.Write(buffer, 0, size);
                            received += size;

                            size = input.Read(buffer, 0, buffer.Length);
                        }

                        Console.WriteLine("File downloaded! : " + fileName + "| Size: " + received / 1024 + "Kb");
                    }

                    fileStream.Flush();
                    fileStream.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}