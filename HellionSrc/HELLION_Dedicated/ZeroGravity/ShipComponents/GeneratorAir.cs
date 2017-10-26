// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.GeneratorAir
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class GeneratorAir : Generator
  {
    public override GeneratorType Type
    {
      get
      {
        return GeneratorType.Air;
      }
    }

    public override DistributionSystemType OutputType
    {
      get
      {
        return DistributionSystemType.Air;
      }
    }

    public GeneratorAir(SpaceObjectVessel vessel, VesselObjectID id, GeneratorData genData)
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
        MachineryPart machineryPart = (MachineryPart) null;
        this.machineryParts.TryGetValue(machineryPartSlot.Key, out machineryPart);
        if (slotType == MachineryPartType.ServoMotor)
        {
          num1 = 1f - basicModifier;
          if (machineryPart != null & isActive && (double) machineryPart.MaxHealth > 0.0)
            num1 += basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
        }
        else if (slotType == MachineryPartType.ResourceInjector)
        {
          num2 = 1f + basicModifier;
          if (machineryPart != null & isActive && (double) machineryPart.MaxHealth > 0.0)
            num2 -= basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
        }
        else if (slotType != MachineryPartType.ExternalAirVent)
          ;
      }
      this._MaxOutput = this.NominalOutput * num1;
      this._InputFactor = num2;
    }

    public override void SetAuxData(SubSystemAuxData auxData)
    {
    }
  }
}
