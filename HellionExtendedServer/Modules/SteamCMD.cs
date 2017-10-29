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
        private const string SteamCMDDir = "steamcmd";
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

                    Log.Instance.Info("Done! Unpacking and starting SteamCMD to install Hellion_Dedicated.exe");

                    ZipFile.ExtractToDirectory(SteamCMDZip, SteamCMDDir);
                    File.Delete(SteamCMDZip);
                }
                catch (Exception)
                {
                    Log.Instance.Error("Could not download or unpack SteamCMD. Going into manual mode. Please install Hellion Dedicated and move HellionExtendedServer there!");
                    return false;
                }
            }


            try
            {
                string script = $" +@ShutdownOnFailedCommand 1 " +
                    $"+@NoPromptForPassword 1 " +
                    $"+@sSteamCmdForcePlatformType windows " +
                    $"+login anonymous +force_install_dir {Environment.CurrentDirectory} " +
                    $"+app_update 598850 validate +quit";
                

                var steamCmdinfo = new ProcessStartInfo(SteamCMDExe , script)
                {
                    WorkingDirectory = Path.GetFullPath(SteamCMDDir),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    StandardOutputEncoding = Encoding.ASCII
                };

                Console.WriteLine(steamCmdinfo.Arguments);

                var steamCmd = Process.Start(steamCmdinfo);

                while (!steamCmd.HasExited)
                {
                    Console.WriteLine(steamCmd.StandardOutput.ReadLine());
                    Thread.Sleep(100);
                }
            }
            catch (Exception)
            {
                Log.Instance.Error("Could not start SteamCMD. Going into manual mode. Please run SteamCMD manually!");
                return false;
            }

            return true;
        }
    }
}