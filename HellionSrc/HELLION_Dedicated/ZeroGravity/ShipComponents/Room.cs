// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.Room
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class Room : IPersistantObject
  {
    private float _AirPressure = 1f;
    private float _AirPressureChangeRate = 0.0f;
    private float _AirQuality = 1f;
    private float _AirQualityChangeRate = 0.0f;
    private float _Temperature = 25f;
    private bool _UseGravity = false;
    [JsonIgnore]
    public HashSet<Room> LinkedRooms = new HashSet<Room>();
    public HashSet<Door> Doors = new HashSet<Door>();
    [JsonIgnore]
    public List<VesselComponent> VesselComponents = new List<VesselComponent>();
    public List<IAirConsumer> AirConsumers = new List<IAirConsumer>();
    [JsonIgnore]
    public List<ILifeSupportDevice> LifeSupportDevices = new List<ILifeSupportDevice>();
    public bool StatusChanged = true;
    [JsonIgnore]
    public DistributionManager.CompoundRoom CompoundRoom = (DistributionManager.CompoundRoom) null;
    public VesselObjectID ID;
    private bool _GravityMalfunction;
    public bool GravityAutoToggle;
    public SpaceObjectVessel ParentVessel;
    public float Volume;
    public RoomAirTank AirTank;

    public bool IsAirOk
    {
      get
      {
        return (double) this.AirPressure > -0.67 * (double) this.AirQuality + 1.0;
      }
    }

    public bool FireCanBurn
    {
      get
      {
        return (double) this.AirQuality * (double) this.AirPressure < 0.25;
      }
    }

    public bool GravityMalfunction
    {
      get
      {
        return this._GravityMalfunction;
      }
      set
      {
        if (this._GravityMalfunction == (this._GravityMalfunction = value))
          return;
        this.StatusChanged = true;
      }
    }

    public float AirQuality
    {
      get
      {
        return this._AirQuality;
      }
      set
      {
        float num = MathHelper.Clamp(value, 0.0f, 1f);
        if ((double) this._AirQuality != (double) num)
          this.StatusChanged = true;
        this._AirQuality = num;
      }
    }

    public float AirQualityChangeRate
    {
      get
      {
        return this._AirQualityChangeRate;
      }
      set
      {
        if ((double) this._AirQualityChangeRate != (double) value)
          this.StatusChanged = true;
        this._AirQualityChangeRate = value;
      }
    }

    public float AirPressure
    {
      get
      {
        return this._AirPressure;
      }
      set
      {
        float num = MathHelper.Clamp(value, 0.0f, 1f);
        if ((double) this._AirPressure != (double) num)
          this.StatusChanged = true;
        this._AirPressure = num;
      }
    }

    public float AirPressureChangeRate
    {
      get
      {
        return this._AirPressureChangeRate;
      }
      set
      {
        if ((double) this._AirPressureChangeRate != (double) value)
          this.StatusChanged = true;
        this._AirPressureChangeRate = value;
      }
    }

    public bool UseGravity
    {
      get
      {
        return this._UseGravity;
      }
      set
      {
        if (this._UseGravity != value)
          this.StatusChanged = true;
        this._UseGravity = value;
      }
    }

    public float Temperature
    {
      get
      {
        return this._Temperature;
      }
      set
      {
        if ((double) this._Temperature != (double) value)
          this.StatusChanged = true;
        this._Temperature = value;
      }
    }

    public PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataRoom persistenceObjectDataRoom = new PersistenceObjectDataRoom();
      long guid = this.ParentVessel.GUID;
      persistenceObjectDataRoom.GUID = guid;
      int inSceneId = this.ID.InSceneID;
      persistenceObjectDataRoom.InSceneID = inSceneId;
      double airPressure = (double) this.AirPressure;
      persistenceObjectDataRoom.AirPressure = (float) airPressure;
      double airQuality1 = (double) this.AirQuality;
      persistenceObjectDataRoom.AirQuality = (float) airQuality1;
      double temperature = (double) this.Temperature;
      persistenceObjectDataRoom.Temperature = (float) temperature;
      int num1 = this.UseGravity ? 1 : 0;
      persistenceObjectDataRoom.UseGravity = num1 != 0;
      double quantity = (double) this.AirTank.Quantity;
      persistenceObjectDataRoom.AirTankQuantity = (float) quantity;
      double airQuality2 = (double) this.AirTank.AirQuality;
      persistenceObjectDataRoom.AirTankQuality = (float) airQuality2;
      int num2 = this.GravityMalfunction ? 1 : 0;
      persistenceObjectDataRoom.GravityMalfunction = num2 != 0;
      return (PersistenceObjectData) persistenceObjectDataRoom;
    }

    public void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        PersistenceObjectDataRoom persistenceObjectDataRoom = persistenceData as PersistenceObjectDataRoom;
        if (persistenceObjectDataRoom == null)
        {
          Dbg.Warning("PersistenceObjectDataRoom data is null");
        }
        else
        {
          this._AirPressure = persistenceObjectDataRoom.AirPressure;
          this._AirQuality = persistenceObjectDataRoom.AirQuality;
          this._Temperature = persistenceObjectDataRoom.Temperature;
          this._UseGravity = persistenceObjectDataRoom.UseGravity;
          this.AirTank.Quantity = persistenceObjectDataRoom.AirTankQuantity;
          this.AirTank.AirQuality = persistenceObjectDataRoom.AirTankQuality;
          this.GravityMalfunction = persistenceObjectDataRoom.GravityMalfunction;
        }
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }

    public RoomDetails GetDetails()
    {
      RoomDetails roomDetails = new RoomDetails();
      roomDetails.InSceneID = this.ID.InSceneID;
      roomDetails.AirPressure = this.AirPressure;
      roomDetails.AirQuality = this.AirQuality;
      roomDetails.CompoundRoomID = this.CompoundRoom.ID;
      int num1 = this.UseGravity ? 1 : 0;
      roomDetails.UseGravity = num1 != 0;
      double temperature = (double) this.Temperature;
      roomDetails.Temperature = (float) temperature;
      double quantity = (double) this.AirTank.Quantity;
      roomDetails.AirInTank = (float) quantity;
      int num2 = this.AirConsumers.Count<IAirConsumer>((Func<IAirConsumer, bool>) (m => m is AirConsumerFire)) > 0 ? 1 : 0;
      roomDetails.Fire = num2 != 0;
      int num3 = this.AirConsumers.Count<IAirConsumer>((Func<IAirConsumer, bool>) (m => m is AirConsumerBreach)) > 0 ? 1 : 0;
      roomDetails.Breach = num3 != 0;
      int num4 = this.GravityMalfunction ? 1 : 0;
      roomDetails.GravityMalfunction = num4 != 0;
      return roomDetails;
    }
  }
}
