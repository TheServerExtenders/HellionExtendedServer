// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.SubSystemEmergencyLights
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ZeroGravity.Data;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class SubSystemEmergencyLights : SubSystem
  {
    public override SubSystemType Type
    {
      get
      {
        return SubSystemType.EmergencyLight;
      }
    }

    public SubSystemEmergencyLights(SpaceObjectVessel vessel, VesselObjectID id, SubSystemData ssData)
      : base(vessel, id, ssData)
    {
      this.autoReactivate = this.AutoReactivate;
    }

    public override void SetAuxData(SubSystemAuxData auxData)
    {
    }
  }
}
