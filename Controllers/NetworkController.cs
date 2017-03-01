using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common;

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

            m_network = networkController;
            Console.WriteLine("Network Controller Loaded!");

            
        }

        private void TextChatMessageListener(NetworkData data)
        {
            TextChatMessage textChatMessage = data as TextChatMessage;
            Player player1 = Server.Instance.GetPlayer(textChatMessage.Sender);
            textChatMessage.GUID = player1.FakeGuid;
            textChatMessage.Name = player1.Name;

            Console.WriteLine(textChatMessage.Name + ": " + textChatMessage.MessageText);
        }

        public void MessageAllClients(string msg)
        {
            byte[] guid = Guid.NewGuid().ToByteArray();

            TextChatMessage textChatMessage = new TextChatMessage();

            textChatMessage.GUID = BitConverter.ToInt64(guid, 0);
            textChatMessage.Name = "Server";
            textChatMessage.MessageText = msg;
            m_network.SendToAllClients((NetworkData)textChatMessage, textChatMessage.Sender);
            Console.WriteLine("Server: " + msg);
        }
    }
}
