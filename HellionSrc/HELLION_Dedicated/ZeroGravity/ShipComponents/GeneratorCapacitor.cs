// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.GeneratorCapacitor
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ZeroGravity.Data;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class GeneratorCapacitor : Generator
  {
    public float MaxCapacity;
    private float _Capacity;

    public override GeneratorType Type
    {
      get
      {
        return GeneratorType.Capacitor;
      }
    }

    public override DistributionSystemType OutputType
    {
      get
      {
        return DistributionSystemType.Power;
      }
    }

    public float Capacity
    {
      get
      {
        return this._Capacity;
      }
      set
      {
        if ((double) this._Capacity != (double) value)
          this.StatusChanged = true;
        this._Capacity = value;
      }
    }

    public GeneratorCapacitor(SpaceObjectVessel vessel, VesselObjectID id, GeneratorData genData)
      : base(vessel, id, genData)
    {
    }

    public override IAuxDetails GetAuxDetails()
    {
      return (IAuxDetails) new GeneratorCapacitorAuxDetails()
      {
        Capacity = this.Capacity
      };
    }

    public override void SetAuxData(SubSystemAuxData auxData)
    {
      GeneratorCapacitorAuxData capacitorAuxData = auxData as GeneratorCapacitorAuxData;
      this.MaxCapacity = capacitorAuxData.MaxCapacity;
      this.Capacity = capacitorAuxData.Capacity;
    }
  }
}
