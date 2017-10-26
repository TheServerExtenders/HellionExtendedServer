// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.SubSystemPDU
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class SubSystemPDU : SubSystem
  {
    private int _LimiterIndex;
    private List<VesselObjectID> _PDUGroups;

    public override SubSystemType Type
    {
      get
      {
        return SubSystemType.PDU;
      }
    }

    public int LimiterIndex
    {
      get
      {
        return this._LimiterIndex;
      }
      set
      {
        if (this._LimiterIndex == value)
          return;
        this.StatusChanged = true;
        this._LimiterIndex = value;
      }
    }

    public List<VesselObjectID> PDUGroups
    {
      get
      {
        return this._PDUGroups;
      }
      set
      {
        this._PDUGroups = value;
        this.StatusChanged = true;
      }
    }

    public SubSystemPDU(SpaceObjectVessel vessel, VesselObjectID id, SubSystemData ssData)
      : base(vessel, id, ssData)
    {
    }

    public override IAuxDetails GetAuxDetails()
    {
      return (IAuxDetails) new PDUAuxDetails()
      {
        LimiterIndex = this.LimiterIndex,
        PDUGroups = this.PDUGroups
      };
    }

    public override void SetAuxData(SubSystemAuxData auxData)
    {
    }

    public override void SetAuxDetails(IAuxDetails auxDetails)
    {
      this.PDUGroups = (auxDetails as PDUAuxDetails).PDUGroups;
    }
  }
}
