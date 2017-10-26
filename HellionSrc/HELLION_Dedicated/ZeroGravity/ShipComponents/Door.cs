// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.Door
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using Newtonsoft.Json;
using System;
using ZeroGravity.Math;
using ZeroGravity.Network;

namespace ZeroGravity.ShipComponents
{
  public class Door : IAirConsumer, IPersistantObject
  {
    private Room _Room1 = (Room) null;
    private Room _Room2 = (Room) null;
    public VesselObjectID PairedDoorID = (VesselObjectID) null;
    public bool StatusChanged = true;
    public VesselObjectID ID;
    public bool IsSealable;
    public bool LockedAutoToggle;
    public bool HasPower;
    private bool _IsOpen;
    private bool _IsLocked;
    [JsonIgnore]
    public Vector3D PositionRelativeToDockingPort;
    public float PassageArea;

    public bool IsSealed
    {
      get
      {
        return this.IsSealable && !this.IsOpen;
      }
    }

    public bool isExternal
    {
      get
      {
        return this.Room1 == null || this.Room2 == null;
      }
    }

    public bool IsOpen
    {
      get
      {
        return this._IsOpen;
      }
      set
      {
        if (value == this._IsOpen)
          return;
        this.StatusChanged = true;
        this._IsOpen = value;
      }
    }

    public bool IsLocked
    {
      get
      {
        return this._IsLocked;
      }
      set
      {
        if (value == this._IsLocked)
          return;
        this.StatusChanged = true;
        this._IsLocked = value;
      }
    }

    [JsonIgnore]
    public Room Room1
    {
      get
      {
        return this._Room1;
      }
      set
      {
        if (value == this._Room1)
          return;
        this.StatusChanged = true;
        this._Room1 = value;
      }
    }

    [JsonIgnore]
    public Room Room2
    {
      get
      {
        return this._Room2;
      }
      set
      {
        if (value == this._Room2)
          return;
        this.StatusChanged = true;
        this._Room2 = value;
      }
    }

    public float AirQualityDegradationRate
    {
      get
      {
        return 0.0f;
      }
    }

    public float AirQuantityDecreaseRate
    {
      get
      {
        if (!this.isExternal || this.IsSealed)
          return 0.0f;
        float num = 0.0f;
        if (this.Room1 != null && this.Room2 == null)
          num = this.Room1.AirPressure;
        else if (this.Room1 == null && this.Room2 != null)
          num = this.Room2.AirPressure;
        else if (this.Room1 != null && this.Room2 != null)
          num = System.Math.Abs(this.Room1.AirPressure - this.Room2.AirPressure);
        return (float) (0.61 * (double) this.PassageArea * System.Math.Sqrt(2.0 * (double) num * 100000.0 / 1.225));
      }
    }

    public bool AffectsQuality
    {
      get
      {
        return false;
      }
    }

    public bool AffectsQuantity
    {
      get
      {
        return this.isExternal && !this.IsSealed;
      }
    }

    public DoorDetails GetDetails()
    {
      DoorDetails doorDetails = new DoorDetails();
      doorDetails.InSceneID = this.ID.InSceneID;
      int num1 = this.HasPower ? 1 : 0;
      doorDetails.HasPower = num1 != 0;
      int num2 = this.IsLocked ? 1 : 0;
      doorDetails.IsLocked = num2 != 0;
      int num3 = this.IsOpen ? 1 : 0;
      doorDetails.IsOpen = num3 != 0;
      VesselObjectID vesselObjectId1 = this.Room1 != null ? this.Room1.ID : (VesselObjectID) null;
      doorDetails.Room1ID = vesselObjectId1;
      VesselObjectID vesselObjectId2 = this.Room2 != null ? this.Room2.ID : (VesselObjectID) null;
      doorDetails.Room2ID = vesselObjectId2;
      return doorDetails;
    }

    public PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataDoor persistenceObjectDataDoor = new PersistenceObjectDataDoor();
      persistenceObjectDataDoor.InSceneID = this.ID.InSceneID;
      int num1 = this.HasPower ? 1 : 0;
      persistenceObjectDataDoor.HasPower = num1 != 0;
      int num2 = this.IsLocked ? 1 : 0;
      persistenceObjectDataDoor.IsLocked = num2 != 0;
      int num3 = this.IsOpen ? 1 : 0;
      persistenceObjectDataDoor.IsOpen = num3 != 0;
      return (PersistenceObjectData) persistenceObjectDataDoor;
    }

    public void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        PersistenceObjectDataDoor persistenceObjectDataDoor = persistenceData as PersistenceObjectDataDoor;
        if (persistenceObjectDataDoor == null)
        {
          Dbg.Warning("PersistenceObjectDataDoor data is null");
        }
        else
        {
          this.HasPower = persistenceObjectDataDoor.HasPower;
          this.IsLocked = persistenceObjectDataDoor.IsLocked;
          this.IsOpen = persistenceObjectDataDoor.IsOpen;
        }
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
