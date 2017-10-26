// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.SubSystemRCS
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class SubSystemRCS : SubSystem
  {
    private float _MaxOperationRate = 1f;
    private double lastSwitchOnTime;
    public float Acceleration;
    public float RotationAcceleration;
    public float RotationStabilization;

    public override SubSystemType Type
    {
      get
      {
        return SubSystemType.RCS;
      }
    }

    public override float PowerUpTime
    {
      get
      {
        return 0.0f;
      }
    }

    public override float CoolDownTime
    {
      get
      {
        return 0.0f;
      }
    }

    public override bool AutoReactivate
    {
      get
      {
        return false;
      }
    }

    public float MaxOperationRate
    {
      get
      {
        return this._MaxOperationRate;
      }
      set
      {
        if ((double) this._MaxOperationRate != (double) value)
          this.StatusChanged = true;
        this._MaxOperationRate = value;
      }
    }

    public override SystemStatus Status
    {
      get
      {
        if (Server.Instance.RunTime.TotalSeconds - this.lastSwitchOnTime > 1.0 && this._Status != SystemStatus.OffLine)
          this.Status = SystemStatus.OffLine;
        return base.Status;
      }
      protected set
      {
        if (value == SystemStatus.OnLine)
          this.lastSwitchOnTime = Server.Instance.RunTime.TotalSeconds;
        base.Status = value;
      }
    }

    public override void Update(double duration)
    {
      base.Update(duration);
      float num = 1f;
      foreach (KeyValuePair<VesselObjectID, MachineryPartSlotData> machineryPartSlot in this.machineryPartSlots)
      {
        MachineryPartType slotType = machineryPartSlot.Value.SlotType;
        float basicModifier = machineryPartSlot.Value.BasicModifier;
        bool isActive = machineryPartSlot.Value.IsActive;
        MachineryPart machineryPart = (MachineryPart) null;
        this.machineryParts.TryGetValue(machineryPartSlot.Key, out machineryPart);
        if (slotType == MachineryPartType.ResourceInjector)
        {
          num = 1f - basicModifier;
          if (machineryPart != null & isActive && (double) machineryPart.MaxHealth > 0.0)
            num += basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
        }
      }
      this.MaxOperationRate = num;
    }

    public SubSystemRCS(SpaceObjectVessel vessel, VesselObjectID id, SubSystemData ssData)
      : base(vessel, id, ssData)
    {
    }

    public override void SetAuxData(SubSystemAuxData auxData)
    {
      SubSystemRCSAuxData systemRcsAuxData = auxData as SubSystemRCSAuxData;
      this.Acceleration = systemRcsAuxData.Acceleration;
      this.RotationAcceleration = systemRcsAuxData.RotationAcceleration;
      this.RotationStabilization = systemRcsAuxData.RotationStabilization;
    }

    public override IAuxDetails GetAuxDetails()
    {
      return (IAuxDetails) new RCSAuxDetails()
      {
        MaxOperationRate = this.MaxOperationRate
      };
    }
  }
}
