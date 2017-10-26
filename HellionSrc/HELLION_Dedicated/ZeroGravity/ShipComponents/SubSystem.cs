// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.SubSystem
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
  public abstract class SubSystem : VesselComponent
  {
    protected bool _IsSwitchedOn = true;
    private bool _IsWorking;
    public bool HasEnoughCPU;
    private float _Temperature;
    public bool AutoTuneOperationRate;

    public abstract SubSystemType Type { get; }

    public abstract void SetAuxData(SubSystemAuxData auxData);

    public float Temperature
    {
      get
      {
        return this._Temperature;
      }
      set
      {
        if ((double) this._Temperature != (double) value)
          this.StatusChanged = true;
        this._Temperature = value;
      }
    }

    public SubSystem(SpaceObjectVessel vessel, VesselObjectID id, SubSystemData ssData)
      : base(vessel, id)
    {
      SubSystemData subSystemData = ObjectCopier.DeepCopy<SubSystemData>(ssData, 10);
      this.ResourceRequirements = DistributionManager.ResourceRequirementsToDictionary(subSystemData.ResourceRequirements);
      if (subSystemData.SpawnSettings != null)
      {
        foreach (SystemSpawnSettings spawnSetting in subSystemData.SpawnSettings)
        {
          if (vessel.CheckTag(spawnSetting.Tag, spawnSetting.Case))
          {
            float num = MathHelper.Clamp(spawnSetting.ResourceRequirementMultiplier, 0.0f, float.MaxValue);
            using (Dictionary<DistributionSystemType, ResourceRequirement>.Enumerator enumerator = this.ResourceRequirements.GetEnumerator())
            {
              while (enumerator.MoveNext())
              {
                KeyValuePair<DistributionSystemType, ResourceRequirement> current = enumerator.Current;
                this.ResourceRequirements[current.Key].Nominal *= num;
                this.ResourceRequirements[current.Key].Standby *= num;
              }
              break;
            }
          }
        }
      }
      this._PowerUpTime = subSystemData.PowerUpTime;
      this._CoolDownTime = subSystemData.CoolDownTime;
      this.Status = subSystemData.Status;
      this._AutoReactivate = subSystemData.AutoReactivate;
      this._OperationRate = subSystemData.OperationRate;
      this.AutoTuneOperationRate = subSystemData.AutoTuneOperationRate;
      this.SetAuxData(subSystemData.AuxData);
    }

    public virtual IAuxDetails GetAuxDetails()
    {
      return (IAuxDetails) null;
    }

    public virtual void SetDetails(SubSystemDetails details)
    {
      if (details.Status == SystemStatus.OnLine)
        this.GoOnLine();
      else if (details.Status == SystemStatus.OffLine)
        this.GoOffLine(false, false);
      this.SetAuxDetails(details.AuxDetails);
    }

    public virtual void SetAuxDetails(IAuxDetails auxDetails)
    {
    }

    public override bool CheckAvailableResources(float consumptionFactor, float duration, bool standby, ref Dictionary<IResourceProvider, float> reservedCapacities, ref Dictionary<ResourceContainer, float> reservedQuantities, ref string debugText)
    {
      if (standby || !this.AutoTuneOperationRate && (double) this.OperationRate > 1.40129846432482E-45)
        return base.CheckAvailableResources(consumptionFactor, duration, standby, ref reservedCapacities, ref reservedQuantities, ref debugText);
      if ((double) this.OperationRate <= 1.40129846432482E-45)
        return true;
      float num1 = 1f;
      float consumptionFactor1 = 1f;
      float num2 = 0.0f;
      Dictionary<IResourceProvider, float> dictionary1 = new Dictionary<IResourceProvider, float>((IDictionary<IResourceProvider, float>) reservedCapacities);
      Dictionary<ResourceContainer, float> dictionary2 = new Dictionary<ResourceContainer, float>((IDictionary<ResourceContainer, float>) reservedQuantities);
      string str = debugText;
      string debugText1 = (string) null;
      for (int index = 0; index < 5; ++index)
      {
        Dictionary<IResourceProvider, float> reservedCapacities1 = new Dictionary<IResourceProvider, float>((IDictionary<IResourceProvider, float>) reservedCapacities);
        Dictionary<ResourceContainer, float> reservedQuantities1 = new Dictionary<ResourceContainer, float>((IDictionary<ResourceContainer, float>) reservedQuantities);
        debugText1 = debugText;
        num1 /= 2f;
        if (base.CheckAvailableResources(consumptionFactor1, duration, false, ref reservedCapacities1, ref reservedQuantities1, ref debugText1))
        {
          if ((double) consumptionFactor1 > (double) num2)
          {
            num2 = consumptionFactor1;
            dictionary1 = reservedCapacities1;
            dictionary2 = reservedQuantities1;
            str = debugText1;
          }
          if ((double) num2 < 1.0)
            consumptionFactor1 += num1;
          else
            break;
        }
        else
          consumptionFactor1 -= num1;
      }
      if ((double) num2 > 1.40129846432482E-45)
      {
        this.OperationRate = num2;
        reservedCapacities = dictionary1;
        reservedQuantities = dictionary2;
        return true;
      }
      debugText = debugText1;
      return false;
    }
  }
}
