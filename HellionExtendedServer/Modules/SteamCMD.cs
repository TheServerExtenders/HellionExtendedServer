using HellionExtendedServer.Common;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text;
using System.Threading;

namespace HellionExtendedServer.Modules
{
    public class SteamCMD
    {
        private const string SteamCMDDir = "Hes\\steamcmd";
        private static string SteamCMDExe = $"{SteamCMDDir}\\steamcmd.exe";
        private static string SteamCMDZip = $"{SteamCMDDir}\\steamcmd.zip";

        public SteamCMD()
        {
            Log.Instance.Info("Running SteamCMD Checks..");
        }

        public bool GetSteamCMD()
        {

            if (!Directory.Exists(SteamCMDDir))
                Directory.CreateDirectory(SteamCMDDir);

            if (!File.Exists(SteamCMDExe))
            {
                try
                {
                    Log.Instance.Info("SteamCMD does not exist, downloading!");

                    using (var client = new WebClient())
                        client.DownloadFile("https://steamcdn-a.akamaihd.net/client/installer/steamcmd.zip", SteamCMDZip);

                    Log.Instance.Info("Done! Unpacking and starting SteamCMD to install Hellion Dedicated Server");

                    ZipFile.ExtractToDirectory(SteamCMDZip, SteamCMDDir);
                    File.Delete(SteamCMDZip);
                }
                catch (Exception)
                {
                    Log.Instance.Error("Could not download or unpack SteamCMD. Going into manual mode. Please install Hellion Dedicated and move HellionExtendedServer there!");
                    return false;
                }
            }

            string script = @" +login anonymous +force_install_dir ../../ +app_update 598850 validate +quit";

            try
            {
                Log.Instance.Info("Updating Hellion Dedicated Server...");

                var steamCmdinfo = new ProcessStartInfo(SteamCMDExe, script)
                {
                    WorkingDirectory = Path.GetFullPath(SteamCMDDir),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.ASCII
                };
                Process steamCmd = Process.Start(steamCmdinfo);

                while (!steamCmd.HasExited)
                {
                    Console.WriteLine(steamCmd.StandardOutput.ReadLine());
                    Thread.Sleep(100);
                }
            }
            catch (Exception)
            {
                Log.Instance.Error("Could not start SteamCMD. Going into manual mode. Please run SteamCMD manually to install or update the dedicated server!");
                return false;
            }

            Log.Instance.Info("Hellion Dedicated has been successfully installed!");

            return true;
        }
    }
}