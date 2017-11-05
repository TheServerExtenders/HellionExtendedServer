using HellionExtendedServer.Common;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using Octokit;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

namespace HellionExtendedServer.Managers
{
    public class UpdateManager
    {
        private GitHubClient _git = new GitHubClient(new ProductHeaderValue("HellionExtendedServer"));
        private const string UpdateFileName = "update.zip";

        private Release m_currentRelease;
        public Release CurrentRelease => m_currentRelease;

        public static bool EnableAutoUpdates = true;

        public UpdateManager(string[] args)
        {
            ServicePointManager.DefaultConnectionLimit = 10;

            CheckForUpdates().GetAwaiter().GetResult();
        }

        public async Task CheckForUpdates(bool forceUpdate = false)
        {
            await GetLatestReleaseInfo();
            CheckVersion(forceUpdate);
        }

        public void DownloadLatestRelease()
        {
            try
            {
                Console.WriteLine("HellionExtendedServer:  Downloading latest release...");

                WebClient client = new WebClient();
                client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(ReleaseDownloaded);
                client.DownloadDataAsync(new Uri(m_currentRelease.Assets.First().BrowserDownloadUrl));
            }
            catch (Exception ex)
            {
                Console.WriteLine("HellionExtendedServer:  Update Failed (DownloadLatestRelease)" + ex.ToString());
            }
                      
        }

        private void ReleaseDownloaded(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                File.WriteAllBytes(Path.Combine(Globals.GetFolderPath(HESFolderName.Updates), UpdateFileName), e.Result);
                ZipFile.ExtractToDirectory(Path.Combine(Globals.GetFolderPath(HESFolderName.Updates), UpdateFileName), Globals.GetFolderPath(HESFolderName.Updates));
                File.Delete(Path.Combine(Globals.GetFolderPath(HESFolderName.Updates), UpdateFileName));
                Console.WriteLine("HellionExtendedServer:  Update has been downloaded!");
              
                //ApplyUpdate(EnableAutoUpdates); 
                
                Console.WriteLine("HellionExtendedServer:  Update has been applied. Please restart HellionExtendedServer.exe to finish the update!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("HellionExtendedServer:  Update Failed (ReleaseDownloadedEvent)" + ex.ToString());
            }
        }

        public void ApplyUpdate(bool restart = false)
        {
            try
            {
                Console.WriteLine("HellionExtendedServer:  Applying Update...");

                string updatePath = Globals.GetFolderPath(HESFolderName.Updates);
                string hesPath = Globals.GetFolderPath(HESFolderName.Hes);

                foreach (string dirPath in Directory.GetDirectories(updatePath, "*",
                    SearchOption.AllDirectories))
                    Directory.CreateDirectory(dirPath.Replace(updatePath, hesPath));

                foreach (string newPath in Directory.GetFiles(updatePath, "*.*",
                    SearchOption.AllDirectories))
                    File.Copy(newPath, newPath.Replace(updatePath, hesPath), true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("HellionExtendedServer:  Update Failed (ApplyUpdate)" + ex.ToString());
            }

        }

        public bool CheckVersion(bool forceUpdate = false)
        {
            try
            {
                var checkedVersion = new Version(m_currentRelease.TagName);

                if (checkedVersion > HES.Version || forceUpdate)
                {
                    Console.WriteLine("HellionExtendedServer:  A new version of Hellion Extended Server has been detected.\r\n");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Name: { m_currentRelease.Assets.First().Name }");
                    Console.WriteLine($"Version: { m_currentRelease.TagName }");
                    Console.WriteLine($"Total Downloads: { m_currentRelease.Assets.First().DownloadCount }");
                    Console.WriteLine($"Published Date: { m_currentRelease.Assets.First().CreatedAt }\r\n");
                    Console.ResetColor();

                    if (!EnableAutoUpdates)
                    {
                        Console.WriteLine("Would you like to see the changes? (y/n)");

                        if (Console.ReadKey().Key == ConsoleKey.Y)
                        {
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\r\nChanges:\r\n" + m_currentRelease.Body);
                            Console.ResetColor();
                        }
                           

                        Console.WriteLine("Would you like to update now? (y/n)");

                        if (Console.ReadKey().Key == ConsoleKey.Y)
                        {
                            Console.WriteLine("\r\n");
                            DownloadLatestRelease();
                            return true;
                        }

                        Console.WriteLine("HellionExtendedServer:  Skipping update.. We'll ask next time you restart HES!");
                    }
                    else
                    {
                        Console.WriteLine("HellionExtendedServer:  Auto updating");
                        DownloadLatestRelease();
                    }
                    return true;
                }
                else
                {
                    Console.WriteLine("HellionExtendedServer:  HES is running the latest version!");
                }
                              
            }
            catch (Exception ex)
            {
                Console.WriteLine("HellionExtendedServer:  Update Failed (CheckVersion)" + ex.ToString());
            }
            return false;
        }

        public async Task GetLatestReleaseInfo()
        {
            try
            {
                m_currentRelease = await _git.Repository.Release.GetLatest("HellionCommunity", "HellionExtendedServer").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Update Failed (GetLatestReleaseInfo)" + ex.ToString());
            }
        }
    }   
}