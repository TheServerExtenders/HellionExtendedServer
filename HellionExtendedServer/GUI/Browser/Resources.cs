using Newtonsoft.Json;
using System.Collections.Generic;

namespace HellionExtendedServer.GUI.Browser
{
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