// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.Generator
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using Newtonsoft.Json;
using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public abstract class Generator : VesselComponent, IResourceProvider
  {
    private bool _IsSwitchedOn = true;
    private float statusChangeCountdown = 0.0f;
    private HashSet<IResourceConsumer> _ConnectedConsumers = new HashSet<IResourceConsumer>();
    private float _NominalOutput;
    private float _Output;
    private bool _IsWorking;
    private float _Temperature;
    protected float _MaxOutput;
    public bool HasEnoughCPU;

    public abstract GeneratorType Type { get; }

    public abstract DistributionSystemType OutputType { get; }

    public abstract void SetAuxData(SubSystemAuxData auxData);

    public virtual float MaxOutput
    {
      get
      {
        return this._MaxOutput;
      }
    }

    public override SystemStatus Status
    {
      get
      {
        return base.Status;
      }
      protected set
      {
        base.Status = value;
        if (this.Status == SystemStatus.OnLine)
          return;
        this._Output = -1f;
        this.Output = 0.0f;
      }
    }

    public float NominalOutput
    {
      get
      {
        return this._NominalOutput;
      }
      set
      {
        if ((double) this._NominalOutput != (double) value)
          this.StatusChanged = true;
        this._NominalOutput = value;
      }
    }

    public float Output
    {
      get
      {
        return this._Output;
      }
      set
      {
        if ((double) this._Output == (double) value)
          return;
        this._Output = value;
        this.OperationRate = (double) this.NominalOutput > 0.0 ? this.Output / this.NominalOutput : 0.0f;
        this.StatusChanged = true;
      }
    }

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

    [JsonIgnore]
    public HashSet<IResourceConsumer> ConnectedConsumers
    {
      get
      {
        return this._ConnectedConsumers;
      }
    }

    public Generator(SpaceObjectVessel vessel, VesselObjectID id, GeneratorData genData)
      : base(vessel, id)
    {
      GeneratorData generatorData = ObjectCopier.DeepCopy<GeneratorData>(genData, 10);
      this._OperationRate = generatorData.OutputRate;
      this._NominalOutput = generatorData.NominalOutput;
      this.ResourceRequirements = DistributionManager.ResourceRequirementsToDictionary(generatorData.ResourceRequirements);
      if (generatorData.SpawnSettings != null)
      {
        foreach (SystemSpawnSettings spawnSetting in generatorData.SpawnSettings)
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
      this._PowerUpTime = generatorData.PowerUpTime;
      this._CoolDownTime = generatorData.CoolDownTime;
      this.Status = generatorData.Status;
      this._AutoReactivate = generatorData.AutoReactivate;
      this.SetAuxData(generatorData.AuxData);
      this._MaxOutput = this._NominalOutput;
      this.ParentVessel = vessel;
    }

    public virtual IAuxDetails GetAuxDetails()
    {
      return (IAuxDetails) null;
    }

    public virtual void SetDetails(GeneratorDetails details)
    {
      if (details.Status == SystemStatus.OnLine)
        this.GoOnLine();
      else if (details.Status == SystemStatus.OffLine)
        this.GoOffLine(false, false);
      this.OperationRate = details.OutputRate;
      this.SetAuxDetails(details.AuxDetails);
    }

    public virtual void SetAuxDetails(IAuxDetails auxDetails)
    {
    }
  }
}
