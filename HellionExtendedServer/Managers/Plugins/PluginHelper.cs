using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZeroGravity;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace HellionExtendedServer.Managers.Plugins
{
    public class PluginHelper
    {
        private Server svr;

        public virtual Server GetServer { get { return svr; } }

        public PluginHelper(Server server)
        {
            svr = server;
        }

        public void SendMessageToClient(Player p, String message, String from)
        {
            TextChatMessage packet = new TextChatMessage();
            packet.GUID = p.GUID;
            packet.Local = true;
            packet.Name = "Server";
            packet.MessageText = message;
            GetServer.NetworkController.SendToGameClient(p.GUID, packet);
        }
        public void SendMessageToClient(Player p, String message)
        {
            SendMessageToClient(p,message,"Server");
        }

        public Player GetPlayer(String name)
        {
            //TODO
            return null;
        }

    }
}
