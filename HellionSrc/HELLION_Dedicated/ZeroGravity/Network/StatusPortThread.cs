// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.StatusPortThread
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Net.Sockets;
using System.Threading;
using ZeroGravity.Objects;

namespace ZeroGravity.Network
{
  public class StatusPortThread
  {
    private Socket socket;
    private Thread listeningThread;

    public StatusPortThread(Socket soc)
    {
      this.socket = soc;
    }

    public void Start()
    {
      this.listeningThread = new Thread(new ThreadStart(this.Listen));
      this.listeningThread.IsBackground = true;
      this.listeningThread.Start();
    }

    public void Stop()
    {
      this.socket.Close();
    }

    private void Listen()
    {
      try
      {
        NetworkData data = Serializer.ReceiveData(this.socket);
        if (data == null)
          return;
        string address = this.socket.RemoteEndPoint.ToString().Split(":".ToCharArray(), 2)[0];
        if (data is ServerShutDownMessage)
        {
          if (!Server.Instance.IsAddressAutorized(address))
            return;
          ServerShutDownMessage serverShutDownMessage = data as ServerShutDownMessage;
          Server.Restart = serverShutDownMessage.Restrat;
          Server.CleanRestart = serverShutDownMessage.CleanRestart;
          Server.SavePersistenceDataOnShutdown = !Server.Restart && Server.PersistenceSaveInterval > 0.0 || Server.Restart && !Server.CleanRestart;
          Server.IsRunning = false;
        }
        else if (data is ServerStatusRequest)
        {
          ServerStatusRequest serverStatusRequest = data as ServerStatusRequest;
          ServerStatusResponse serverStatusResponse = new ServerStatusResponse();
          serverStatusResponse.Response = ResponseResult.Success;
          serverStatusResponse.Description = serverStatusRequest.SendDetails ? Server.Instance.Description : (string) null;
          serverStatusResponse.NotificationEMail = serverStatusRequest.SendDetails ? Server.Instance.NotificationEMail : (string) null;
          serverStatusResponse.MaxPlayers = (short) Server.Instance.MaxPlayers;
          serverStatusResponse.CurrentPlayers = Server.Instance.NetworkController.CurrentOnlinePlayers();
          Player playerFromSteamId = Server.Instance.GetPlayerFromSteamID(serverStatusRequest.SteamId);
          if (playerFromSteamId != null)
            serverStatusResponse.CharacterData = playerFromSteamId.GetCharacterData();
          try
          {
            this.socket.Send(Serializer.Serialize((NetworkData) serverStatusResponse));
          }
          catch (ArgumentNullException ex)
          {
            Dbg.Error((object) "Serialized data buffer is null", (object) data.GetType().ToString(), (object) data);
            throw;
          }
        }
        else if (data is DeleteCharacterRequest)
        {
          DeleteCharacterRequest characterRequest = data as DeleteCharacterRequest;
          if (characterRequest.ServerId != Server.Instance.NetworkController.ServerID)
            return;
          Player playerFromSteamId = Server.Instance.GetPlayerFromSteamID(characterRequest.SteamId);
          if (!Server.Instance.NetworkController.clientList.Keys.Contains(playerFromSteamId.GUID))
            playerFromSteamId.Destroy();
        }
      }
      catch (Exception ex)
      {
      }
    }
  }
}
