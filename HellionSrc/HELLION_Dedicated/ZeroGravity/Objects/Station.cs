// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.Station
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Math;

namespace ZeroGravity.Objects
{
  public class Station : SpaceObjectVessel
  {
    private List<Vector3D> playerSpawnPoints = new List<Vector3D>();
    private List<Vector3D> shipSpawnPoints = new List<Vector3D>();

    public override SpaceObjectType ObjectType
    {
      get
      {
        return SpaceObjectType.Station;
      }
    }

    public bool CanSpawnPlayers
    {
      get
      {
        return this.playerSpawnPoints.Count > 0;
      }
    }

    public bool CanSpawnShips
    {
      get
      {
        return this.playerSpawnPoints.Count > 0;
      }
    }

    public Station(long guid, bool initializeOrbit, Vector3D position, Vector3D velocity, Vector3D forward, Vector3D up)
      : base(guid, initializeOrbit, position, velocity, forward, up)
    {
    }

    ~Station()
    {
    }

    public void PositionUpdated()
    {
    }

    public bool GetPlayerSpawnPoint(out Vector3D spawnPoint)
    {
      spawnPoint = Vector3D.Zero;
      if (!this.CanSpawnPlayers)
        return false;
      spawnPoint = this.playerSpawnPoints[0];
      return true;
    }

    public override void RemovePlayerFromCrew(Player pl, bool checkDetails = false)
    {
      throw new NotImplementedException();
    }

    public override void AddPlayerToCrew(Player pl)
    {
      throw new NotImplementedException();
    }

    public override bool HasPlayerInCrew(Player pl)
    {
      throw new NotImplementedException();
    }
  }
}
