using System;
using System.IO;
using System.Collections.Generic;
using ZeroGravity;
using ZeroGravity.Helpers;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;
using static ZeroGravity.Network.NetworkController;
using HellionExtendedServer.Managers;
using NLog;

namespace HellionExtendedServer.Controllers
{
    public class NetworkController
    {
        #region Fields

        private static NetworkController m_networkController;
        private ZeroGravity.Network.NetworkController m_network;
        private static readonly Logger chatlogger = LogManager.GetCurrentClassLogger();

        #endregion Fields

        #region Properties

        public static NetworkController Instance { get { return m_networkController; } }
        public ThreadSafeDictionary<long, Client> ClientList { get { return m_network.clientList; } }

        #endregion Properties

        public NetworkController(ZeroGravity.Network.NetworkController networkController)
        {


            m_networkController = this;

            // Hook into the Chat Message event and add in ours along side the original
            networkController.EventSystem.AddListener(typeof(TextChatMessage), new EventSystem.NetworkDataDelegate(this.TextChatMessageListener));
            Log.Instance.Info(HES.Localization.Sentences["ChatMsgListener"]);

            // Hook into the player spawn event and add in ours as well!
            networkController.EventSystem.AddListener(typeof(PlayerSpawnRequest), new EventSystem.NetworkDataDelegate(this.PlayerSpawnRequestListener));
            Log.Instance.Info(HES.Localization.Sentences["PlayerSpawnListener"]);

            // [IN TEST] Could be used to detect when the player is physicly in the server
            networkController.EventSystem.AddListener(typeof(PlayersOnServerRequest), new EventSystem.NetworkDataDelegate(this.PlayerOnServerListener));
            Log.Instance.Info(HES.Localization.Sentences["PlayerOnServerListener"]);

            m_network = networkController;
            Log.Instance.Info(HES.Localization.Sentences["NetControlerLoaded"]);
        }

        #region Event Handlers

        /// <summary>
        /// This method is invoked when the event is fired
        /// </summary>
        /// <param name="data"> the network data object</param>
        private void PlayerOnServerListener(NetworkData data)
        {
            try
            {
                PlayersOnServerRequest playerOnServerRequest = data as PlayersOnServerRequest;

                if (playerOnServerRequest == null)
                    return;

                Player ply;
                if (ConnectedPlayer(playerOnServerRequest.Sender, out ply))
                    return;

                
                Console.WriteLine(string.Format(HES.Localization.Sentences["NewPlayer"],ClientList[playerOnServerRequest.Sender].Player.Name));
                MessageAllClients(string.Format(HES.Localization.Sentences["Welcome"], ClientList[playerOnServerRequest.Sender].Player.Name, Server.Instance.ServerName));

            }
            catch (Exception ex)
            {
                Log.Instance.Error("Hellion Extended Server [ON SERVER ERROR] : " + ex.InnerException.ToString());
            }
        }

        /// <summary>
        /// This method is invoked when the event is fired
        /// </summary>
        /// <param name="data"> the network data object</param>
        private void PlayerSpawnRequestListener(NetworkData data)
        {
            try
            {
                PlayerSpawnRequest playerSpawnRequest = data as PlayerSpawnRequest;

                if (playerSpawnRequest == null)
                    return;

                Player ply;
                if (ConnectedPlayer(playerSpawnRequest.Sender, out ply))
                    return;

                chatlogger.Info(string.Format(HES.Localization.Sentences["PlayerSpawnLog"], ply.Name, ply.SteamId));
                MessageAllClients(string.Format(HES.Localization.Sentences["PlayerSpawnChat"], ply.Name), false, false);
            }
            catch (Exception ex)
            {
                Log.Instance.Error("Hellion Extended Server [SPAWN REQUEST ERROR] : " + ex.InnerException.ToString());
            }
        }

        /// <summary>
        /// This method is invoked when the event is fired
        /// </summary>
        /// <param name="data"> the network data object</param>
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

        #endregion Event Handlers

        #region Methods

        /// <summary>
        ///
        /// </summary>
        /// <param name="msg">the message to send</param>
        /// <param name="sendAsServer"> sends Server as the name of the message</param>
        /// <param name="printToConsole"> prints the message to the console</param>
        public void MessageAllClients(string msg, bool sendAsServer = true, bool printToConsole = true)
        {
            if (String.IsNullOrEmpty(msg))
                return;

            byte[] guid = Guid.NewGuid().ToByteArray();

            TextChatMessage textChatMessage = new TextChatMessage();

            textChatMessage.GUID = BitConverter.ToInt64(guid, 0);
            textChatMessage.Name = (sendAsServer ? "Server" : "");
            textChatMessage.MessageText = msg;
            try
            {
                m_network.SendToAllClients((NetworkData)textChatMessage, textChatMessage.Sender);
            }
            catch (Exception ex)
            {
                Log.Instance.Warn(HES.Localization.Sentences["PlayerNotConnected"]);
            }

            if (printToConsole)
                chatlogger.Info(textChatMessage.Name + " : " + msg);
        }

        /// <summary>
        /// This method allow to send a message to a specific player
        /// </summary>
        /// <param name="msg">Message to send</param>
        /// <param name="SenderName">Name of the sender (Server for exemple)</param>
        /// <param name="ReceiverName">Name or steamid of the receiver</param>
        /// <param name="steamId">True if the receiverName is the steamID</param>
        /// <returns></returns>
        public void MessageToClient(string msg, string SenderName, string ReceiverName)
        {
            if (String.IsNullOrEmpty(msg))
                return;

            byte[] guid = Guid.NewGuid().ToByteArray();

            TextChatMessage textChatMessage = new TextChatMessage();

            textChatMessage.GUID = BitConverter.ToInt64(guid, 0);
            textChatMessage.Name = (SenderName);
            textChatMessage.MessageText = msg;

            Player receiver = null;
            if (ConnectedPlayer(ReceiverName, out receiver))
            {
                Client recClient = GetClient(receiver);
                m_network.SendToGameClient(recClient.ClientGUID, (NetworkData)textChatMessage);
                chatlogger.Info(textChatMessage.Name + "->" + ReceiverName + ": " + msg);
            }
            else
            {
                Console.WriteLine(HES.Localization.Sentences["PlayerNotConnected"]);
            }
        }

        /// <summary>
        /// This method allow to get the client of a specific player (usefull to get his ClientGUID
        /// </summary>
        /// <param name="player">Researched player</param>
        /// <returns></returns>
        public Client GetClient(Player player)
        {
            if (player == null)
                return null;

            ThreadSafeDictionary<long, Client> clients = m_network.clientList;
            foreach (var client in clients)
            {
                if (client.Value.Player == player)
                    return client.Value;
            }

            return null;
        }

        #region ConnectedPlayer part

        public bool ConnectedPlayer(string name, out Player player)
        {
            player = null;
            foreach (var item in ClientList)
            {
                if (item.Value.Player.Name == name)
                {
                    player = item.Value.Player;
                    return true;
                }
            }
            return false;
        }

        public bool ConnectedPlayer(long senderId, out Player player)
        {
            player = null;
            if(ClientList.ContainsKey(senderId))
            {
                player = ClientList[senderId].Player;
                return true;
            }
            return false;
        }

        public bool ConnectedPlayer(string name)
        {
            Player ply = null;
            return ConnectedPlayer(name, out ply);
        }

        #endregion

        #endregion Methods
    }
}