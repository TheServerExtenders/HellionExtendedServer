// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.SubSystemScrubberDevice
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class SubSystemScrubberDevice : SubSystem, ILifeSupportDevice
  {
    public float NominalScrubbingCapacity { get; private set; }

    public float MaxScrubbingCapacity { get; private set; }

    public override SubSystemType Type
    {
      get
      {
        return SubSystemType.ScrubberDevice;
      }
    }

    public SubSystemScrubberDevice(SpaceObjectVessel vessel, VesselObjectID id, SubSystemData ssData)
      : base(vessel, id, ssData)
    {
      this.autoReactivate = this.AutoReactivate;
    }

    public override void SetAuxData(SubSystemAuxData auxData)
    {
      this.NominalScrubbingCapacity = (auxData as SubSystemScrubberDeviceAuxData).NominalScrubbingCapacity;
      this.MaxScrubbingCapacity = this.NominalScrubbingCapacity;
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
        if (slotType == MachineryPartType.AirFilterUnit)
        {
          num = 1f - basicModifier;
          if (machineryPart != null & isActive && (double) machineryPart.MaxHealth > 0.0)
            num += basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
        }
      }
      this.MaxScrubbingCapacity = this.NominalScrubbingCapacity * num;
    }
  }
}
