// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.SubSystemEngine
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class SubSystemEngine : SubSystem
  {
    private float accelerationFactor = 1f;
    private float accelerationBuildupFactor = 1f;
    private float _Acceleration;
    private float _AccelerationBuildup;
    public float ReverseAcceleration;
    public bool ThrustActive;
    public bool ReverseThrust;
    public float RequiredThrust;

    public float Acceleration
    {
      get
      {
        return this._Acceleration * this.accelerationFactor;
      }
    }

    public float AccelerationBuildup
    {
      get
      {
        return this._AccelerationBuildup * this.accelerationBuildupFactor;
      }
    }

    public override bool AutoReactivate
    {
      get
      {
        return false;
      }
    }

    public override SubSystemType Type
    {
      get
      {
        return SubSystemType.Engine;
      }
    }

    public SubSystemEngine(SpaceObjectVessel vessel, VesselObjectID id, SubSystemData ssData)
      : base(vessel, id, ssData)
    {
    }

    public override void Update(double duration)
    {
      base.Update(duration);
      if (this.ThrustActive && (double) this.RequiredThrust != 0.0)
      {
        if ((double) this.OperationRate != (double) this.RequiredThrust)
          this.OperationRate = this.RequiredThrust;
      }
      else
        this.OperationRate = 0.0f;
      float num1 = 1f;
      float num2 = 1f;
      float num3 = 1f;
      foreach (KeyValuePair<VesselObjectID, MachineryPartSlotData> machineryPartSlot in this.machineryPartSlots)
      {
        MachineryPartType slotType = machineryPartSlot.Value.SlotType;
        float basicModifier = machineryPartSlot.Value.BasicModifier;
        bool isActive = machineryPartSlot.Value.IsActive;
        MachineryPart machineryPart = (MachineryPart) null;
        this.machineryParts.TryGetValue(machineryPartSlot.Key, out machineryPart);
        if (slotType == MachineryPartType.HighEnergyLaser)
        {
          this.accelerationBuildupFactor = 1f + basicModifier;
          if (machineryPart != null & isActive)
            this.accelerationBuildupFactor = this.accelerationBuildupFactor - basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
        }
        else if (slotType == MachineryPartType.ResourceInjector)
        {
          num1 = 1f + basicModifier;
          if (machineryPart != null & isActive)
            num1 -= basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
        }
        else if (slotType == MachineryPartType.PlasmaAccelerator)
        {
          this.accelerationFactor = 1f - basicModifier;
          if (machineryPart != null & isActive)
            this.accelerationFactor = this.accelerationFactor + basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
        }
        else if (slotType == MachineryPartType.EmShieldGenerator)
        {
          num2 = 1f + basicModifier;
          num3 = 1f + basicModifier;
          if (machineryPart != null & isActive)
          {
            num2 -= basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
            num3 -= basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
          }
        }
      }
      this.InputFactor = num1;
      this.InputFactorStandby = num2;
      this.powerUpFactor = num3;
      this.cooldownFactor = num3;
    }

    public override void SetAuxData(SubSystemAuxData auxData)
    {
      SubSystemEngineAuxData systemEngineAuxData = auxData as SubSystemEngineAuxData;
      this._Acceleration = systemEngineAuxData.Acceleration;
      this.ReverseAcceleration = systemEngineAuxData.ReverseAcceleration;
      this._AccelerationBuildup = systemEngineAuxData.AccelerationBuildup;
    }
  }
}
