// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.HandheldAsteroidScanner
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  public class HandheldAsteroidScanner : Item
  {
    private HandheldAsteroidScannerStats hasStats;
    private int penetrationLevel;
    private Asteroid currentAsteroid;

    public override DynamicObjectStats StatsNew
    {
      get
      {
        return (DynamicObjectStats) this.hasStats;
      }
    }

    public HandheldAsteroidScanner(IDynamicObjectAuxData data)
    {
      if (data == null)
        return;
      this.SetData(data as HandheldAsteroidScannerData);
    }

    private void SetData(HandheldAsteroidScannerData hasd)
    {
      this.penetrationLevel = hasd.penetrationLevel;
      this.Health = hasd.Health;
      this.MaxHealth = hasd.MaxHealth;
    }

    public void UpdateResources(double dbl)
    {
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      HandheldAsteroidScannerStats asteroidScannerStats = stats as HandheldAsteroidScannerStats;
      if (asteroidScannerStats.Use)
      {
        SpaceObjectVessel vessel = Server.Instance.GetVessel(asteroidScannerStats.AsteroidGuid);
        if (vessel is Asteroid)
        {
          this.currentAsteroid = vessel as Asteroid;
          this.hasStats = new HandheldAsteroidScannerStats();
          this.hasStats.resources = this.currentAsteroid.GetResourcesOnPosition(asteroidScannerStats.position.ToVector3D(), 0);
          this.DynamicObj.StatsChanged = true;
        }
      }
      return false;
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataHandheldAsteroidScanner handheldAsteroidScanner = new PersistenceObjectDataHandheldAsteroidScanner();
      this.FillPersistenceData((PersistenceObjectDataItem) handheldAsteroidScanner);
      handheldAsteroidScanner.ScannerData = new HandheldAsteroidScannerData();
      handheldAsteroidScanner.ScannerData.ItemType = this.Type;
      handheldAsteroidScanner.ScannerData.penetrationLevel = this.penetrationLevel;
      handheldAsteroidScanner.ScannerData.Health = this.Health;
      handheldAsteroidScanner.ScannerData.MaxHealth = this.MaxHealth;
      return (PersistenceObjectData) handheldAsteroidScanner;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataHandheldAsteroidScanner handheldAsteroidScanner = persistenceData as PersistenceObjectDataHandheldAsteroidScanner;
        if (handheldAsteroidScanner == null)
          Dbg.Warning((object) "PersistenceObjectDataHandheldAsteroidScanner data is null", (object) this.GUID);
        else
          this.SetData(handheldAsteroidScanner.ScannerData);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
