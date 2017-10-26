// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.SubSystemAirVent
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ZeroGravity.Data;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class SubSystemAirVent : SubSystem, IAirConsumer
  {
    private Room _Room;

    public Room room
    {
      get
      {
        return this._Room;
      }
      set
      {
        this._Room = value;
      }
    }

    public float Output { get; private set; }

    public override SubSystemType Type
    {
      get
      {
        return SubSystemType.AirVent;
      }
    }

    public float AirQualityDegradationRate
    {
      get
      {
        return 0.0f;
      }
    }

    public float AirQuantityDecreaseRate
    {
      get
      {
        if (this.Status == SystemStatus.OnLine)
          return this.Output;
        return 0.0f;
      }
    }

    public bool AffectsQuality
    {
      get
      {
        return false;
      }
    }

    public bool AffectsQuantity
    {
      get
      {
        return (double) this.AirQuantityDecreaseRate > 0.0;
      }
    }

    public SubSystemAirVent(SpaceObjectVessel vessel, VesselObjectID id, SubSystemData ssData)
      : base(vessel, id, ssData)
    {
      this._IsSwitchedOn = false;
    }

    public override void SetAuxData(SubSystemAuxData auxData)
    {
      this.Output = (auxData as SubSystemAirDevicesAuxData).Output;
    }
  }
}
