// Decompiled with JetBrains decompiler
// Type: GameServerConsole.Scripts.Data.StashPlayerVessels
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity;
using ZeroGravity.Math;
using ZeroGravity.Objects;

namespace GameServerConsole.Scripts.Data
{
  public class StashPlayerVessels : PersistenceObjectData
  {
    public List<PersistenceObjectData> LinkedVessels = new List<PersistenceObjectData>();
    public long PlayerGUID;
    public PersistenceObjectDataPlayer PlayerData;

    public Player Load()
    {
      foreach (PersistenceObjectDataShip linkedVessel in this.LinkedVessels)
        new Ship(linkedVessel.GUID, false, Vector3D.Zero, Vector3D.One, Vector3D.Forward, Vector3D.Up).LoadPersistenceData((PersistenceObjectData) linkedVessel);
      Player player = new Player(this.PlayerData.GUID, Vector3D.Zero, QuaternionD.Identity, "PersistenceLoad", "", this.PlayerData.Gender, this.PlayerData.HeadType, this.PlayerData.HairType, false);
      player.LoadPersistenceData((PersistenceObjectData) this.PlayerData);
      return player;
    }

    public StashPlayerVessels(Player p, List<Ship> shs)
    {
      if (p == null || shs == null)
        return;
      this.PlayerGUID = p.GUID;
      this.PlayerData = p.GetPersistenceData() as PersistenceObjectDataPlayer;
      foreach (Ship ship in shs)
        this.LinkedVessels.Add(ship.GetPersistenceData());
    }
  }
}
