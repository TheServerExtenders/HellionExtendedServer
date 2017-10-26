// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.NetworkController
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using ZeroGravity.Helpers;
using ZeroGravity.Objects;

namespace ZeroGravity.Network
{
  public class NetworkController
  {
    public ThreadSafeDictionary<long, NetworkController.Client> clientList = new ThreadSafeDictionary<long, NetworkController.Client>();
    public int GameClientPort = 6004;
    public int StatusPort = 6005;
    public string MainServerAddres = "188.166.144.65";
    public int MainServerPort = 6001;
    public long ServerID = 0;
    private GameClientConnectionListener gameClientConnectionListener;
    private StatusPortConnectionListener statusPortConnectionListener;
    private MainServerThreads mainServerThreads;
    public EventSystem EventSystem;

    public NetworkController()
    {
      this.EventSystem = new EventSystem();
      this.mainServerThreads = new MainServerThreads(5);
      this.EventSystem.AddListener(typeof (LogInRequest), new EventSystem.NetworkDataDelegate(this.LogInRequestListener));
      this.EventSystem.AddListener(typeof (LogOutRequest), new EventSystem.NetworkDataDelegate(this.LogOutRequestListener));
    }

    public short CurrentOnlinePlayers()
    {
      return (short) this.clientList.Count;
    }

    public void SendDestroyPlayerMessage(long guid, long fakeGUID)
    {
      this.clientList.Lock();
      foreach (KeyValuePair<long, NetworkController.Client> client in this.clientList)
      {
        if (client.Value.Player != null && client.Value.Player.GUID != guid)
          this.SendToGameClient(client.Value.Player.GUID, (NetworkData) new DestroyObjectMessage()
          {
            ObjectType = SpaceObjectType.Player,
            ID = fakeGUID
          });
      }
      this.clientList.Unlock();
    }

    public void DisconnectClient(long guid)
    {
      if (!this.clientList.ContainsKey(guid))
        return;
      this.DisconnectClient(this.clientList[guid]);
    }

    public void DisconnectClient(NetworkController.Client cl)
    {
      if (cl.Player != null)
      {
        cl.Player.LogoutDisconnectReset();
        cl.Player.DiconnectFromNetworkContoller();
        cl.Thread.Stop();
        if (!this.clientList.ContainsKey(cl.Player.GUID))
          return;
        this.clientList.Remove(cl.Player.GUID);
      }
      else
      {
        cl.Thread.Stop();
        if (this.clientList.ContainsKey(cl.ClientGUID))
          this.clientList.Remove(cl.ClientGUID);
      }
    }

    public void DisconnectAllClients()
    {
      foreach (long key in (IEnumerable<long>) this.clientList.Keys)
        this.DisconnectClient(key);
    }

    public void SendCharacterSpawnToOtherPlayers(Player spawnedPlayer)
    {
      if (!this.clientList.ContainsKey(spawnedPlayer.GUID))
        return;
      SpawnObjectsResponse spawnObjectsResponse = new SpawnObjectsResponse();
      spawnObjectsResponse.Data.Add(spawnedPlayer.GetSpawnResponseData((Player) null));
      foreach (KeyValuePair<long, NetworkController.Client> client in this.clientList)
      {
        Player player = client.Value.Player;
        if (player != null && player != spawnedPlayer && (player.IsAlive && player.EnviromentReady) && player.IsSubscribedTo((SpaceObject) spawnedPlayer, true))
          this.SendToGameClient(player.GUID, (NetworkData) spawnObjectsResponse);
      }
    }

    public void AddCharacterSpawnsToResponse(Player pl, ref SpawnObjectsResponse res)
    {
      if (pl == null)
        return;
      foreach (Player allPlayer in Server.Instance.AllPlayers)
      {
        if (pl != allPlayer && allPlayer.IsAlive && pl.IsSubscribedTo((SpaceObject) allPlayer, true))
          res.Data.Add(allPlayer.GetSpawnResponseData(pl));
      }
    }

    public void LogInRequestListener(NetworkData data)
    {
      LogInRequest logInRequest = data as LogInRequest;
      if ((int) Server.Instance.NetworkController.CurrentOnlinePlayers() >= Server.Instance.MaxPlayers)
      {
        Dbg.Warning((object) "Maximum number of players exceeded.", (object) logInRequest.ServerID, (object) this.ServerID);
        this.SendToGameClient(logInRequest.Sender, (NetworkData) new LogInResponse()
        {
          Response = ResponseResult.Error
        });
      }
      else if ((int) logInRequest.ClientHash != (int) Server.CombinedHash)
      {
        Dbg.Warning((object) "Server/client hash mismatch.", (object) logInRequest.ServerID, (object) this.ServerID);
        this.SendToGameClient(logInRequest.Sender, (NetworkData) new LogInResponse()
        {
          Response = ResponseResult.Error
        });
      }
      else if (logInRequest.ServerID != this.ServerID)
      {
        Dbg.Warning((object) "LogInRequest server ID doesn't match this server ID.", (object) logInRequest.ServerID, (object) this.ServerID);
        this.SendToGameClient(logInRequest.Sender, (NetworkData) new LogInResponse()
        {
          Response = ResponseResult.Error
        });
      }
      else
      {
        if (logInRequest.Password == null)
          logInRequest.Password = "";
        if (logInRequest.Password != Server.Instance.ServerPassword)
        {
          Dbg.Warning((object) "LogInRequest server password doesn't match this server's password.", (object) logInRequest.ServerID, (object) this.ServerID);
          this.SendToGameClient(logInRequest.Sender, (NetworkData) new LogInResponse()
          {
            Response = ResponseResult.WrongPassword
          });
        }
        else
        {
          long guid = GUIDFactory.SteamIdToGuid(logInRequest.SteamId);
          if (this.clientList.ContainsKey(guid))
          {
            try
            {
              if (this.clientList.ContainsKey(guid))
                Server.Instance.NetworkController.DisconnectClient(this.clientList[guid]);
              if (!this.clientList.ContainsKey(logInRequest.Sender))
                return;
              Server.Instance.NetworkController.DisconnectClient(this.clientList[logInRequest.Sender]);
            }
            catch (Exception ex)
            {
              if (this.clientList.ContainsKey(guid))
                this.DisconnectClient(guid);
              if (this.clientList.ContainsKey(logInRequest.Sender))
                this.clientList[logInRequest.Sender].ClientGUID = guid;
              this.clientList.Add(guid, this.clientList[logInRequest.Sender]);
              this.clientList.Remove(logInRequest.Sender);
              Server.Instance.LoginPlayer(guid, logInRequest.SteamId, logInRequest.CharacterData);
            }
          }
          else
          {
            if (this.clientList.ContainsKey(logInRequest.Sender))
              this.clientList[logInRequest.Sender].ClientGUID = guid;
            this.clientList.Add(guid, this.clientList[logInRequest.Sender]);
            this.clientList.Remove(logInRequest.Sender);
            Server.Instance.LoginPlayer(guid, logInRequest.SteamId, logInRequest.CharacterData);
          }
        }
      }
    }

    public void Start()
    {
      this.gameClientConnectionListener = new GameClientConnectionListener();
      this.gameClientConnectionListener.Start(this.GameClientPort);
      this.statusPortConnectionListener = new StatusPortConnectionListener();
      this.statusPortConnectionListener.Start(this.StatusPort);
    }

    public void SendToMainServer(NetworkData data)
    {
      this.mainServerThreads.Send(data);
    }

    public NetworkController.Client AddClient(Socket c, GameClientThread thr)
    {
      NetworkController.Client client = new NetworkController.Client(c, thr);
      long key = -1;
      try
      {
        this.clientList.Lock();
        while (this.clientList.InnerDictionary.ContainsKey(key))
          --key;
      }
      finally
      {
        client.ClientGUID = key;
        this.clientList.Add(key, client);
        this.clientList.Unlock();
      }
      return client;
    }

    public void SendToGameClient(long clientID, NetworkData data)
    {
      if (!this.clientList.ContainsKey(clientID))
        return;
      data.Sender = 0L;
      this.clientList[clientID].Thread.Send(data);
    }

    public void LogOutRequestListener(NetworkData data)
    {
      if (!this.clientList.ContainsKey(data.Sender))
        return;
      this.LogOutPlayer((data as LogOutRequest).Sender);
      GameClientThread thread = this.clientList[data.Sender].Thread;
      LogOutResponse logOutResponse = new LogOutResponse();
      long num1 = 0;
      logOutResponse.Sender = num1;
      int num2 = 1;
      logOutResponse.Response = (ResponseResult) num2;
      thread.ClearEverytingAndSend((NetworkData) logOutResponse);
    }

    public void LogOutPlayer(long GUID)
    {
      if (!this.clientList.ContainsKey(GUID) || this.clientList[GUID].Player == null)
        return;
      this.clientList[GUID].Player.LogoutDisconnectReset();
    }

    public void ConnectPlayer(Player player, bool doLogin)
    {
      if (!this.clientList.ContainsKey(player.GUID))
        return;
      this.clientList[player.GUID].Player = player;
      this.clientList[player.GUID].Player.ConnectToNetworkController();
      LogInResponse logInResponse = new LogInResponse();
      logInResponse.ID = player.FakeGuid;
      logInResponse.Name = player.Name;
      logInResponse.ServerTime = Server.Instance.SolarSystem.CurrentTime;
      logInResponse.IsAlive = player.IsAlive;
      logInResponse.CanContinue = player.AuthorizedSpawnPoint != null;
      if (!player.IsAlive)
        logInResponse.SpawnPointsList = Server.Instance.GetAvailableSpawnPoints(player);
      this.SendToGameClient(player.GUID, (NetworkData) logInResponse);
    }

    public void SendToAllClients(NetworkData data, long skipPlayerGUID = -1)
    {
      this.clientList.Lock();
      foreach (KeyValuePair<long, NetworkController.Client> client in this.clientList)
      {
        Player player = client.Value.Player;
        if (player != null && player.IsAlive && player.EnviromentReady && player.GUID != skipPlayerGUID)
          this.SendToGameClient(client.Value.Player.GUID, data);
      }
      this.clientList.Unlock();
    }

    public void SendToClientsSubscribedTo(NetworkData data, long skipPlalerGUID = -1, params SpaceObject[] spaceObjects)
    {
      if (spaceObjects.Length == 0)
        return;
      this.clientList.Lock();
      foreach (KeyValuePair<long, NetworkController.Client> client in this.clientList)
      {
        Player player = client.Value.Player;
        if (player != null && player.GUID != skipPlalerGUID)
        {
          if (player.IsAlive && player.EnviromentReady)
          {
            bool flag = false;
            foreach (SpaceObject spaceObject in spaceObjects)
            {
              if (spaceObject != null && player.IsSubscribedTo(spaceObject, false))
              {
                flag = true;
                break;
              }
            }
            if (flag)
              this.SendToGameClient(player.GUID, data);
          }
          else if (!player.EnviromentReady && data is ShipStatsMessage)
            player.MessagesReceivedWhileLoading.Add(data as ShipStatsMessage);
        }
      }
      this.clientList.Unlock();
    }

    public void SendToClientsSubscribedToParents(NetworkData data, SpaceObject spaceObject, long skipPlalerGUID = -1, int depth = 4)
    {
      List<SpaceObject> spaceObjectList = new List<SpaceObject>();
      spaceObjectList.Add(spaceObject);
      for (SpaceObject parent = spaceObject.Parent; parent != null && depth > 0; --depth)
      {
        spaceObjectList.Add(parent);
        parent = parent.Parent;
      }
      this.SendToClientsSubscribedTo(data, skipPlalerGUID, spaceObjectList.ToArray());
    }

    private void OnApplicationQuit()
    {
      this.statusPortConnectionListener.Stop();
      this.gameClientConnectionListener.Stop();
      foreach (KeyValuePair<long, NetworkController.Client> client in this.clientList)
      {
        client.Value.Thread.Stop();
        if (client.Value.Player != null)
        {
          client.Value.Player.LogoutDisconnectReset();
          client.Value.Player.DiconnectFromNetworkContoller();
        }
      }
      this.clientList.Clear();
    }

    public long GetClientGuid(NetworkController.Client c)
    {
      foreach (KeyValuePair<long, NetworkController.Client> client in this.clientList)
      {
        if (client.Value == c)
          return client.Key;
      }
      throw new Exception("Client not found");
    }

    public class Client
    {
      public Player Player = (Player) null;
      public GameClientThread Thread = (GameClientThread) null;
      public long ClientGUID;
      public Socket TcpClientSocket;

      public Client(Socket soc, GameClientThread th)
      {
        this.TcpClientSocket = soc;
        this.Thread = th;
      }
    }
  }
}
