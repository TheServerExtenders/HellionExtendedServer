using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Managers;
using System.Reflection;
using System.Resources;
using System.Collections;

namespace HellionExtendedServer.Components
{
    public class Localization
    {
        #region Fields

        private Dictionary<string, string> m_sentences = new Dictionary<string, string>();

        public static string PathFolder = "./Localization/";
        public static string Version = "0.03";

        #endregion

        #region Properties

        public Dictionary<string, string> Sentences { get { return m_sentences; } }

        #endregion

        #region Methods

        public void Load(string FileName)
        {
            if (File.Exists(PathFolder + FileName + ".resx"))
            {
                ResXResourceSet resxSet = new ResXResourceSet(PathFolder + FileName + ".resx");
                if (resxSet.GetString("version")==Version)
                {
                    using (ResXResourceReader resxReader = new ResXResourceReader(PathFolder + FileName + ".resx"))
                    {
                        foreach (DictionaryEntry entry in resxReader)
                        {
                            m_sentences.Add((string)entry.Key, (string)entry.Value);
                            
                        }
                    }
                }else
                {
                    Log.Instance.Info("Your localization file is not updated ! Please download the lastest version on our github page. English language loading...");
                    CreateDefault();
                    Load("En");
                }
            }
            else
            {
                Log.Instance.Info("No localization file detected ! English language loading...");
                CreateDefault();
                Load("En");
            }
        }

        public static void CreateDefault()
        {
            if (File.Exists(PathFolder + "En.resx"))
                File.Delete(PathFolder + "En.resx");

            Directory.CreateDirectory(PathFolder);
            var stream = File.Create(PathFolder + "En.resx");
            stream.Close();
            using (ResXResourceWriter resx = new ResXResourceWriter(PathFolder + "En.resx"))
            {
                resx.AddResource("version", Version);
                resx.AddResource("Initialization", "Hellion Extended Server v{0} Initialized.");
                resx.AddResource("PlayersConnected", "Players Connected : {0}/{1}");
                resx.AddResource("AllPlayers", "{0} players already played in the server since its launching.");
                resx.AddResource("PlayerNotConnected", "This player is not connected");
                resx.AddResource("NoPlayerName", "No player name specified");
                resx.AddResource("PlayerKicked", "{0} was kicked from the server.");
                resx.AddResource("BadSynthax", "Bad synthax ! Use / help to watch all valid commands");
                resx.AddResource("LoadingGUI", "(WIP)Loading GUI...");
                resx.AddResource("DescHelp", "Type directly into the console to chat with online players." + Environment.NewLine + "Current commands are : " + Environment.NewLine);
                resx.AddResource("HelpCommand", "/help - this page ;)");
                resx.AddResource("SaveCommand", "/save - forces a universe save");
                resx.AddResource("StartCommand", "/start - start the server");
                resx.AddResource("StopCommand", "/stop - stop the server");
                resx.AddResource("OpenGUICommand", "/opengui - open the gui");
                resx.AddResource("PlayersCommand", "/players " + Environment.NewLine + "\t -count - returns the current amount of online players" + Environment.NewLine + "\t -list - returns the full list of connected players" + Environment.NewLine + "\t -all - returns every player that has ever been on the server. And if they're online.");
                resx.AddResource("MsgCommand", "/send (name) text - send a message to the specified player");
                resx.AddResource("KickCommand", "/kick (name) - kick the specified player from the server");
                resx.AddResource("Closing", "CLOSING HELLION EXTENDED SERVER");
                resx.AddResource("ChatMsgListener", "Chat Message Listener Added.");
                resx.AddResource("FailedInitPlugin", "Failed initialization of Plugin {0}. Uncaught Exception: {1}");
                resx.AddResource("FailedLoadAssembly", "Failed to load assembly : {0} : {1}");
                resx.AddResource("FailedShutdownPlugin", "Uncaught Exception in Plugin {0}. Exception: {1}");
                resx.AddResource("InitializationPlugin", "Initialization of Plugin {0} failed. Could not find a public, parameterless constructor for {0}");
                resx.AddResource("InitializingPlugin", "Initializing Plugin : {0}");
                resx.AddResource("LoadingDedicated", "Loading HELLION Dedicated...");
                resx.AddResource("NetControlerLoaded", "Network Controller Loaded!");
                resx.AddResource("NewPlayer", "A new player is arrived : {0}");
                resx.AddResource("PlayerOnServerListener", "Player On Server Listener Added.");
                resx.AddResource("PlayerSpawnChat", "{0} has spawned !");
                resx.AddResource("PlayerSpawnListener", "Player Spawns Listener Added.");
                resx.AddResource("PlayerSpawnLog", "{0} spawned ({1})");
                resx.AddResource("ReadyForConnections", "Ready for connections !");
                resx.AddResource("SaveAlreadyInProgress", "Save is already in progress!");
                resx.AddResource("SavedUniverse", "Universe saved");
                resx.AddResource("SavedUniverseTime", "Universe Saved in {0}ms to {1}");
                resx.AddResource("SavingUniverse", "Saving Universe...");
                resx.AddResource("ServerDesc", "==============================================================================" + Environment.NewLine + "\tServer name: {5}" + Environment.NewLine + "\tServer ID: {1}" + Environment.NewLine + "\tStart date: {0}" + Environment.NewLine + "\tServer ticks: {2}{4}" + Environment.NewLine + "\tMax server ticks (not precise): {3}" + Environment.NewLine + "==============================================================================");
                resx.AddResource("ShutdownPlugin", "Shutting down Plugin {0}");
                resx.AddResource("ShuttingDown", "Shutting down server...");
                resx.AddResource("SuccessShutdown", "Server Successfully shutdown.");
                resx.AddResource("WaitingStart", "Waiting for server to start. This may take at least 10 seconds or longer depending on the size of the current save.");
                resx.AddResource("Welcome", "Welcome {0} on {1} !");
                resx.AddResource("WorldInit", "World Initialized !");
            }
        }

        #endregion
    }
}
