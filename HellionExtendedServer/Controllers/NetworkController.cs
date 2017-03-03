using System;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace HellionExtendedServer.Controllers
{
    public class NetworkController
    {
        #region Fields

        private static NetworkController m_networkController;
        private ZeroGravity.Network.NetworkController m_network;

        #endregion Fields

        #region Properties

        public static NetworkController Instance { get { return m_networkController; } }

        #endregion Properties

        public NetworkController(ZeroGravity.Network.NetworkController networkController)
        {
            m_networkController = this;

            // Hook into the Chat Message event and add in ours along side the original
            networkController.EventSystem.AddListener(typeof(TextChatMessage), new EventSystem.NetworkDataDelegate(this.TextChatMessageListener));
            Console.WriteLine("Chat Message Listener Added.");

            // Hook into the player spawn event and add in ours as well!
            networkController.EventSystem.AddListener(typeof(PlayerSpawnRequest), new EventSystem.NetworkDataDelegate(this.PlayerSpawnRequestListener));
            Console.WriteLine("Player Spawns Listener Added.");

            m_network = networkController;
            Console.WriteLine("Network Controller Loaded!");
        }

        #region Event Handlers

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
                if (m_network.clientList.ContainsKey(playerSpawnRequest.Sender))
                    ply = m_network.clientList[playerSpawnRequest.Sender].Player;
                else
                    return;

                Console.WriteLine(ply.Name + " spawned (" + ply.SteamId + ") ");
                MessageAllClients(ply.Name + " has spawned!", false, false);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Hellion Extended Server[SpawnRequest]:" + ex.InnerException.ToString());
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

                Console.WriteLine("(" + textChatMessage.Sender + ")" + textChatMessage.Name + ": " + textChatMessage.MessageText);
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Hellion Extended Server[Chat]:" + ex.InnerException.ToString());
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
            textChatMessage.Name = (sendAsServer ? "Server :" : "");
            textChatMessage.MessageText = msg;
            m_network.SendToAllClients((NetworkData)textChatMessage, textChatMessage.Sender);

            if (printToConsole)
                Console.WriteLine(textChatMessage.Name + "" + msg);
        }

        #endregion Methods
    }
}