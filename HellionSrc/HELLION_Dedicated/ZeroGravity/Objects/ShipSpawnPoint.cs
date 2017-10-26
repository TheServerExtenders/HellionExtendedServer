// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.ShipSpawnPoint
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  public class ShipSpawnPoint
  {
    public int SpawnPointID;
    public SpawnPointType Type;
    public Player Player;
    public bool IsPlayerInSpawnPoint;
    public string InvitedPlayerSteamID;
    public string InvitedPlayerName;
    public SceneTriggerExecuter Executer;
    public int ExecuterStateID;
    public Ship Ship;
    public SpawnPointState State;
    public List<int> ExecuterOccupiedStateIDs;

    public SpawnPointStats SetStats(SpawnPointStats stats, Player sender)
    {
      if (this.Type == SpawnPointType.SimpleSpawn)
        return (SpawnPointStats) null;
      if (stats.PlayerInvite.HasValue)
      {
        if (this.State == SpawnPointState.Authorized || this.State == SpawnPointState.Locked && sender != this.Player)
          return (SpawnPointStats) null;
        this.State = SpawnPointState.Locked;
        this.Player = sender;
        if (Server.Instance.PlayerInviteChanged(this, stats.InvitedPlayerSteamID, stats.InvitedPlayerName, sender))
          return (SpawnPointStats) null;
        return new SpawnPointStats()
        {
          InSceneID = this.SpawnPointID,
          NewState = new SpawnPointState?(this.State),
          PlayerGUID = new long?(this.Player != null ? this.Player.FakeGuid : -1L),
          PlayerName = this.Player != null ? this.Player.Name : "",
          PlayerSteamID = this.Player != null ? this.Player.SteamId : "",
          InvitedPlayerName = this.InvitedPlayerName,
          InvitedPlayerSteamID = this.InvitedPlayerSteamID
        };
      }
      if (!stats.NewState.HasValue)
        return (SpawnPointStats) null;
      SpawnPointState? newState = stats.NewState;
      SpawnPointState spawnPointState1 = SpawnPointState.Unlocked;
      if ((newState.GetValueOrDefault() == spawnPointState1 ? (newState.HasValue ? 1 : 0) : 0) != 0 && this.State == SpawnPointState.Locked && sender == this.Player)
      {
        this.State = SpawnPointState.Unlocked;
        this.Player = (Player) null;
        if (!this.InvitedPlayerSteamID.IsNullOrEmpty())
          Server.Instance.PlayerInviteChanged(this, "", "", sender);
        else
          return new SpawnPointStats()
          {
            InSceneID = this.SpawnPointID,
            NewState = new SpawnPointState?(this.State),
            PlayerGUID = new long?(-1L)
          };
      }
      else
      {
        int num;
        if (stats.HackUnlock.HasValue && stats.HackUnlock.Value)
        {
          newState = stats.NewState;
          SpawnPointState spawnPointState2 = SpawnPointState.Unlocked;
          if ((newState.GetValueOrDefault() == spawnPointState2 ? (newState.HasValue ? 1 : 0) : 0) != 0 && this.State == SpawnPointState.Locked && (sender != this.Player && sender.CurrentActiveItem != null))
          {
            num = ItemTypeRange.IsHackingTool(sender.CurrentActiveItem.Type) ? 1 : 0;
            goto label_18;
          }
        }
        num = 0;
label_18:
        if (num != 0)
        {
          this.State = SpawnPointState.Unlocked;
          this.Player = (Player) null;
          sender.CurrentActiveItem.ChangeStats((DynamicObjectStats) new DisposableHackingToolStats()
          {
            Use = true
          });
          if (!this.InvitedPlayerSteamID.IsNullOrEmpty())
            Server.Instance.PlayerInviteChanged(this, "", "", sender);
          else
            return new SpawnPointStats()
            {
              InSceneID = this.SpawnPointID,
              NewState = new SpawnPointState?(this.State),
              PlayerGUID = new long?(-1L)
            };
        }
        else
        {
          newState = stats.NewState;
          SpawnPointState spawnPointState2 = SpawnPointState.Locked;
          if ((newState.GetValueOrDefault() == spawnPointState2 ? (newState.HasValue ? 1 : 0) : 0) != 0 && this.Player == null && this.State == SpawnPointState.Unlocked)
          {
            this.State = SpawnPointState.Locked;
            this.Player = sender;
            return new SpawnPointStats()
            {
              InSceneID = this.SpawnPointID,
              NewState = new SpawnPointState?(this.State),
              PlayerGUID = new long?(sender.FakeGuid),
              PlayerName = sender.Name,
              PlayerSteamID = sender.SteamId
            };
          }
          newState = stats.NewState;
          SpawnPointState spawnPointState3 = SpawnPointState.Authorized;
          if (newState.GetValueOrDefault() == spawnPointState3 && newState.HasValue)
          {
            if (sender.AuthorizedSpawnPoint != null && sender.AuthorizedSpawnPoint.State == SpawnPointState.Authorized)
            {
              sender.AuthorizedSpawnPoint.State = SpawnPointState.Locked;
              ShipStatsMessage shipStatsMessage = new ShipStatsMessage();
              shipStatsMessage.GUID = sender.AuthorizedSpawnPoint.Ship.GUID;
              shipStatsMessage.Temperature = new float?(sender.AuthorizedSpawnPoint.Ship.Temperature);
              shipStatsMessage.Health = new float?(sender.AuthorizedSpawnPoint.Ship.Health);
              shipStatsMessage.VesselObjects = new VesselObjects();
              shipStatsMessage.VesselObjects.SpawnPoints = new List<SpawnPointStats>();
              shipStatsMessage.VesselObjects.SpawnPoints.Add(new SpawnPointStats()
              {
                InSceneID = sender.AuthorizedSpawnPoint.SpawnPointID,
                NewState = new SpawnPointState?(sender.AuthorizedSpawnPoint.State),
                PlayerGUID = new long?(sender.FakeGuid),
                PlayerName = sender.Name,
                PlayerSteamID = sender.SteamId
              });
              Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) shipStatsMessage, -1L, (SpaceObject) sender.AuthorizedSpawnPoint.Ship);
            }
            this.State = SpawnPointState.Authorized;
            this.Player = sender;
            sender.SetSpawnPoint(this);
            return new SpawnPointStats()
            {
              InSceneID = this.SpawnPointID,
              NewState = new SpawnPointState?(this.State),
              PlayerGUID = new long?(sender.FakeGuid),
              PlayerName = sender.Name,
              PlayerSteamID = sender.SteamId
            };
          }
        }
      }
      return (SpawnPointStats) null;
    }

    public void SetInvitation(string id, string name, bool sendMessage)
    {
      if (id == this.InvitedPlayerSteamID || this.InvitedPlayerSteamID.IsNullOrEmpty() && id.IsNullOrEmpty())
        return;
      this.InvitedPlayerSteamID = id;
      this.InvitedPlayerName = name;
      if (!sendMessage)
        return;
      ShipStatsMessage shipStatsMessage = new ShipStatsMessage();
      shipStatsMessage.GUID = this.Ship.GUID;
      shipStatsMessage.Temperature = new float?(this.Ship.Temperature);
      shipStatsMessage.Health = new float?(this.Ship.Health);
      shipStatsMessage.VesselObjects = new VesselObjects();
      shipStatsMessage.VesselObjects.SpawnPoints = new List<SpawnPointStats>();
      shipStatsMessage.VesselObjects.SpawnPoints.Add(new SpawnPointStats()
      {
        InSceneID = this.SpawnPointID,
        NewState = new SpawnPointState?(this.State),
        PlayerGUID = new long?(this.Player != null ? this.Player.FakeGuid : -1L),
        PlayerName = this.Player != null ? this.Player.Name : "",
        PlayerSteamID = this.Player != null ? this.Player.SteamId : "",
        PlayerInvite = new bool?(true),
        InvitedPlayerName = this.InvitedPlayerName,
        InvitedPlayerSteamID = this.InvitedPlayerSteamID
      });
      Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) shipStatsMessage, -1L, (SpaceObject) this.Ship);
    }

    public void AuthorizePlayerToSpawnPoint(Player pl, bool sendMessage)
    {
      if (this.Type == SpawnPointType.SimpleSpawn || this.Player == pl && this.State == SpawnPointState.Authorized)
        return;
      this.State = SpawnPointState.Authorized;
      this.Player = pl;
      pl.SetSpawnPoint(this);
      if (!sendMessage)
        return;
      ShipStatsMessage shipStatsMessage = new ShipStatsMessage();
      shipStatsMessage.GUID = this.Ship.GUID;
      shipStatsMessage.Temperature = new float?(this.Ship.Temperature);
      shipStatsMessage.Health = new float?(this.Ship.Health);
      shipStatsMessage.VesselObjects = new VesselObjects();
      shipStatsMessage.VesselObjects.SpawnPoints = new List<SpawnPointStats>();
      shipStatsMessage.VesselObjects.SpawnPoints.Add(new SpawnPointStats()
      {
        InSceneID = this.SpawnPointID,
        NewState = new SpawnPointState?(this.State),
        PlayerGUID = new long?(this.Player.FakeGuid),
        PlayerName = this.Player.Name,
        PlayerSteamID = this.Player.SteamId
      });
      Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) shipStatsMessage, -1L, (SpaceObject) this.Ship);
    }
  }
}
