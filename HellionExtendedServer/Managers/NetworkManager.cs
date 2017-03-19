using HellionExtendedServer;
using HellionExtendedServer.Common;
using NLog;
using System;
using System.Collections;
using System.Collections.Generic;
using ZeroGravity;
using ZeroGravity.Helpers;
using ZeroGravity.Network;
using ZeroGravity.Objects;
using static ZeroGravity.Network.NetworkController;

namespace HellionExtendedServer.Managers
{
    public class NetworkManager
    {
        #region Fields

        private static NetworkManager m_networkManager;
        internal ZeroGravity.Network.NetworkController m_network;
        private static readonly Logger chatlogger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Properties

        public static NetworkManager Instance { get { return m_networkManager; } }
        internal ZeroGravity.Network.NetworkController NetContoller { get { return m_network; } }
        public ThreadSafeDictionary<long, Client> ClientList { get { return m_network.clientList; } }
        #endregion Properties

        public NetworkManager(ZeroGravity.Network.NetworkController networkController)
        {

            m_networkManager = this;
            // ISSUE: method pointer
            networkController.EventSystem.AddListener(typeof(TextChatMessage), new EventSystem.NetworkDataDelegate(TextChatMessageListener));
            Log.Instance.Info("Chat Message Listener Added.");

            // Hook into the player spawn event and add in ours as well!
            networkController.EventSystem.AddListener(typeof(PlayerSpawnRequest), new EventSystem.NetworkDataDelegate(PlayerSpawnRequestListener));
            Log.Instance.Info("Player Spawns Listener Added.");

            // [IN TEST] Could be used to detect when the player is physicly in the server
            networkController.EventSystem.AddListener(typeof(PlayersOnServerRequest), new EventSystem.NetworkDataDelegate(PlayerOnServerListener));
            Log.Instance.Info("Player On Server Listener Added.");

            m_network = networkController;
            Log.Instance.Info("Network Controller Loaded!");
        }

        private void PlayerOnServerListener(NetworkData data)
        {
            try
            {
                PlayersOnServerRequest playersOnServerRequest = data as PlayersOnServerRequest;
                Player player;
                if (playersOnServerRequest == null || ConnectedPlayer(playersOnServerRequest.Sender, out player))
                    return;
                Console.WriteLine(string.Format(HES.Localization.Sentences["NewPlayer"], ClientList[playersOnServerRequest.Sender].Player.Name));
                MessageAllClients(string.Format(HES.Localization.Sentences["Welcome"], ClientList[playersOnServerRequest.Sender].Player.Name, Server.Instance.ServerName), true, true);
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Hellion Extended Server [ON SERVER ERROR] : " + ex.InnerException.ToString());
            }
        }

        private void PlayerSpawnRequestListener(NetworkData data)
        {
            try
            {
                PlayerSpawnRequest playerSpawnRequest = data as PlayerSpawnRequest;
                Player player;
                if (playerSpawnRequest == null || ConnectedPlayer(playerSpawnRequest.Sender, out player))
                    return;
                chatlogger.Info(string.Format(HES.Localization.Sentences["PlayerSpawnLog"], player.Name, player.SteamId));
                MessageAllClients(string.Format(HES.Localization.Sentences["PlayerSpawnChat"], player.Name), false, false);
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Hellion Extended Server [SPAWN REQUEST ERROR] : " + ex.InnerException.ToString());
            }
        }

        private void TextChatMessageListener(NetworkData data)
        {
            try
            {
                TextChatMessage textChatMessage = data as TextChatMessage;
                              
                chatlogger.Info("(" + textChatMessage.Sender + ")" + textChatMessage.Name + ": " + textChatMessage.MessageText);
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Hellion Extended Server [MSG REQUEST ERROR] : " + ex.InnerException.ToString());
            }
        }

        public void MessageAllClients(string msg, bool sendAsServer = true, bool printToConsole = true, bool printtoGui = true)
        {
            if (string.IsNullOrEmpty(msg))
                return;
            byte[] guid = Guid.NewGuid().ToByteArray();

            TextChatMessage textChatMessage = new TextChatMessage();

            textChatMessage.GUID = BitConverter.ToInt64(guid, 0);
            textChatMessage.Name = (sendAsServer ? "Server: " : "");
            textChatMessage.MessageText = msg;
            try
            {
                
                m_network.SendToAllClients(textChatMessage, (textChatMessage).Sender);
            }
            catch (Exception)
            {
                Log.Instance.Warn(HES.Localization.Sentences["PlayerNotConnected"]);
            }

            if (printtoGui)
                HES.GUI.AddChatLine(String.Format("{0} - {1}: {2}", DateTime.Now.ToLocalTime(), textChatMessage.Name, msg));

            if (!printToConsole)
                return;
            chatlogger.Info((string)textChatMessage.Name + " : " + msg);

            
        }

        public void MessageToClient(string msg, string SenderName, string ReceiverName)
        {
            if (string.IsNullOrEmpty(msg))
                return;
            byte[] guid = Guid.NewGuid().ToByteArray();

            TextChatMessage textChatMessage = new TextChatMessage();

            textChatMessage.GUID = BitConverter.ToInt64(guid, 0);
            textChatMessage.Name = (SenderName);
            textChatMessage.MessageText = msg;

            Player player = (Player)null;
            if (ConnectedPlayer(ReceiverName, out player))
            {
                m_network.SendToGameClient(GetClient(player).ClientGUID, textChatMessage);
                chatlogger.Info((string)textChatMessage.Name + "->" + ReceiverName + ": " + msg);
            }
            else
                Console.WriteLine(HES.Localization.Sentences["PlayerNotConnected"]);
        }

        public Client GetClient(Player player)
        {
            if (player == null)
                return (Client)null;
            foreach (var client in ClientList)
            {
                if (client.Value.Player == player)
                    return client.Value;
            }
            return (Client)null;
        }

        public bool ConnectedPlayer(string name, out Player player)
        {
            player = (Player)null;
            foreach (var client in ClientList)
            {
                if (client.Value.Player.Name == name)
                {
                    player = client.Value.Player;
                    return true;
                }
                
            }
            return false;
        }

        public bool ConnectedPlayer(long senderId, out Player player)
        {
            player = null;
            if (!ClientList.ContainsKey(senderId))
                return false;
            player = ClientList[senderId].Player;
            return true;
        }

        public bool ConnectedPlayer(string name)
        {
            Player player = (Player)null;
            return ConnectedPlayer(name, out player);
        }
    }
}
