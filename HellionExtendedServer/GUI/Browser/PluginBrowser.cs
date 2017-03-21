using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace HellionExtendedServer.GUI.Browser
{
    public class PluginBrowser
    {
        private List<PluginFile> m_pluginfiles;
        private Resources m_resources;

        public List<PluginFile> PluginFiles => m_pluginfiles;

        public PluginBrowser()
        {
            m_pluginfiles = new List<PluginFile>();

            GetOnlineResources(@"https://hellionextendedserver.com/api.php?action=getResources&grab_description=1");
        }

        public void GetOnlineResources(string resourcesApiLink)
        {
            string json;

            try
            {
                var request = WebRequest.Create(resourcesApiLink) as HttpWebRequest;
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
                m_resources = JsonConvert.DeserializeObject<Resources>(json);          
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return;
            }

            PopulatePluginList(m_resources);
        }

        public void PopulatePluginList(Resources resources)
        {
            try
            {
                if (resources != null)
                {
                    for (int i = 0; i < resources.Count; i++)
                    {
                        var resource = resources.ResourceList[i];

                        var plugin = new PluginFile();

                        plugin.Name = resource.Title;
                        plugin.Id = resource.Id;
                        plugin.VersionId = resource.VersionId;
                        plugin.Version = resource.VersionString;
                        plugin.Author = resource.AuthorUsername;
                        plugin.DownloadCount = resource.TimesDownloaded;
                        plugin.Description = (string)resource.Description;

                        plugin.Name = plugin.Name.ToLower();

                        plugin.DownloadURL = String.Format($"http://hellionextendedserver.com/index.php?resources/{plugin.Name}.{plugin.Id}/download&version={plugin.VersionId}");

                        m_pluginfiles.Add(plugin);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}