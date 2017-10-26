// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.VesselComponent
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public abstract class VesselComponent : IResourceConsumer, IPersistantObject
  {
    protected Dictionary<VesselObjectID, MachineryPart> machineryParts = new Dictionary<VesselObjectID, MachineryPart>();
    protected Dictionary<VesselObjectID, MachineryPartSlotData> machineryPartSlots = new Dictionary<VesselObjectID, MachineryPartSlotData>();
    protected float _InputFactor = 1f;
    protected float _InputFactorStandby = 1f;
    private bool _Defective = false;
    protected float powerUpFactor = 1f;
    protected float cooldownFactor = 1f;
    protected SystemStatus _Status = SystemStatus.None;
    public string _DebugInfo = (string) null;
    protected SystemSecondaryStatus _SecondaryStatus = SystemSecondaryStatus.None;
    protected float _OperationRate = 0.0f;
    private Dictionary<DistributionSystemType, SortedSet<IResourceProvider>> _ConnectedProviders = new Dictionary<DistributionSystemType, SortedSet<IResourceProvider>>();
    private Dictionary<DistributionSystemType, ResourceRequirement> _ResourcesConsumption = new Dictionary<DistributionSystemType, ResourceRequirement>();
    private Dictionary<DistributionSystemType, HashSet<ResourceContainer>> _ResourceContainers = new Dictionary<DistributionSystemType, HashSet<ResourceContainer>>();
    private float statusChangeCountdown = 0.0f;
    public bool StatusChanged = true;
    public bool IsPowerConsumer = false;
    protected float _PowerUpTime;
    protected float _CoolDownTime;
    protected bool _AutoReactivate;
    protected bool autoReactivate;
    public SpaceObjectVessel ParentVessel;

    public VesselObjectID ID { get; set; }

    public Room Room { get; set; }

    public virtual bool AutoReactivate
    {
      get
      {
        return this._AutoReactivate;
      }
    }

    public bool CanReactivate
    {
      get
      {
        return this._AutoReactivate && this.autoReactivate;
      }
    }

    public virtual float PowerUpTime
    {
      get
      {
        return this._PowerUpTime * this.powerUpFactor;
      }
    }

    public bool Defective
    {
      get
      {
        return this._Defective;
      }
      set
      {
        if (this._Defective == (this._Defective = value))
          return;
        if (value && (this.Status == SystemStatus.OnLine || this.Status == SystemStatus.PowerUp))
          this.GoOffLine(true, false);
        else if (value)
          this.SecondaryStatus = SystemSecondaryStatus.Defective;
        else if ((this.Status == SystemStatus.OffLine || this.Status == SystemStatus.CoolDown) && this.autoReactivate)
          this.GoOnLine();
        else
          this.SecondaryStatus = SystemSecondaryStatus.None;
      }
    }

    public virtual float CoolDownTime
    {
      get
      {
        if ((double) this._CoolDownTime < 1.0)
          return 1f;
        return this._CoolDownTime * this.cooldownFactor;
      }
    }

    public virtual float InputFactor
    {
      get
      {
        return this._InputFactor;
      }
      set
      {
        if ((double) this._InputFactor == (double) value)
          return;
        this._InputFactor = value;
        this.StatusChanged = true;
      }
    }

    public virtual float InputFactorStandby
    {
      get
      {
        return this._InputFactorStandby;
      }
      set
      {
        if ((double) this._InputFactorStandby == (double) value)
          return;
        this._InputFactorStandby = value;
        this.StatusChanged = true;
      }
    }

    public string DebugInfo
    {
      get
      {
        return this._DebugInfo;
      }
      set
      {
        if (!(this._DebugInfo != value))
          return;
        this._DebugInfo = value;
        this.StatusChanged = true;
      }
    }

    public virtual SystemStatus Status
    {
      get
      {
        return this._Status;
      }
      protected set
      {
        if (this._Status == value)
          return;
        this._Status = value;
        if (this.Status == SystemStatus.OnLine)
          this._SecondaryStatus = (double) this.OperationRate <= 1.40129846432482E-45 ? SystemSecondaryStatus.Idle : SystemSecondaryStatus.None;
        this.StatusChanged = true;
      }
    }

    public virtual SystemSecondaryStatus SecondaryStatus
    {
      get
      {
        return this._SecondaryStatus;
      }
      protected set
      {
        if (this._SecondaryStatus == value)
          return;
        this._SecondaryStatus = value;
        this.StatusChanged = true;
      }
    }

    public float OperationRate
    {
      get
      {
        return this._OperationRate;
      }
      set
      {
        if ((double) this._OperationRate == (double) value)
          return;
        this._OperationRate = value;
        if (this.Status == SystemStatus.OnLine)
          this._SecondaryStatus = (double) this.OperationRate <= 1.40129846432482E-45 ? SystemSecondaryStatus.Idle : SystemSecondaryStatus.None;
        this.StatusChanged = true;
      }
    }

    public Dictionary<DistributionSystemType, SortedSet<IResourceProvider>> ConnectedProviders
    {
      get
      {
        return this._ConnectedProviders;
      }
    }

    public virtual Dictionary<DistributionSystemType, ResourceRequirement> ResourceRequirements
    {
      get
      {
        return this._ResourcesConsumption;
      }
      set
      {
        this._ResourcesConsumption = value;
        this.IsPowerConsumer = this._ResourcesConsumption.ContainsKey(DistributionSystemType.Power);
      }
    }

    public Dictionary<DistributionSystemType, HashSet<ResourceContainer>> ResourceContainers
    {
      get
      {
        return this._ResourceContainers;
      }
    }

    public VesselComponent(SpaceObjectVessel vessel, VesselObjectID id)
    {
      this.ID = id;
      this.ParentVessel = vessel;
    }

    public virtual void FitPartToSlot(VesselObjectID slotKey, MachineryPart part)
    {
      this.machineryParts[slotKey] = part;
    }

    public virtual void RemovePartFromSlot(VesselObjectID slotKey)
    {
      this.machineryParts[slotKey] = (MachineryPart) null;
    }

    public virtual void InitMachineryPartSlot(VesselObjectID slotKey, MachineryPart part, MachineryPartSlotData partSlotData)
    {
      this.FitPartToSlot(slotKey, part);
      this.machineryPartSlots[slotKey] = ObjectCopier.DeepCopy<MachineryPartSlotData>(partSlotData, 10);
    }

    public void SetMachineryPartSlotActive(VesselObjectID slotKey, bool state)
    {
      this.machineryPartSlots[slotKey].IsActive = state;
    }

    public virtual void GoOffLine(bool autoRestart, bool malfunction = false)
    {
      this.autoReactivate = autoRestart && this.AutoReactivate;
      if (this.Status == SystemStatus.OnLine || this.Status == SystemStatus.PowerUp)
      {
        if ((double) this.CoolDownTime > 0.0)
        {
          this.statusChangeCountdown = this.CoolDownTime;
          this.Status = SystemStatus.CoolDown;
        }
        else
          this.Status = SystemStatus.OffLine;
      }
      this.SecondaryStatus = !this.Defective ? (!(this.autoReactivate & malfunction) ? SystemSecondaryStatus.None : SystemSecondaryStatus.Malfunction) : SystemSecondaryStatus.Defective;
      this.DebugInfo = "";
    }

    public virtual void GoOnLine()
    {
      if (this.Status != SystemStatus.OffLine)
        return;
      Dictionary<IResourceProvider, float> reservedCapacities = new Dictionary<IResourceProvider, float>();
      Dictionary<ResourceContainer, float> reservedQuantities = new Dictionary<ResourceContainer, float>();
      string debugText = (string) null;
      if (this.CheckAvailableResources(this.OperationRate, 1f, this is Generator, ref reservedCapacities, ref reservedQuantities, ref debugText))
      {
        if ((double) this.PowerUpTime > 0.0)
        {
          this.statusChangeCountdown = this.PowerUpTime;
          this.Status = SystemStatus.PowerUp;
        }
        else
          this.Status = SystemStatus.OnLine;
      }
      else
      {
        this.autoReactivate = this.AutoReactivate;
        if (this.Defective)
          this.SecondaryStatus = SystemSecondaryStatus.Defective;
        else if (this.autoReactivate)
          this.SecondaryStatus = SystemSecondaryStatus.Malfunction;
      }
      this.DebugInfo = debugText;
      debugText = "Vessel " + (object) this.ParentVessel.GUID + "\n" + debugText;
    }

    public virtual void Update(double duration)
    {
      this.statusChangeCountdown = this.statusChangeCountdown - (float) duration;
      if ((double) this.statusChangeCountdown <= 0.0)
      {
        this.statusChangeCountdown = 0.0f;
        if (this.Status == SystemStatus.PowerUp)
          this.Status = SystemStatus.OnLine;
        else if (this.Status == SystemStatus.CoolDown)
          this.Status = SystemStatus.OffLine;
      }
      if (this.Status == SystemStatus.OnLine)
      {
        foreach (KeyValuePair<VesselObjectID, MachineryPartSlotData> machineryPartSlot in this.machineryPartSlots)
        {
          float partDecay = machineryPartSlot.Value.PartDecay;
          MachineryPart machineryPart = (MachineryPart) null;
          this.machineryParts.TryGetValue(machineryPartSlot.Key, out machineryPart);
          if (machineryPartSlot.Value.IsActive && machineryPart != null && (double) machineryPart.Health > 0.0)
          {
            int health = (int) machineryPart.Health;
            machineryPart.Health = MathHelper.Clamp(machineryPart.Health - (float) ((double) machineryPart.WearMultiplier * (double) partDecay / 3600.0 * duration), 0.0f, machineryPart.MaxHealth);
            if (health != (int) machineryPart.Health)
              machineryPart.DynamicObj.SendStatsToClient();
          }
        }
      }
      else
      {
        if (this.Status != SystemStatus.OffLine || !this.autoReactivate)
          return;
        this.GoOnLine();
      }
    }

    public virtual bool CheckAvailableResources(float consumptionFactor, float duration, bool standby, ref Dictionary<IResourceProvider, float> reservedCapacities, ref Dictionary<ResourceContainer, float> reservedQuantities, ref string debugText)
    {
      if (this.Defective)
      {
        string[] strArray = this.GetType().ToString().Split('.');
        debugText = debugText + strArray[strArray.Length - 1] + ": MALFUNCTION\n";
        return false;
      }
      bool flag1 = true;
      Dictionary<IResourceProvider, float> dictionary1 = new Dictionary<IResourceProvider, float>((IDictionary<IResourceProvider, float>) reservedCapacities);
      Dictionary<ResourceContainer, float> dictionary2 = new Dictionary<ResourceContainer, float>((IDictionary<ResourceContainer, float>) reservedQuantities);
      foreach (DistributionSystemType key1 in this.ResourceRequirements.Keys)
      {
        ResourceRequirement resourceRequirement = this.ResourceRequirements[key1];
        float num1 = !standby ? resourceRequirement.Nominal * consumptionFactor * this._InputFactor : resourceRequirement.Standby * this.InputFactorStandby;
        if (this.ConnectedProviders.ContainsKey(key1) && (double) num1 > 0.0)
        {
          foreach (IResourceProvider key2 in this.ConnectedProviders[key1])
          {
            bool flag2 = true;
            if (key2 is Generator)
            {
              Generator generator = (Generator) key2;
              if (generator.Status == SystemStatus.OnLine && (double) generator.MaxOutput > 1.40129846432482E-45)
              {
                Dictionary<IResourceProvider, float> reservedCapacities1 = new Dictionary<IResourceProvider, float>((IDictionary<IResourceProvider, float>) reservedCapacities);
                Dictionary<ResourceContainer, float> reservedQuantities1 = new Dictionary<ResourceContainer, float>((IDictionary<ResourceContainer, float>) reservedQuantities);
                if (generator is GeneratorCapacitor)
                {
                  GeneratorCapacitor generatorCapacitor = generator as GeneratorCapacitor;
                  flag2 = (double) generatorCapacitor.Capacity >= (double) num1;
                  if (flag2)
                    generatorCapacitor.Capacity -= num1;
                }
                else
                  flag2 = generator.CheckAvailableResources((double) generator.MaxOutput > 0.0 ? MathHelper.Clamp(num1 / generator.MaxOutput, 0.0f, 1f) : 1f, duration, standby, ref reservedCapacities1, ref reservedQuantities1, ref debugText);
                if (flag2)
                {
                  reservedCapacities = reservedCapacities1;
                  reservedQuantities = reservedQuantities1;
                }
              }
              else
                flag2 = false;
            }
            else if (key2 is RoomAirTank && (double) (key2 as RoomAirTank).Quantity < (double) num1)
              flag2 = false;
            if (flag2)
            {
              float num2 = 0.0f;
              if (reservedCapacities != null && reservedCapacities.ContainsKey(key2))
                num2 = reservedCapacities[key2];
              float num3 = key2.MaxOutput - num2;
              float num4 = 0.0f;
              if (key2 is ResourceContainer)
              {
                reservedQuantities.TryGetValue(key2 as ResourceContainer, out num4);
                float num5 = (key2 as ResourceContainer).GetCompartment(new int?()).Resources[0].Quantity - num4;
                if ((double) num3 * (double) duration > (double) num5)
                  num3 = num5 / duration;
              }
              if ((double) num3 >= (double) num1)
              {
                if (key2 is ResourceContainer)
                  reservedQuantities[key2 as ResourceContainer] = num1 * duration + num4;
                reservedCapacities[key2] = num2 + num1;
                num1 = 0.0f;
                break;
              }
              if (key2 is ResourceContainer && reservedCapacities.ContainsKey(key2))
                reservedQuantities[key2 as ResourceContainer] = (key2.MaxOutput - reservedCapacities[key2]) * duration + num4;
              reservedCapacities[key2] = key2.MaxOutput;
              num1 -= num3;
            }
          }
        }
        if ((double) num1 > 0.0)
        {
          flag1 = false;
          if (debugText.IsNullOrEmpty())
            debugText = "";
          string[] strArray = this.GetType().ToString().Split('.');
          debugText = debugText + strArray[strArray.Length - 1] + ": " + (object) num1 + " of " + key1.ToString() + " short\n";
        }
      }
      if (this is GeneratorCapacitor)
        return true;
      if (!flag1)
      {
        reservedCapacities = dictionary1;
        reservedQuantities = dictionary2;
      }
      return flag1;
    }

    public void CheckStatus(float operationRate, float duration, bool standby, ref Dictionary<IResourceProvider, float> reservedCapacities, ref Dictionary<ResourceContainer, float> reservedQuantities)
    {
      if (this.Status != SystemStatus.OnLine && (this.Status != SystemStatus.OffLine || !this.CanReactivate))
        return;
      Dictionary<IResourceProvider, float> dictionary1 = new Dictionary<IResourceProvider, float>((IDictionary<IResourceProvider, float>) reservedCapacities);
      Dictionary<ResourceContainer, float> dictionary2 = new Dictionary<ResourceContainer, float>((IDictionary<ResourceContainer, float>) reservedQuantities);
      string debugText = (string) null;
      bool flag = this.CheckAvailableResources(operationRate, duration, standby, ref reservedCapacities, ref reservedQuantities, ref debugText);
      debugText = "Vessel " + (object) this.ParentVessel.GUID + "\n" + debugText;
      if (flag && this.Status == SystemStatus.OffLine && this.CanReactivate)
        this.GoOnLine();
      else if (!flag)
      {
        if (this.Status == SystemStatus.OnLine)
          this.GoOffLine(true, true);
        reservedCapacities = dictionary1;
        reservedQuantities = dictionary2;
      }
    }

    public PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataVesselComponent dataVesselComponent = new PersistenceObjectDataVesselComponent();
      long guid = this.ParentVessel.GUID;
      dataVesselComponent.GUID = guid;
      int inSceneId = this.ID.InSceneID;
      dataVesselComponent.InSceneID = inSceneId;
      int status = (int) this.Status;
      dataVesselComponent.Status = (SystemStatus) status;
      double statusChangeCountdown = (double) this.statusChangeCountdown;
      dataVesselComponent.StatusChangeCountdown = (float) statusChangeCountdown;
      int num1 = this.autoReactivate ? 1 : 0;
      dataVesselComponent.AutoReactivate = num1 != 0;
      int num2 = this.Defective ? 1 : 0;
      dataVesselComponent.Defective = num2 != 0;
      return (PersistenceObjectData) dataVesselComponent;
    }

    public void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        PersistenceObjectDataVesselComponent dataVesselComponent = persistenceData as PersistenceObjectDataVesselComponent;
        if (dataVesselComponent == null)
        {
          Dbg.Warning("PersistenceObjectDataVesselComponent data is null");
        }
        else
        {
          this.autoReactivate = dataVesselComponent.AutoReactivate;
          this.statusChangeCountdown = dataVesselComponent.StatusChangeCountdown;
          this._Status = dataVesselComponent.Status;
          this.Defective = dataVesselComponent.Defective;
        }
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
