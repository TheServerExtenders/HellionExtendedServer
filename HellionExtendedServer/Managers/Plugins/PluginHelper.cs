using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HellionExtendedServer.Common;
using NLog;
using ZeroGravity;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace HellionExtendedServer.Managers.Plugins
{
    public class PluginHelper
    {
        private Server svr;

        public Server GetServer { get { return svr; } }
        public Logger GetLogger{ get { return Log.Instance; } }

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
            SendMessageToClient(p, message, "Server");
        }

        public void SendMessageToServer(String message)
        {
            foreach (Player p in GetServer.AllPlayers)
            {
                SendMessageToClient(p, message, "Server");
            }
        }

        public Player getPlayerExact(String name)
        {
            foreach (Player player in ServerInstance.Instance.Server.AllPlayers)
            {
                if (player.Name.ToLower() == name.ToLower()) return player;
            }
            return null;
        }
        public Player getPlayerFromGuid(long guid)
        {
            foreach (Player player in ServerInstance.Instance.Server.AllPlayers)
            {
                if (player.GUID == guid) return player;
            }
            return null;
        }

        public Player GetPlayer(String name)
        {
            Player found = null;
            int delta = int.MaxValue;
            foreach(Player player in ServerInstance.Instance.Server.AllPlayers)
            {
                if (player.Name.ToLower().StartsWith(name))
                {
                    int curDelta = player.Name.Length - name.Length;
                    if (curDelta < delta)
                    {
                        found = player;
                        delta = curDelta;
                    }
                    if (curDelta == 0)
                    {
                        break;
                    }
                }
            }
            return found;
        }

    }
}
