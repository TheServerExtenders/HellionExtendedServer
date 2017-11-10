using HellionExtendedServer.Common;
using HellionExtendedServer.Modules;
using Octokit;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace HellionExtendedServer.Managers
{
    public class UpdateManager
    {
        private GitHubClient _git = new GitHubClient(new ProductHeaderValue("HellionExtendedServer"));
        private const string UpdateFileName = "update.zip";

        private Release m_currentRelease;
        private Release m_developmentRelease;
        private bool m_useDevRelease = Config.Instance.Settings.EnableDevelopmentVersion;

        private static UpdateManager m_instance;
        public static UpdateManager Instance => m_instance;

        public Release CurrentRelease => m_currentRelease;
        public Release DevelopmentRelease => m_developmentRelease;

        public List<FileInfo> FileList = new List<FileInfo>();
        public List<FileInfo> CurrentFileList = new List<FileInfo>();

        public static bool EnableAutoUpdates = true;
        public static bool GUIMode = false;
        public static bool HasUpdate = false;
        public static Version NewVersionNumber = new Version();

        public UpdateManager()
        {
            m_instance = this;

            ServicePointManager.DefaultConnectionLimit = 10;

            foreach (string file in Directory.GetFiles(Globals.GetFolderPath(HESFolderName.Updates), "*", SearchOption.AllDirectories))
                FileList.Add(new FileInfo(file));

            foreach (string file in Directory.GetFiles(Environment.CurrentDirectory, "*", SearchOption.AllDirectories))
            {
                var currentFile = new FileInfo(file);

                if (currentFile.Extension == ".old")
                    currentFile.Delete();

                if (file.Contains("updates") || file.Contains("temp"))
                    continue;

                CurrentFileList.Add(currentFile);
            }

            CheckForUpdates().GetAwaiter().GetResult();
        }

        public async Task CheckForUpdates(bool forceUpdate = false)
        {
            try
            {
                await GetLatestReleaseInfo();
                CheckVersion(forceUpdate);
            }
            catch (Exception ex)
            {
                Console.WriteLine("HellionExtendedServer:  Update Failed (CheckForUpdates)" + ex.ToString());
            }       
        }

        public bool DownloadLatestRelease(bool getDevelopmentVersion = false)
        {
            try
            {
                Console.WriteLine("HellionExtendedServer:  Downloading latest release...");

                WebClient client = new WebClient();
                client.DownloadDataCompleted += new DownloadDataCompletedEventHandler(ReleaseDownloaded);

                if(getDevelopmentVersion)
                    client.DownloadDataAsync(new Uri(m_developmentRelease.Assets.FirstOrDefault().BrowserDownloadUrl));
                else
                    client.DownloadDataAsync(new Uri(m_currentRelease.Assets.FirstOrDefault().BrowserDownloadUrl));
                
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("HellionExtendedServer:  Update Failed (DownloadLatestRelease)" + ex.ToString());
            }
            return false;
        }

        private void ReleaseDownloaded(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                FileList.ForEach((file) => file.Delete());
                FileList.Clear();

                string updatePath = Globals.GetFolderPath(HESFolderName.Updates);

                File.WriteAllBytes(Path.Combine(updatePath, UpdateFileName), e.Result);
                ZipFile.ExtractToDirectory(Path.Combine(updatePath, UpdateFileName), updatePath);
                File.Delete(Path.Combine(updatePath, UpdateFileName));
                Console.WriteLine("HellionExtendedServer:  Update has been downloaded!");

                foreach (string file in Directory.GetFiles(updatePath, "*", SearchOption.AllDirectories))
                    FileList.Add(new FileInfo(file));

                OnUpdateDownloaded?.Invoke(m_useDevRelease ? m_developmentRelease : m_currentRelease);

                if (!GUIMode)
                {
                    ApplyUpdate();
                    Console.WriteLine("HellionExtendedServer:  Update has been applied. Please restart HellionExtendedServer.exe to finish the update!");

                    if (ServerInstance.Instance != null)
                        HES.Restart(ServerInstance.Instance.IsRunning);
                    else
                        HES.Restart(false);


                }                                
            }
            catch (Exception ex)
            {
                Console.WriteLine("HellionExtendedServer:  Update Failed (ReleaseDownloadedEvent)" + ex.ToString());
            }
        }

        public bool ApplyUpdate()
        {
            try
            {
                Console.WriteLine("HellionExtendedServer:  Applying Update...");

                string updatePath = Globals.GetFolderPath(HESFolderName.Updates);
                string hesPath = Globals.GetFolderPath(HESFolderName.Hes);

                // for all of the files already in the server folder
                foreach (var file in CurrentFileList)
                {
                    // if the old file has an updated version
                    if (FileList.Exists(x => x.Name == file.Name))
                    {
                        var newFile = FileList.Find(x => x.Name == file.Name);
                        var fullName = Path.GetFullPath(file.FullName);

                        // rename old file if the file exists
                        if (File.Exists(fullName))
                            File.Move(fullName, fullName + ".old");

                        // move new file if it doesn't already exist
                        if (!File.Exists(fullName) && File.Exists(Path.GetFullPath(newFile.FullName)))
                            File.Move(Path.GetFullPath(newFile.FullName), fullName);
                    }
                }

                if (Config.Instance.Settings.AutoRestartsEnable && !GUIMode)
                    HES.Restart();

                OnUpdateApplied?.Invoke(m_useDevRelease ? m_developmentRelease : m_currentRelease);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("HellionExtendedServer:  Update Failed (ApplyUpdate)" + ex.ToString());
            }
            return false;
        }

        public bool CheckVersion(bool forceUpdate = false)
        {
            try
            {
                string devText = (m_useDevRelease ? "Development Version" : "");

                var checkedVersion = new Version(m_currentRelease.TagName);

                if (m_useDevRelease)
                    checkedVersion = new Version(m_developmentRelease.TagName);

                NewVersionNumber = checkedVersion;

                Release localRelease = m_currentRelease;

                if (m_useDevRelease)
                    localRelease = m_developmentRelease;

                if (GUIMode)
                {
                    HasUpdate = (checkedVersion > HES.Version || forceUpdate);

                    OnUpdateChecked?.Invoke(localRelease);
                    return true;
                }

                if (checkedVersion > HES.Version || forceUpdate)
                {                                       
                    Console.WriteLine($"HellionExtendedServer:  A new {devText} version of Hellion Extended Server has been detected.\r\n");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Name: { localRelease.Assets.First().Name }");
                    Console.WriteLine($"Version: { localRelease.TagName }");
                    Console.WriteLine($"Total Downloads: { localRelease.Assets.First().DownloadCount }");
                    Console.WriteLine($"Published Date: { localRelease.Assets.First().CreatedAt }\r\n");
                    Console.ResetColor();


                    if (!EnableAutoUpdates)
                    {                     
                        Console.WriteLine("Would you like to see the changes? (y/n)");

                        if (Console.ReadKey().Key == ConsoleKey.Y)
                        {

                            Console.ForegroundColor = ConsoleColor.Yellow;
                            Console.WriteLine("\r\nChanges:\r\n" + localRelease.Body);
                            Console.ResetColor();
                        }

                        if (m_useDevRelease)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("WARNING: Be absolutely sure that your server is backed up!");
                            Console.WriteLine("WARNING: Development Versions CAN break your server!\r\n");
                            Console.WriteLine($"Do you agree to use this {devText}? (y/n)");
                            Console.ResetColor();

                            if (Console.ReadKey().Key == ConsoleKey.Y)
                            {
                                Console.WriteLine("\r\nWould you like to update with the development version now? (y/n)");

                                if (Console.ReadKey().Key == ConsoleKey.Y)
                                {
                                    Console.WriteLine("\r\n");
                                    DownloadLatestRelease(true);
                                    return true;
                                }

                            }else if( Console.ReadKey().Key == ConsoleKey.N)
                            {
                                Console.WriteLine($"Canceling this {devText} update for now");
                                return false;
                            }

                        }
                        else
                        {
                            Console.WriteLine("Would you like to update now? (y/n)");

                            if (Console.ReadKey().Key == ConsoleKey.Y)
                            {
                                Console.WriteLine("\r\n");
                                DownloadLatestRelease();
                                return true;
                            }
                        }
                     
                        Console.WriteLine("HellionExtendedServer:  Skipping update.. We'll ask next time you restart HES!");
                    }
                    else
                    {
                        Console.WriteLine("HellionExtendedServer:  Auto updating");
                        DownloadLatestRelease(m_useDevRelease);
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

                if (m_useDevRelease)
                {
                    var releases = await _git.Repository.Release.GetAll("HellionCommunity", "HellionExtendedServer").ConfigureAwait(false);
                    m_developmentRelease = releases.FirstOrDefault(x => x.Prerelease == true);
                    
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine("Update Failed (GetLatestReleaseInfo)" + ex.ToString());
            }
        }

        public delegate void UpdateEventHandler(Release release);
        public event UpdateEventHandler OnUpdateChecked;
        public event UpdateEventHandler OnUpdateDownloaded;
        public event UpdateEventHandler OnUpdateApplied;
    }
}