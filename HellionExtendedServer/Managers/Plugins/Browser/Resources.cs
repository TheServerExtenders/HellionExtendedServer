using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace HellionExtendedServer.Managers.Plugins.Website
{
    public class PluginFile
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public int VersionId { get; set; }
        public string Version { get; set; }
        public string TimesRated { get; set; }
        public string RatingSUM { get; set; }
        public string RatingAVG { get; set; }
        public string Description { get; set; }
        public int DownloadCount { get; set; }
        public string DownloadURL { get; set; }
        public long LastUpdateDate { get; set; }
        public long CreatedData { get; set; }

        public PluginFile()
        {
            Console.WriteLine("Constructing plugin file");
        }

        public void GetResources()
        {
            string json;

            try
            {
                Console.WriteLine("Getting Resources");

                var request = WebRequest.Create(@"https://hellionextendedserver.com/api.php?action=getResources&hash=AeAAUen5PKBEmtewUBRqSCwq") as HttpWebRequest;
                request.Method = "GET";
                request.Proxy = null;
                request.UserAgent = nameof(HellionExtendedServer);

                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        json = reader.ReadToEnd();
                    }
                }

                Console.WriteLine("Converting JSON to Resources");
                var resources = JsonConvert.DeserializeObject<Resources>(json);

                for (int i = 0; i < resources.Count; i++)
                {
                    var resource = resources.ResourceList[i];

                    Console.WriteLine("Building Plugin");
                    var plugin = new PluginFile()
                    {
                        Name = resource.Title,
                        Id = resource.Id, 
                        VersionId = resource.VersionId,
                        Version = resource.VersionString
                    };

                    Console.WriteLine("Ready to compare...");

                    Console.WriteLine($"{plugin.Id}:{plugin.Name}.dll | {plugin.Version}  ");

                    plugin.Name = plugin.Name.ToLower();

                    DownloadURL = String.Format($"http://hellionextendedserver.com/index.php?resources/{plugin.Name}.{plugin.Id}/download&version={plugin.VersionId}");

                    Console.WriteLine("NewDL: " + DownloadURL);

                    PluginBrowser.DownloadPlugin(plugin.Name + ".zip", DownloadURL);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void BuildPluginFile()
        {
        }
    }

    public class Resources
    {
        [JsonProperty("count")]
        public int Count { get; set; }

        [JsonProperty("resources")]
        public List<Resource> ResourceList { get; set; }
    }

    public class Resource
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("author_id")]
        public int AuthorId { get; set; }

        [JsonProperty("author_username")]
        public string AuthorUsername { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("creation_date")]
        public int CreationDate { get; set; }

        [JsonProperty("category_id")]
        public int CategoryId { get; set; }

        [JsonProperty("version_id")]
        public int VersionId { get; set; }

        [JsonProperty("version_string")]
        public string VersionString { get; set; }

        [JsonProperty("file_hash")]
        public string FileHash { get; set; }

        [JsonProperty("description_id")]
        public int DescriptionId { get; set; }

        [JsonProperty("description")]
        public object Description { get; set; }

        [JsonProperty("thread_id")]
        public int ThreadId { get; set; }

        [JsonProperty("external_url")]
        public string ExternalUrl { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("times_downloaded")]
        public int TimesDownloaded { get; set; }

        [JsonProperty("times_rated")]
        public int TimesRated { get; set; }

        [JsonProperty("rating_sum")]
        public int RatingSum { get; set; }

        [JsonProperty("rating_avg")]
        public int RatingAvg { get; set; }

        [JsonProperty("rating_weighted")]
        public int RatingWeighted { get; set; }

        [JsonProperty("times_updated")]
        public int TimesUpdated { get; set; }

        [JsonProperty("times_reviewed")]
        public int TimesReviewed { get; set; }

        [JsonProperty("last_update")]
        public int LastUpdate { get; set; }

        [JsonProperty("custom_fields")]
        public string CustomFields { get; set; }
    }
}