// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.GeneratorScrubber
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
  public class GeneratorScrubber : Generator
  {
    public float ScrubberCartridgeConsumption;

    public override GeneratorType Type
    {
      get
      {
        return GeneratorType.AirScrubber;
      }
    }

    public override DistributionSystemType OutputType
    {
      get
      {
        return DistributionSystemType.ScrubbedAir;
      }
    }

    public GeneratorScrubber(SpaceObjectVessel vessel, VesselObjectID id, GeneratorData genData)
      : base(vessel, id, genData)
    {
      this.ScrubberCartridgeConsumption = (genData.AuxData as GeneratorScrubbedAirAuxData).ScrubberCartridgeConsumption;
    }

    public override void Update(double duration)
    {
      base.Update(duration);
      float num1 = 1f;
      float num2 = 1f;
      float num3 = 1f;
      List<MachineryPart> machineryPartList = new List<MachineryPart>();
      int num4 = 0;
      int num5 = 0;
      foreach (KeyValuePair<VesselObjectID, MachineryPartSlotData> machineryPartSlot in this.machineryPartSlots)
      {
        MachineryPartType slotType = machineryPartSlot.Value.SlotType;
        float basicModifier = machineryPartSlot.Value.BasicModifier;
        bool isActive = machineryPartSlot.Value.IsActive;
        MachineryPart machineryPart = (MachineryPart) null;
        this.machineryParts.TryGetValue(machineryPartSlot.Key, out machineryPart);
        if (slotType == MachineryPartType.ServoMotor)
        {
          num1 = 1f - basicModifier;
          if (machineryPart != null & isActive && (double) machineryPart.MaxHealth > 0.0)
            num1 += basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
        }
        else if (slotType == MachineryPartType.CarbonFilters)
        {
          ++num4;
          if (isActive && machineryPart != null && (double) machineryPart.Health > 0.0)
          {
            machineryPartList.Add(machineryPart);
            ++num5;
            num2 = 0.0f;
          }
        }
        else if (slotType == MachineryPartType.AirProcessingController)
        {
          num3 = 1f + basicModifier;
          this.powerUpFactor = 1f + basicModifier;
          if (machineryPart != null & isActive && (double) machineryPart.MaxHealth > 0.0)
          {
            num3 -= basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
            this.powerUpFactor = this.powerUpFactor - basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
          }
        }
      }
      foreach (MachineryPart machineryPart in machineryPartList)
      {
        num2 += 1f / (float) num4 * machineryPart.BoostFactor;
        machineryPart.Health = (float) (int) MathHelper.Clamp((double) machineryPart.Health - (double) machineryPart.WearMultiplier * (double) this.ScrubberCartridgeConsumption / (double) num5 * duration, 0.0, (double) machineryPart.MaxHealth);
        machineryPart.DynamicObj.SendStatsToClient();
      }
      this._MaxOutput = this.NominalOutput * num1 * num2;
      this._InputFactor = num3;
      this.cooldownFactor = this.powerUpFactor;
    }

    public override void SetAuxData(SubSystemAuxData auxData)
    {
    }
  }
}
