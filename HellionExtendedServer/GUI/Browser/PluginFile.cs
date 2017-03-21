using System;
using System.IO;
using System.Net;

namespace HellionExtendedServer.GUI.Browser
{
    public class PluginFile : IEquatable<PluginFile>
    {
        public string Author { get; set; }
        public long CreatedData { get; set; }
        public string Description { get; set; }
        public int DownloadCount { get; set; }
        public string DownloadURL { get; set; }
        public int Id { get; set; }
        public long LastUpdateDate { get; set; }
        public string Name { get; set; }
        public string RatingAVG { get; set; }
        public string RatingSUM { get; set; }
        public string TimesRated { get; set; }
        public string Version { get; set; }
        public int VersionId { get; set; }

        public void Download()
        {
            try
            {
                var request = WebRequest.Create(DownloadURL) as HttpWebRequest;
                request.Method = "GET";
                request.Proxy = null;
                request.CookieContainer = new CookieContainer();
                request.UserAgent = nameof(HellionExtendedServer);

                long received = 0;
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    var buffer = new byte[1024];

                    var fileStream = File.OpenWrite(Name);
                    using (Stream input = response.GetResponseStream())
                    {
                        var size = input.Read(buffer, 0, buffer.Length);
                        while (size > 0)
                        {
                            fileStream.Write(buffer, 0, size);
                            received += size;

                            size = input.Read(buffer, 0, buffer.Length);
                        }

                        Console.WriteLine("File downloaded! : " + Name + "| Size: " + received / 1024 + "Kb");
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

        public bool Equals(PluginFile other)
        {
            return Id == other.Id;
        }
    }
}