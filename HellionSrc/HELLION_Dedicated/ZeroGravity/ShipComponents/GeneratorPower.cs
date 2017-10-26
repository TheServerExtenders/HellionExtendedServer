// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.GeneratorPower
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class GeneratorPower : Generator
  {
    public override GeneratorType Type
    {
      get
      {
        return GeneratorType.Power;
      }
    }

    public override DistributionSystemType OutputType
    {
      get
      {
        return DistributionSystemType.Power;
      }
    }

    public GeneratorPower(SpaceObjectVessel vessel, VesselObjectID id, GeneratorData genData)
      : base(vessel, id, genData)
    {
    }

    public override void Update(double duration)
    {
      base.Update(duration);
      float num1 = 1f;
      float num2 = 1f;
      float num3 = 1f;
      float num4 = 1f;
      Dictionary<MachineryPartSlotData, MachineryPart> dictionary = new Dictionary<MachineryPartSlotData, MachineryPart>();
      foreach (KeyValuePair<VesselObjectID, MachineryPartSlotData> machineryPartSlot in this.machineryPartSlots)
      {
        MachineryPartType slotType = machineryPartSlot.Value.SlotType;
        float basicModifier = machineryPartSlot.Value.BasicModifier;
        bool isActive = machineryPartSlot.Value.IsActive;
        MachineryPart machineryPart = (MachineryPart) null;
        this.machineryParts.TryGetValue(machineryPartSlot.Key, out machineryPart);
        if (slotType == MachineryPartType.CoreContainmentFieldGenerator)
        {
          num1 = 1f - basicModifier;
          if (machineryPart != null & isActive && (double) machineryPart.MaxHealth > 0.0)
            num1 += basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
        }
        else if (slotType == MachineryPartType.ThermonuclearCatalyst)
        {
          dictionary[machineryPartSlot.Value] = machineryPart;
          num2 = 0.0f;
        }
        else if (slotType == MachineryPartType.ResourceInjector)
        {
          num4 = 1f + basicModifier;
          if (machineryPart != null & isActive && (double) machineryPart.MaxHealth > 0.0)
            num4 -= basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
        }
        else if (slotType == MachineryPartType.EMFieldController)
        {
          this.powerUpFactor = 1f + basicModifier;
          if (machineryPart != null & isActive && (double) machineryPart.MaxHealth > 0.0)
            this.powerUpFactor = this.powerUpFactor - basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
        }
        else if (slotType == MachineryPartType.ExternalDeuteriumExhaust && (machineryPart == null || !isActive || (double) machineryPart.Health == 0.0))
          num3 = 1f + basicModifier;
      }
      foreach (KeyValuePair<MachineryPartSlotData, MachineryPart> keyValuePair in dictionary)
      {
        if (keyValuePair.Key.IsActive && keyValuePair.Value != null && (double) keyValuePair.Value.Health > 0.0)
          num2 += 1f / (float) dictionary.Count * keyValuePair.Value.BoostFactor;
      }
      this._MaxOutput = this.NominalOutput * num1 * num2;
      this._InputFactor = num4 * num3;
      this.cooldownFactor = this.powerUpFactor;
    }

    public override void SetAuxData(SubSystemAuxData auxData)
    {
    }
  }
}
