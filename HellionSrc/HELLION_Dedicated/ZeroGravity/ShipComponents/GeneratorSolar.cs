// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.GeneratorSolar
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class GeneratorSolar : Generator
  {
    private float outputFactor = 1f;
    private float deployFactor = 1f;
    private double sqMin = 1E+22;
    private double sqMax = 9E+22;
    public double ThresholdDistanceMin;
    public double ThresholdDistanceMax;

    public override GeneratorType Type
    {
      get
      {
        return GeneratorType.Solar;
      }
    }

    public override DistributionSystemType OutputType
    {
      get
      {
        return DistributionSystemType.Power;
      }
    }

    public override float MaxOutput
    {
      get
      {
        return this.NominalOutput * this.outputFactor;
      }
    }

    public override float PowerUpTime
    {
      get
      {
        return this._PowerUpTime * this.deployFactor;
      }
    }

    public override float CoolDownTime
    {
      get
      {
        return this._CoolDownTime;
      }
    }

    public GeneratorSolar(SpaceObjectVessel vessel, VesselObjectID id, GeneratorData genData)
      : base(vessel, id, genData)
    {
    }

    public override void Update(double duration)
    {
      base.Update(duration);
      float num1 = 1f;
      float num2 = 1f;
      foreach (KeyValuePair<VesselObjectID, MachineryPartSlotData> machineryPartSlot in this.machineryPartSlots)
      {
        MachineryPartType slotType = machineryPartSlot.Value.SlotType;
        float basicModifier = machineryPartSlot.Value.BasicModifier;
        bool isActive = machineryPartSlot.Value.IsActive;
        MachineryPart machineryPart = this.machineryParts[machineryPartSlot.Key];
        if (slotType == MachineryPartType.SolarPanel)
        {
          num1 = 1f - basicModifier;
          if (machineryPart != null & isActive && (double) machineryPart.MaxHealth > 0.0)
            num1 += basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
        }
        else if (slotType == MachineryPartType.ServoMotor)
        {
          num2 = float.MaxValue;
          if (machineryPart != null & isActive && (double) machineryPart.MaxHealth > 0.0)
            num2 = (float) (1.0 + (double) basicModifier - (double) basicModifier * ((double) machineryPart.Health / (double) machineryPart.MaxHealth) * (double) machineryPart.BoostFactor);
        }
      }
      if (this.ParentVessel.IsExposedToSunlight)
      {
        double num3 = this.sqMax - this.sqMin;
        this.outputFactor = num1 * (float) MathHelper.Clamp((num3 - (this.ParentVessel.SqrDistanceFromSun - this.sqMin)) / num3, 0.0, 1.0);
      }
      else
        this.outputFactor = 0.0f;
      if ((double) num2 == 3.40282346638529E+38 && (this.Status == SystemStatus.OnLine || this.Status == SystemStatus.PowerUp))
        this.GoOffLine(true, true);
      this.deployFactor = num2;
    }

    public override void SetAuxData(SubSystemAuxData auxData)
    {
      GeneratorSolarAuxData generatorSolarAuxData = auxData as GeneratorSolarAuxData;
      this.ThresholdDistanceMax = generatorSolarAuxData.ThresholdDistanceMax;
      this.ThresholdDistanceMin = generatorSolarAuxData.ThresholdDistanceMin;
      this.sqMax = this.ThresholdDistanceMax * this.ThresholdDistanceMax;
      this.sqMin = this.ThresholdDistanceMin * this.ThresholdDistanceMin;
    }

    public override IAuxDetails GetAuxDetails()
    {
      return (IAuxDetails) new GeneratorSolarAuxDetails()
      {
        ExposureToSunlight = (float) ((double) this.MaxOutput / (double) this.NominalOutput)
      };
    }
  }
}
