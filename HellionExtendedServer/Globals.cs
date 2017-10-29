using System.IO;

namespace HellionExtendedServer
{
    public enum HESFolderName
    {
        Base,
        Hes,
        Bin,
        Config,
        Localization,
        Logs,
        Plugins,
        Updates
    }

    public enum HESFileName
    {
        GameServerInI,
        GameServerInIBackup,
        GameServerInIOriginal
    }

    public static class Globals
    {
        public static string RootFolderName = "Hes";

        public static string GetFilePath(HESFileName hESFileName)
        {
            string file = "";
            switch (hESFileName)
            {
                case HESFileName.GameServerInI:
                    file = Path.Combine(GetFolderPath(HESFolderName.Base), "GameServer.ini");
                    break;

                case HESFileName.GameServerInIBackup:
                    file = Path.Combine(GetFolderPath(HESFolderName.Config), "GameServer.ini.backup");
                    break;

                case HESFileName.GameServerInIOriginal:
                    file = Path.Combine(GetFolderPath(HESFolderName.Config), "GameServer.ini.original");
                    break;

                default:
                    file = "GameServer.ini.original";
                    break;
            }
            return file;
        }

        public static string GetFolderPath(HESFolderName hESFolder)
        {
            string path = "";

            switch (hESFolder)
            {
                case HESFolderName.Base:
                    path = "";
                    break;

                case HESFolderName.Hes:
                    path = RootFolderName;
                    break;

                case HESFolderName.Bin:
                    path = Path.Combine(RootFolderName, "bin");
                    break;

                case HESFolderName.Config:
                    path = Path.Combine(RootFolderName, "config");
                    break;

                case HESFolderName.Localization:
                    path = Path.Combine(RootFolderName, "localization");
                    break;

                case HESFolderName.Logs:
                    path = Path.Combine(RootFolderName, "logs");
                    break;

                case HESFolderName.Plugins:
                    path = Path.Combine(RootFolderName, "plugins");
                    break;

                case HESFolderName.Updates:
                    path = Path.Combine(RootFolderName, "updates");
                    break;

                default:
                    path = "";
                    break;
            }

            return path;
        }
    }
}