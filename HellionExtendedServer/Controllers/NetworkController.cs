using System;

using ZeroGravity;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;


namespace HellionExtendedServer.Controllers
{
    public class NetworkController
    {
        private static NetworkController m_networkController;
        private ZeroGravity.Network.NetworkController m_network;


        public static NetworkController Instance { get { return m_networkController; } }

        public NetworkController(ZeroGravity.Network.NetworkController networkController)
        {
            m_networkController = this;
            networkController.EventSystem.AddListener(typeof(TextChatMessage), new EventSystem.NetworkDataDelegate(this.TextChatMessageListener));
            Console.WriteLine("Chat Message Listener Added.");

            networkController.EventSystem.AddListener(typeof(PlayerSpawnRequest), new EventSystem.NetworkDataDelegate(this.PlayerSpawnRequestListener));
            Console.WriteLine("Player Spawns Listener Added.");

            m_network = networkController;
            Console.WriteLine("Network Controller Loaded!");

            
        }

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
              
                Console.WriteLine(ply.Name + " spawned ("+ ply.SteamId +") ");
                MessageAllClients(ply.Name + " has spawned!", false);
              
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Hellion Extended Server[SpawnRequest]:" + ex.InnerException.ToString());
                
            }
           
        }

        private void TextChatMessageListener(NetworkData data)
        {
            try
            {              
                TextChatMessage textChatMessage = data as TextChatMessage;
                              
                Console.WriteLine("(" +textChatMessage.Sender+ ")" + textChatMessage.Name + ": " + textChatMessage.MessageText);
                                           
            }
            catch (Exception ex)
            {
                Console.WriteLine("[ERROR] Hellion Extended Server[Chat]:" + ex.InnerException.ToString());
                
            }                      
        }

        public void MessageAllClients(string msg, bool sendAsServer = true)
        {
            if (String.IsNullOrEmpty(msg))
                return;

            byte[] guid = Guid.NewGuid().ToByteArray();

            TextChatMessage textChatMessage = new TextChatMessage();

            textChatMessage.GUID = BitConverter.ToInt64(guid, 0);
            textChatMessage.Name = (sendAsServer ? "Server :" : "");
            textChatMessage.MessageText = msg;
            m_network.SendToAllClients((NetworkData)textChatMessage, textChatMessage.Sender);
            Console.WriteLine(textChatMessage.Name + ": " + msg);
        }

        
    }
}
