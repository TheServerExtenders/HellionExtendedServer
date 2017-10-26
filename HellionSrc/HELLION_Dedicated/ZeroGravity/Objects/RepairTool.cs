// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.RepairTool
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
  internal class RepairTool : Item, ICargo
  {
    private Dictionary<ResourceType, float> ResourceChangedCounter = new Dictionary<ResourceType, float>();
    private RepairToolStats _StatsNew = new RepairToolStats();
    public float RepairAmount;
    public float UsageCooldown;
    public float Range;
    public CargoCompartmentData FuelCompartment;
    public float FuelConsumption;
    public bool Active;
    private List<CargoCompartmentData> _Compartments;

    private float maxFuel
    {
      get
      {
        return this.FuelCompartment.Capacity;
      }
    }

    private float currentFuel
    {
      get
      {
        return this.FuelCompartment.Resources.Count > 0 ? this.FuelCompartment.Resources[0].Quantity : 0.0f;
      }
    }

    public float FreeSpace
    {
      get
      {
        return this.FuelCompartment.Capacity - this.FuelCompartment.Resources.Sum<CargoResourceData>((Func<CargoResourceData, float>) (m => m.Quantity));
      }
    }

    public List<CargoCompartmentData> Compartments
    {
      get
      {
        return this._Compartments;
      }
    }

    public override DynamicObjectStats StatsNew
    {
      get
      {
        return (DynamicObjectStats) this._StatsNew;
      }
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      RepairToolStats repairToolStats = stats as RepairToolStats;
      if (repairToolStats.Active.HasValue)
      {
        this.Active = repairToolStats.Active.Value;
        this._StatsNew.Active = new bool?(this.Active);
      }
      this.DynamicObj.SendStatsToClient();
      return false;
    }

    public RepairTool(IDynamicObjectAuxData data)
    {
      if (data == null)
        return;
      this.SetData(data as RepairToolData);
    }

    private void SetData(RepairToolData rtd)
    {
      this.RepairAmount = rtd.RepairAmount;
      this.UsageCooldown = rtd.UsageCooldown;
      this.Range = rtd.Range;
      this.FuelCompartment = rtd.FuelCompartment;
      this.FuelConsumption = rtd.FuelConsumption;
      this.Health = rtd.Health;
      this.MaxHealth = rtd.MaxHealth;
      this._Compartments = new List<CargoCompartmentData>()
      {
        this.FuelCompartment
      };
      this._StatsNew.FuelResource = this.FuelCompartment.Resources[0];
      this._StatsNew.Active = new bool?(this.Active);
    }

    private RepairToolData GetData()
    {
      RepairToolData repairToolData = new RepairToolData();
      repairToolData.RepairAmount = this.RepairAmount;
      repairToolData.UsageCooldown = this.UsageCooldown;
      repairToolData.Range = this.Range;
      repairToolData.FuelCompartment = this.FuelCompartment;
      repairToolData.FuelConsumption = this.FuelConsumption;
      double health = (double) this.Health;
      repairToolData.Health = (float) health;
      double maxHealth = (double) this.MaxHealth;
      repairToolData.MaxHealth = (float) maxHealth;
      return repairToolData;
    }

    public void RepairVessel(VesselObjectID repairPointID)
    {
      SpaceObjectVessel vessel = Server.Instance.GetVessel(repairPointID.VesselGUID);
      if (vessel == null)
        return;
      VesselRepairPoint vesselRepairPoint = vessel.RepairPoints.Find((Predicate<VesselRepairPoint>) (m => m.ID.Equals((object) repairPointID)));
      if (vesselRepairPoint != null)
      {
        float num1 = this.RepairAmount;
        if ((double) (num1 * this.FuelConsumption) > (double) this.currentFuel)
          num1 = this.currentFuel / this.FuelConsumption;
        float num2 = (double) vesselRepairPoint.MaxHealth - (double) vesselRepairPoint.Health < (double) num1 ? vesselRepairPoint.MaxHealth - vesselRepairPoint.Health : num1;
        double num3 = (double) vessel.ChangeHealthBy(num2, (List<VesselRepairPoint>) null);
        vesselRepairPoint.Health += num2;
        if ((double) vessel.MaxHealth - (double) vessel.Health < 1.40129846432482E-45)
          vesselRepairPoint.Health = vesselRepairPoint.MaxHealth;
        if ((double) num2 > 0.0)
        {
          CargoResourceData resource = this.FuelCompartment.Resources[0];
          double num4 = (double) this.ChangeQuantityBy((int) this.FuelCompartment.ID, resource.ResourceType, -num2 * this.FuelConsumption, false);
          this._StatsNew.FuelResource = resource;
          this.DynamicObj.SendStatsToClient();
        }
      }
    }

    public void RepairItem(long guid)
    {
      SpaceObject spaceObject = Server.Instance.GetObject(guid);
      if (!(spaceObject is DynamicObject))
        return;
      Item obj = (spaceObject as DynamicObject).Item;
      if (obj != null)
      {
        float num1 = this.RepairAmount;
        if ((double) (num1 * this.FuelConsumption) > (double) this.currentFuel)
          num1 = this.currentFuel / this.FuelConsumption;
        float health = obj.Health;
        obj.Health += num1;
        float num2 = obj.Health - health;
        if ((double) num2 > 0.0)
          this.ConsumeFuel(num2 * this.FuelConsumption);
      }
    }

    public void ConsumeFuel(float amount)
    {
      CargoResourceData resource = this.FuelCompartment.Resources[0];
      double num = (double) this.ChangeQuantityBy((int) this.FuelCompartment.ID, resource.ResourceType, -amount, false);
      this._StatsNew.FuelResource = resource;
      this.DynamicObj.SendStatsToClient();
      this.DynamicObj.SendStatsToClient();
    }

    public CargoCompartmentData GetCompartment(int? id = null)
    {
      if (id.HasValue)
        return this._Compartments.Find((Predicate<CargoCompartmentData>) (m => (int) m.ID == id.Value));
      return this._Compartments[0];
    }

    private float changeQuantityBy(ResourceType resourceType, float quantity, bool sendStats = true)
    {
      CargoResourceData cargoResourceData = this.FuelCompartment.Resources.Find((Predicate<CargoResourceData>) (m => m.ResourceType == resourceType));
      if (cargoResourceData == null)
      {
        cargoResourceData = new CargoResourceData()
        {
          ResourceType = resourceType,
          Quantity = 0.0f
        };
        this.FuelCompartment.Resources.Add(cargoResourceData);
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
      if ((double) this.ResourceChangedCounter[resourceType] <= 1.40129846432482E-45)
        this.ResourceChangedCounter[resourceType] = 0.0f;
      this.DynamicObj.StatsChanged = true;
      if ((double) Math.Abs(this.ResourceChangedCounter[resourceType] / this.FuelCompartment.Capacity) >= 0.00999999977648258)
      {
        this.DynamicObj.SendStatsToClient();
        this.ResourceChangedCounter[resourceType] = 0.0f;
      }
      return num;
    }

    public float ChangeQuantityBy(int compartmentID, ResourceType resourceType, float quantity, bool wholeAmount = false)
    {
      float num = this.changeQuantityBy(resourceType, quantity, true);
      this._StatsNew.FuelResource = this.GetCompartment(new int?(compartmentID)).Resources[0];
      this.DynamicObj.SendStatsToClient();
      return num;
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataRepairTool objectDataRepairTool = new PersistenceObjectDataRepairTool();
      this.FillPersistenceData((PersistenceObjectDataItem) objectDataRepairTool);
      objectDataRepairTool.RepairToolData = this.GetData();
      return (PersistenceObjectData) objectDataRepairTool;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataRepairTool objectDataRepairTool = persistenceData as PersistenceObjectDataRepairTool;
        if (objectDataRepairTool == null)
          Dbg.Warning((object) "PersistenceObjectDataJetpack data is null", (object) this.GUID);
        else
          this.SetData(objectDataRepairTool.RepairToolData);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
