// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.SubSystemAirDevice
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class SubSystemAirDevice : SubSystem, ILifeSupportDevice
  {
    public float MaxOutput { get; private set; }

    public float NominalOutput { get; private set; }

    public override SubSystemType Type
    {
      get
      {
        return SubSystemType.AirDevice;
      }
    }

    public SubSystemAirDevice(SpaceObjectVessel vessel, VesselObjectID id, SubSystemData ssData)
      : base(vessel, id, ssData)
    {
      this.autoReactivate = this.AutoReactivate && this.Status == SystemStatus.OnLine;
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
        if (slotType == MachineryPartType.PressureRegulator)
        {
          num = 1f - basicModifier;
          if (machineryPart != null & isActive && (double) machineryPart.MaxHealth > 0.0)
            num += basicModifier * (machineryPart.Health / machineryPart.MaxHealth) * machineryPart.BoostFactor;
        }
      }
      this.MaxOutput = this.NominalOutput * num;
      this._InputFactor = num;
    }

    public override void SetAuxData(SubSystemAuxData auxData)
    {
      this.NominalOutput = (auxData as SubSystemAirDevicesAuxData).Output;
      this.MaxOutput = this.NominalOutput;
    }
  }
}
