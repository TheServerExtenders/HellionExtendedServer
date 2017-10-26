// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.Canister
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using System.Linq;
using ZeroGravity.Data;
using ZeroGravity.Network;
using ZeroGravity.ShipComponents;

namespace ZeroGravity.Objects
{
  internal class Canister : Item, ICargo
  {
    private Dictionary<ResourceType, float> ResourceChangedCounter = new Dictionary<ResourceType, float>();
    private CargoCompartmentData cargoCompartment;
    private List<CargoCompartmentData> _compartments;

    public override DynamicObjectStats StatsNew
    {
      get
      {
        return (DynamicObjectStats) new CanisterStats()
        {
          Resources = new List<CargoResourceData>((IEnumerable<CargoResourceData>) this.cargoCompartment.Resources)
        };
      }
    }

    public bool HasSpace
    {
      get
      {
        return (double) this.FreeSpace > 1.40129846432482E-45;
      }
    }

    public float FreeSpace
    {
      get
      {
        return this.cargoCompartment.Capacity - this.cargoCompartment.Resources.Sum<CargoResourceData>((Func<CargoResourceData, float>) (m => m.Quantity));
      }
    }

    public List<CargoCompartmentData> Compartments
    {
      get
      {
        return this._compartments;
      }
    }

    public Canister(IDynamicObjectAuxData data)
    {
      if (data == null)
        return;
      CanisterData canisterData = data as CanisterData;
      this.Health = canisterData.Health;
      this.MaxHealth = canisterData.MaxHealth;
      this.SetCanisterData(canisterData.CargoCompartment);
    }

    private void SetCanisterData(CargoCompartmentData compartmetData)
    {
      this.cargoCompartment = compartmetData;
      this._compartments = new List<CargoCompartmentData>()
      {
        this.cargoCompartment
      };
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      CanisterStats canisterStats = stats as CanisterStats;
      if (canisterStats == null || (!canisterStats.UseCanister.HasValue || !canisterStats.UseCanister.Value || !(this.DynamicObj.Parent is Player)))
        return false;
      Player parent = this.DynamicObj.Parent as Player;
      if (parent.CurrentJetpack != null)
      {
        foreach (CargoCompartmentData compartment1 in parent.CurrentJetpack.Compartments)
        {
          foreach (CargoCompartmentData compartment2 in this.Compartments)
          {
            List<CargoResourceData> cargoResourceDataList = new List<CargoResourceData>();
            foreach (CargoResourceData resource in compartment2.Resources)
            {
              CargoResourceData resC = resource;
              if ((double) resC.Quantity > 0.0 && compartment1.AllowedResources.Contains(resC.ResourceType) && compartment1.AllowOnlyOneType)
              {
                CargoResourceData cargoResourceData = compartment1.Resources.Find((Predicate<CargoResourceData>) (x => x.ResourceType == resC.ResourceType));
                bool flag = false;
                if (cargoResourceData == null)
                {
                  flag = true;
                  cargoResourceData = new CargoResourceData();
                  cargoResourceData.ResourceType = resC.ResourceType;
                }
                float num = compartment1.Capacity - cargoResourceData.Quantity;
                if ((double) resC.Quantity <= (double) num)
                {
                  cargoResourceData.Quantity += resC.Quantity;
                  resC.Quantity = 0.0f;
                  cargoResourceDataList.Add(resC);
                }
                else
                {
                  cargoResourceData.Quantity += num;
                  resC.Quantity = resC.Quantity - num;
                }
                if (flag)
                  compartment1.Resources.Add(cargoResourceData);
              }
            }
            foreach (CargoResourceData cargoResourceData in cargoResourceDataList)
              compartment2.Resources.Remove(cargoResourceData);
          }
        }
        this.DynamicObj.SendStatsToClient();
        parent.CurrentJetpack.DynamicObj.SendStatsToClient();
      }
      return false;
    }

    public void ChangeQuantity(Dictionary<ResourceType, float> newResources)
    {
      foreach (KeyValuePair<ResourceType, float> newResource in newResources)
      {
        double num = (double) this.changeQuantityBy(newResource.Key, newResource.Value, false);
      }
      this.DynamicObj.SendStatsToClient();
    }

    private float changeQuantityBy(ResourceType resourceType, float quantity, bool sendStats = true)
    {
      CargoResourceData cargoResourceData = this.cargoCompartment.Resources.Find((Predicate<CargoResourceData>) (m => m.ResourceType == resourceType));
      if (cargoResourceData == null)
      {
        cargoResourceData = new CargoResourceData()
        {
          ResourceType = resourceType,
          Quantity = 0.0f
        };
        this.cargoCompartment.Resources.Add(cargoResourceData);
      }
      float freeSpace = this.FreeSpace;
      float num = quantity;
      float quantity1 = cargoResourceData.Quantity;
      if ((double) quantity > 0.0 && (double) quantity > (double) freeSpace)
        num = freeSpace;
      else if ((double) quantity < 0.0 && -(double) num > (double) quantity1)
        num = -quantity1;
      cargoResourceData.Quantity = quantity1 + num;
      if (this.ResourceChangedCounter.ContainsKey(resourceType))
      {
        Dictionary<ResourceType, float> resourceChangedCounter = this.ResourceChangedCounter;
        ResourceType index = resourceType;
        resourceChangedCounter[index] = resourceChangedCounter[index] + num;
      }
      else
        this.ResourceChangedCounter[resourceType] = num;
      if ((double) cargoResourceData.Quantity <= 1.40129846432482E-45)
        this.cargoCompartment.Resources.Remove(cargoResourceData);
      this.DynamicObj.StatsChanged = true;
      if ((double) Math.Abs(this.ResourceChangedCounter[resourceType] / this.cargoCompartment.Capacity) >= 0.00999999977648258)
      {
        this.DynamicObj.SendStatsToClient();
        this.ResourceChangedCounter[resourceType] = 0.0f;
      }
      return num;
    }

    public CargoCompartmentData GetCompartment(int? id = null)
    {
      if (id.HasValue)
        return this._compartments.Find((Predicate<CargoCompartmentData>) (m => (int) m.ID == id.Value));
      return this._compartments[0];
    }

    public float ChangeQuantityBy(int compartmentID, ResourceType resourceType, float quantity, bool wholeAmount = false)
    {
      return this.changeQuantityBy(resourceType, quantity, true);
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataCanister objectDataCanister = new PersistenceObjectDataCanister();
      this.FillPersistenceData((PersistenceObjectDataItem) objectDataCanister);
      objectDataCanister.Compartment = this.cargoCompartment;
      return (PersistenceObjectData) objectDataCanister;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataCanister objectDataCanister = persistenceData as PersistenceObjectDataCanister;
        if (objectDataCanister == null)
          Dbg.Warning((object) "PersistenceObjectDataCanister data is null", (object) this.GUID);
        else
          this.SetCanisterData(objectDataCanister.Compartment);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
