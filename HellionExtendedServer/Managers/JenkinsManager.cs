using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HellionExtendedServer.Managers
{
    public class JenkinsManager
    {
        public string LastSuccessfulBuild_URL;
        public int LastSuccessfulBuild_Number;

        public JenkinsManager()
        {
            using (WebClient wc = new WebClient())
            {
                string updateurl = "http://s1.cybertechpp.com:8080/job/Master%20Branch/api/json?pretty=true"; //Master Branch
                var json = wc.DownloadString(updateurl);
                JObject rss = JObject.Parse(json);
                LastSuccessfulBuild_URL = (string) rss["lastSuccessfulBuild"]["url"]; //http://s1.cybertechpp.com:8080/job/Master%20Branch/5/
                LastSuccessfulBuild_Number = (int) rss["lastSuccessfulBuild"]["number"];
            }
        }

        public void DownloadUpdate()
        {
            List<String> paths = new List<string>();
            using (WebClient wc = new WebClient())
            {
                string updateurl = "http://s1.cybertechpp.com:8080/job/Master%20Branch/"+LastSuccessfulBuild_Number+"/api/json?pretty=true"; //Master Branch
                var json = wc.DownloadString(updateurl);
                JObject rss = JObject.Parse(json);
                //http://s1.cybertechpp.com:8080/job/Master%20Branch/5/artifact/HellionExtendedServer/bin/HESRelease/HellionExtendedServer.exe
                string url = LastSuccessfulBuild_URL + "artifacts/";
                foreach (JObject a in rss["artifacts"])
                {
                    paths.Add(url+a["relativePath"]);
                }
                Console.WriteLine("Found "+paths.Count+" Files to download!");
            }
        }
    }
}