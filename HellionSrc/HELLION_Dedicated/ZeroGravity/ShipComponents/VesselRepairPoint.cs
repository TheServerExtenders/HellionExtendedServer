// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.VesselRepairPoint
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class VesselRepairPoint : IPersistantObject
  {
    private float _MaxHealth = 0.0f;
    private float _Health = 0.0f;
    public bool StatusChanged = true;
    public IAirConsumer AirCousumer = (IAirConsumer) null;
    public VesselObjectID ID;
    public SpaceObjectVessel ParentVessel;
    public RepairPointDamageType DamageType;
    public VesselComponent AffectedSystem;
    public float MalfunctionThreshold;
    public float RepairThreshold;

    public Room Room { get; set; }

    public float MaxHealth
    {
      get
      {
        return this._MaxHealth;
      }
      set
      {
        float num = value;
        if ((double) this._MaxHealth != (double) num)
          this.StatusChanged = true;
        this._MaxHealth = num;
        this.Update();
      }
    }

    public float Health
    {
      get
      {
        return this._Health;
      }
      set
      {
        float num = MathHelper.Clamp((double) this.MaxHealth - (double) value < 1.0 ? this.MaxHealth : value, 0.0f, this.MaxHealth);
        if ((double) this._Health != (double) num)
          this.StatusChanged = true;
        this._Health = num;
        this.Update();
      }
    }

    public void Update()
    {
      if (this.DamageType == RepairPointDamageType.None)
        return;
      float num = (double) this._MaxHealth > 0.0 ? this._Health / this._MaxHealth : 0.0f;
      if (this.DamageType == RepairPointDamageType.System && this.AffectedSystem != null)
      {
        if ((double) num <= (double) this.MalfunctionThreshold && !this.AffectedSystem.Defective)
        {
          this.AffectedSystem.Defective = true;
        }
        else
        {
          if ((double) num < (double) this.RepairThreshold || !this.AffectedSystem.Defective)
            return;
          this.AffectedSystem.Defective = false;
        }
      }
      else if (this.DamageType == RepairPointDamageType.Gravity && this.Room != null)
      {
        if ((double) num <= (double) this.MalfunctionThreshold && !this.Room.GravityMalfunction)
        {
          this.Room.GravityMalfunction = true;
        }
        else
        {
          if ((double) num < (double) this.RepairThreshold || !this.Room.GravityMalfunction)
            return;
          this.Room.GravityMalfunction = false;
        }
      }
      else
      {
        if (this.DamageType != RepairPointDamageType.Breach && this.DamageType != RepairPointDamageType.Fire || this.Room == null)
          return;
        if ((double) num <= (double) this.MalfunctionThreshold && this.AirCousumer == null)
        {
          if (this.DamageType == RepairPointDamageType.Breach)
            this.AirCousumer = (IAirConsumer) new AirConsumerBreach(BreachType.Small);
          else
            this.AirCousumer = (IAirConsumer) new AirConsumerFire(FireType.Small)
            {
              Persistent = true
            };
          if (this.AirCousumer != null)
          {
            this.Room.AirConsumers.Add(this.AirCousumer);
            if (this.Room.CompoundRoom != null)
              this.Room.CompoundRoom.AirConsumers.Add(this.AirCousumer);
            this.Room.StatusChanged = true;
          }
        }
        else if ((double) num >= (double) this.RepairThreshold && this.AirCousumer != null)
        {
          this.Room.AirConsumers.Remove(this.AirCousumer);
          if (this.Room.CompoundRoom != null)
            this.Room.CompoundRoom.AirConsumers.Remove(this.AirCousumer);
          this.AirCousumer = (IAirConsumer) null;
          this.Room.StatusChanged = true;
        }
      }
    }

    public VesselRepairPoint(SpaceObjectVessel vessel, VesselRepairPointData data, float maxHealth)
    {
      this.ID = new VesselObjectID(vessel.GUID, data.InSceneID);
      this.ParentVessel = vessel;
      this.Room = vessel.Rooms.Find((Predicate<Room>) (m => m.ID.InSceneID == data.RoomID));
      this._MaxHealth = maxHealth;
      this._Health = maxHealth;
      this.DamageType = data.DamageType;
      this.AffectedSystem = vessel.Systems.Find((Predicate<VesselComponent>) (m => m.ID.InSceneID == data.AffectedSystemID));
      this.MalfunctionThreshold = data.MalfunctionThreshold;
      this.RepairThreshold = data.RepairThreshold;
      this.Update();
    }

    public PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataRepairPoint objectDataRepairPoint = new PersistenceObjectDataRepairPoint();
      long guid = this.ParentVessel.GUID;
      objectDataRepairPoint.GUID = guid;
      int inSceneId = this.ID.InSceneID;
      objectDataRepairPoint.InSceneID = inSceneId;
      double maxHealth = (double) this.MaxHealth;
      objectDataRepairPoint.MaxHealth = (float) maxHealth;
      double health = (double) this.Health;
      objectDataRepairPoint.Health = (float) health;
      return (PersistenceObjectData) objectDataRepairPoint;
    }

    public void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        PersistenceObjectDataRepairPoint objectDataRepairPoint = persistenceData as PersistenceObjectDataRepairPoint;
        if (objectDataRepairPoint == null)
        {
          Dbg.Warning("PersistenceObjectDataRoom data is null");
        }
        else
        {
          this._MaxHealth = objectDataRepairPoint.MaxHealth;
          this._Health = objectDataRepairPoint.Health;
          this.Update();
        }
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }

    public VesselRepairPointDetails GetDetails()
    {
      bool flag = this.DamageType == RepairPointDamageType.System && this.AffectedSystem != null && this.AffectedSystem.Defective || (this.DamageType == RepairPointDamageType.Fire || this.DamageType == RepairPointDamageType.Breach) && this.AirCousumer != null;
      VesselRepairPointDetails repairPointDetails = new VesselRepairPointDetails();
      repairPointDetails.InSceneID = this.ID.InSceneID;
      repairPointDetails.MaxHealth = this.MaxHealth;
      repairPointDetails.Health = this.Health;
      int num = flag ? 1 : 0;
      repairPointDetails.SecondaryDamageActive = num != 0;
      return repairPointDetails;
    }
  }
}
