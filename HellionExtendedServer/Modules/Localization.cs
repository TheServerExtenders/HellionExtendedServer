using HellionExtendedServer.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Resources;

namespace HellionExtendedServer.Modules
{
    public class Localization
    {
        public static string PathFolder = "./Hes/localization/";
        public static string Version = "0.03";
        private Dictionary<string, string> m_sentences = new Dictionary<string, string>();

        public Dictionary<string, string> Sentences
        {
            get
            {
                return this.m_sentences;
            }
        }

        public void Load(string FileName)
        {
            if (File.Exists(Localization.PathFolder + FileName + ".resx"))
            {
                if (new ResXResourceSet(Localization.PathFolder + FileName + ".resx").GetString("version") == Localization.Version)
                {
                    using (ResXResourceReader resXresourceReader = new ResXResourceReader(Localization.PathFolder + FileName + ".resx"))
                    {
                        foreach (DictionaryEntry dictionaryEntry in resXresourceReader)
                            this.m_sentences.Add((string)dictionaryEntry.Key, (string)dictionaryEntry.Value);
                    }
                }
                else
                {
                    Log.Instance.Info("Your localization file is not updated ! Please download the lastest version on our github page. English language loading...");
                    Localization.CreateDefault();
                    this.Load("En");
                }
            }
            else
            {
                Log.Instance.Info("No localization file detected ! English language loading...");
                Localization.CreateDefault();
                this.Load("En");
            }
        }

        public static void CreateDefault()
        {
            if (File.Exists(Localization.PathFolder + "En.resx"))
                File.Delete(Localization.PathFolder + "En.resx");
            Directory.CreateDirectory(Localization.PathFolder);
            File.Create(Localization.PathFolder + "En.resx").Close();
            using (ResXResourceWriter resXresourceWriter = new ResXResourceWriter(Localization.PathFolder + "En.resx"))
            {
                resXresourceWriter.AddResource("version", Localization.Version);
                resXresourceWriter.AddResource("Initialization", "Hellion Extended Server v{0} Initialized.");
                resXresourceWriter.AddResource("PlayersConnected", "Players Connected : {0}/{1}");
                resXresourceWriter.AddResource("AllPlayers", "{0} players already played in the server since its launching.");
                resXresourceWriter.AddResource("PlayerNotConnected", "This player is not connected");
                resXresourceWriter.AddResource("NoPlayerName", "No player name specified");
                resXresourceWriter.AddResource("PlayerKicked", "{0} was kicked from the server.");
                resXresourceWriter.AddResource("BadSyntax", "Bad synthax ! Use / help to watch all valid commands");
                resXresourceWriter.AddResource("LoadingGUI", "(WIP)Loading GUI...");
                resXresourceWriter.AddResource("DescHelp", "Type directly into the console to chat with online players." + Environment.NewLine + "Current commands are : " + Environment.NewLine);
                resXresourceWriter.AddResource("HelpCommand", "/help - this page ;)");
                resXresourceWriter.AddResource("SaveCommand", "/save - forces a universe save");
                resXresourceWriter.AddResource("StartCommand", "/start - start the server");
                resXresourceWriter.AddResource("StopCommand", "/stop - stop the server");
                resXresourceWriter.AddResource("OpenGUICommand", "/opengui - open the gui");
                resXresourceWriter.AddResource("PlayersCommand", "/players " + Environment.NewLine + "\t -count - returns the current amount of online players" + Environment.NewLine + "\t -list - returns the full list of connected players" + Environment.NewLine + "\t -all - returns every player that has ever been on the server. And if they're online.");
                resXresourceWriter.AddResource("MsgCommand", "/send (name) text - send a message to the specified player");
                resXresourceWriter.AddResource("KickCommand", "/kick (name) - kick the specified player from the server");
                resXresourceWriter.AddResource("Closing", "CLOSING HELLION EXTENDED SERVER");
                resXresourceWriter.AddResource("ChatMsgListener", "Chat Message Listener Added.");
                resXresourceWriter.AddResource("FailedInitPlugin", "Failed initialization of Plugin {0}. Uncaught Exception: {1}");
                resXresourceWriter.AddResource("FailedLoadAssembly", "Failed to load assembly : {0} : {1}");
                resXresourceWriter.AddResource("FailedShutdownPlugin", "Uncaught Exception in Plugin {0}. Exception: {1}");
                resXresourceWriter.AddResource("InitializationPlugin", "Initialization of Plugin {0} failed. Could not find a public, parameterless constructor for {0}");
                resXresourceWriter.AddResource("InitializingPlugin", "Initializing Plugin : {0}");
                resXresourceWriter.AddResource("LoadingDedicated", "Loading HELLION Dedicated...");
                resXresourceWriter.AddResource("NetControlerLoaded", "Network Controller Loaded!");
                resXresourceWriter.AddResource("NewPlayer", "A new player is arrived : {0}");
                resXresourceWriter.AddResource("PlayerOnServerListener", "Player On Server Listener Added.");
                resXresourceWriter.AddResource("PlayerSpawnChat", "{0} has spawned !");
                resXresourceWriter.AddResource("PlayerSpawnListener", "Player Spawns Listener Added.");
                resXresourceWriter.AddResource("PlayerSpawnLog", "{0} spawned ({1})");
                resXresourceWriter.AddResource("ReadyForConnections", "Ready for connections !");
                resXresourceWriter.AddResource("SaveAlreadyInProgress", "Save is already in progress!");
                resXresourceWriter.AddResource("SavedUniverse", "Universe saved");
                resXresourceWriter.AddResource("SavedUniverseTime", "Universe Saved in {0}ms to {1}");
                resXresourceWriter.AddResource("SavingUniverse", "Saving Universe...");
                resXresourceWriter.AddResource("ServerDesc", "==============================================================================" + Environment.NewLine + "\tServer name: {5}" + Environment.NewLine + "\tServer ID: {1}" + Environment.NewLine + "\tStart date: {0}" + Environment.NewLine + "\tServer ticks: {2}{4}" + Environment.NewLine + "\tMax server ticks (not precise): {3}" + Environment.NewLine + "==============================================================================");
                resXresourceWriter.AddResource("ShutdownPlugin", "Shutting down Plugin {0}");
                resXresourceWriter.AddResource("ShuttingDown", "Shutting down server...");
                resXresourceWriter.AddResource("SuccessShutdown", "Server Successfully shutdown.");
                resXresourceWriter.AddResource("WaitingStart", "Waiting for server to start. This may take at least 10 seconds or longer depending on the size of the current save.");
                resXresourceWriter.AddResource("Welcome", "Welcome {0} on {1} !");
                resXresourceWriter.AddResource("WorldInit", "World Initialized !");
            }
        }
    }
}