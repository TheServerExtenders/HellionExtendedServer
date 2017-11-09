using System;
using System.IO;
using System.Reflection;

namespace HellionExtendedServer.Common
{
    #region Server Name Enums
    public enum ServerFolderName
    {
        Base,
        Data       
    }

    /// <summary>
    /// Server File Name Enums
    /// </summary>
    public enum ServerFileName
    {
        GameServerINI
    }
    #endregion

    #region HES Name Enums
    public enum HESFolderName
    {
        Hes,
        Bin,
        Config,
        Localization,
        Logs,
        Plugins,
        Updates
    }

    /// <summary>
    /// Hes File Name Enums
    /// </summary>
    public enum HESFileName
    {
        NLogConfig,
        Config
    }
    #endregion

    public static class Globals
    {
        #region Server Folder Name Fields
        public static readonly string ServerBaseFolderName = "";
        public static readonly string ServerDataFolderName = "Data";
        #endregion

        #region Server File Name Fields 
        public static readonly string GameServerINIFileName = "GameServer.ini";
        #endregion

        #region Hes File Name Fields
        public static readonly string HESConfigFileName = "Config.cfg";
        public static readonly string NLogConfigFileName = "NLog.config";
        #endregion

        #region Hes Folder Names Fields
        public static readonly string HESRootFolderName = "Hes";
        public static readonly string HESBinariesFolderName = "bin";
        public static readonly string HESConfigFolderName = "config";
        public static readonly string HESLocalizationFolderName = "localization";
        public static readonly string HESLogsFolderName = "logs";
        public static readonly string HESPluginsFolderName = "plugins";
        public static readonly string HESUpdatesFolderName = "updates";
        #endregion

        #region FilePath Methods
        public static string GetFilePath(ServerFileName serverFileName, bool fullPath = true)
        {
            string file = "";
            switch (serverFileName)
            {
                case ServerFileName.GameServerINI:
                    file = Path.Combine(GetFolderPath(ServerFolderName.Base, fullPath), GameServerINIFileName);
                    break;
            }
            return file;
        }

        public static string GetFilePath(HESFileName hesFileName, bool fullPath = true)
        {
            string file = "";
            switch (hesFileName)
            {
                case HESFileName.Config:
                    file = Path.Combine(GetFolderPath(HESFolderName.Config, fullPath), HESConfigFileName);
                    break;

                case HESFileName.NLogConfig:
                    file = Path.Combine(GetFolderPath(HESFolderName.Config, fullPath), NLogConfigFileName);
                    break;
            }
            return file;
        }
        #endregion

        #region FolderPath Methods
        public static string GetFolderPath(ServerFolderName serverFolderName, bool fullPath = false)
        {
            string fullBasePath = Environment.CurrentDirectory;

            string path = "";
            switch (serverFolderName)
            {
                case ServerFolderName.Base:
                    path = ServerBaseFolderName;
                    break;

                case ServerFolderName.Data:
                    path = Path.Combine(HESRootFolderName, ServerDataFolderName);
                    break;
            }

            return fullPath ? Path.Combine(ServerBaseFolderName, path) : path;
        }

        public static string GetFolderPath(HESFolderName hesFolder, bool fullPath = false)
        {
            string fullBasePath = Environment.CurrentDirectory;

            string path = "";
            switch (hesFolder)
            {
                case HESFolderName.Hes:
                    path = HESRootFolderName;
                    break;

                case HESFolderName.Bin:
                    path = Path.Combine(HESRootFolderName, HESBinariesFolderName);
                    break;

                case HESFolderName.Config:
                    path = Path.Combine(HESRootFolderName, HESConfigFolderName);
                    break;

                case HESFolderName.Localization:
                    path = Path.Combine(HESRootFolderName, HESLocalizationFolderName);
                    break;

                case HESFolderName.Logs:
                    path = Path.Combine(HESRootFolderName, HESLogsFolderName);
                    break;

                case HESFolderName.Plugins:
                    path = Path.Combine(HESRootFolderName, HESPluginsFolderName);
                    break;

                case HESFolderName.Updates:
                    path = Path.Combine(HESRootFolderName, HESUpdatesFolderName);
                    break;
            }
            return fullPath ? Path.Combine(fullBasePath, path) : path;
        }
        #endregion
    }
}