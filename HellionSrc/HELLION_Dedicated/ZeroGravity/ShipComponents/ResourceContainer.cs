// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.ResourceContainer
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using System.Linq;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;

namespace ZeroGravity.ShipComponents
{
  public class ResourceContainer : IResourceProvider, ICargo, IPersistantObject
  {
    private bool _IsInUse = true;
    private HashSet<IResourceConsumer> _ConnectedConsumers = new HashSet<IResourceConsumer>();
    private float _OutputRate = 1f;
    public bool StatusChanged = true;
    private List<CargoCompartmentData> _Compartments = new List<CargoCompartmentData>();
    public VesselObjectID ID;
    private float _QuantityChangeRate;
    private DistributionSystemType _OutputType;
    private float _Output;
    private float _NominalOutput;
    public SpaceObjectVessel ParentVessel;

    public List<CargoCompartmentData> Compartments
    {
      get
      {
        return this._Compartments;
      }
    }

    public float QuantityChangeRate
    {
      get
      {
        return this._QuantityChangeRate;
      }
      set
      {
        if ((double) this._QuantityChangeRate == (double) value)
          return;
        this.StatusChanged = true;
        this._QuantityChangeRate = value;
      }
    }

    public bool IsInUse
    {
      get
      {
        return this._IsInUse;
      }
      set
      {
        if (this._IsInUse == value)
          return;
        this.StatusChanged = true;
        this._IsInUse = value;
      }
    }

    public HashSet<IResourceConsumer> ConnectedConsumers
    {
      get
      {
        return this._ConnectedConsumers;
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
        if ((double) this._Output != (double) value)
          this.StatusChanged = true;
        this._Output = value;
      }
    }

    public DistributionSystemType OutputType
    {
      get
      {
        return this._OutputType;
      }
      set
      {
        this._OutputType = value;
      }
    }

    public float MaxOutput
    {
      get
      {
        return this._NominalOutput;
      }
    }

    public float OperationRate
    {
      get
      {
        return this._OutputRate;
      }
      set
      {
        if ((double) this._OutputRate != (double) value)
          this.StatusChanged = true;
        this._OutputRate = value;
      }
    }

    public ResourceContainer(SpaceObjectVessel vessel, VesselObjectID id, ResourceContainerData rcData)
    {
      ResourceContainerData resourceContainerData = ObjectCopier.DeepCopy<ResourceContainerData>(rcData, 10);
      this.ParentVessel = vessel;
      this.ID = id;
      this.OutputType = resourceContainerData.DistributionSystemType;
      CargoCompartmentData cargoCompartment = resourceContainerData.CargoCompartment;
      foreach (CargoResourceData resource in cargoCompartment.Resources)
      {
        foreach (ResourcesSpawnSettings spawnSetting in resource.SpawnSettings)
        {
          if (vessel.CheckTag(spawnSetting.Tag, spawnSetting.Case))
          {
            float num = MathHelper.RandomRange(spawnSetting.MinQuantity, spawnSetting.MaxQuantity);
            resource.Quantity = 0.0f;
            float max = cargoCompartment.Capacity - cargoCompartment.Resources.Sum<CargoResourceData>((Func<CargoResourceData, float>) (m => m.Quantity));
            resource.Quantity = MathHelper.Clamp(num, 0.0f, max);
            break;
          }
        }
      }
      this._Compartments = new List<CargoCompartmentData>()
      {
        cargoCompartment
      };
      this.NominalOutput = resourceContainerData.NominalOutput;
      this.IsInUse = resourceContainerData.IsInUse;
    }

    public float ConsumeResource(float consumeQuantity)
    {
      if (!this.IsInUse)
        return 0.0f;
      return this.ChangeQuantityBy((int) this._Compartments[0].ID, this._Compartments[0].Resources[0].ResourceType, -consumeQuantity, false);
    }

    public CargoCompartmentData GetCompartment(int? id = null)
    {
      if (id.HasValue)
        return this._Compartments.Find((Predicate<CargoCompartmentData>) (m => (int) m.ID == id.Value));
      return this._Compartments[0];
    }

    public float ChangeQuantityBy(int compartmentID, ResourceType resourceType, float quantity, bool wholeAmount = false)
    {
      CargoCompartmentData cargoCompartmentData = this.Compartments.Find((Predicate<CargoCompartmentData>) (m => (int) m.ID == compartmentID));
      CargoResourceData cargoResourceData = cargoCompartmentData.Resources.Find((Predicate<CargoResourceData>) (m => m.ResourceType == resourceType));
      if (cargoResourceData == null)
      {
        cargoResourceData = new CargoResourceData()
        {
          ResourceType = resourceType,
          Quantity = 0.0f
        };
        cargoCompartmentData.Resources.Add(cargoResourceData);
      }
      float quantity1 = cargoResourceData.Quantity;
      float num1 = quantity;
      float num2 = cargoCompartmentData.Capacity - cargoResourceData.Quantity;
      if ((double) quantity > 0.0 && (double) quantity > (double) num2)
        num1 = num2;
      else if ((double) quantity < 0.0 && -(double) num1 > (double) quantity1)
        num1 = -quantity1;
      cargoResourceData.Quantity = quantity1 + num1;
      if ((double) cargoResourceData.Quantity <= 0.01 && cargoCompartmentData.AllowOnlyOneType && cargoCompartmentData.AllowedResources.Count > 1)
        cargoCompartmentData.Resources.Remove(cargoResourceData);
      this.StatusChanged = true;
      return num1;
    }

    public PersistenceObjectData GetPersistenceData()
    {
      return (PersistenceObjectData) new PersistenceObjectDataCargo()
      {
        InSceneID = this.ID.InSceneID,
        CargoCompartments = this._Compartments
      };
    }

    public void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        this._Compartments = (persistenceData as PersistenceObjectDataCargo).CargoCompartments;
        foreach (CargoCompartmentData compartment in this._Compartments)
        {
          if (compartment.AllowOnlyOneType && compartment.AllowedResources.Count > 1)
            compartment.Resources.RemoveAll((Predicate<CargoResourceData>) (m => (double) m.Quantity <= 0.01));
        }
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
