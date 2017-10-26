// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.Jetpack
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
  public class Jetpack : Item, IItemWithPower, ICargo
  {
    private Dictionary<int, float> ResourceChangedCounter = new Dictionary<int, float>();
    private float maxPower;
    private float currentPower;
    private CargoCompartmentData OxygenCompartment;
    private CargoCompartmentData PropellantCompartment;
    public float OxygenConsumption;
    public float PropellantConsumption;
    public Helmet Helmet;
    private List<CargoCompartmentData> _Compartments;
    private float accumulatedPower;

    public override DynamicObjectStats StatsNew
    {
      get
      {
        this.accumulatedPower = 0.0f;
        return (DynamicObjectStats) new JetpackStats()
        {
          CurrentPower = new float?(this.currentPower),
          Oxygen = (this.OxygenCompartment.Resources == null || this.OxygenCompartment.Resources.Count <= 0 ? (CargoResourceData) null : this.OxygenCompartment.Resources[0]),
          Propellant = (this.PropellantCompartment.Resources == null || this.PropellantCompartment.Resources.Count <= 0 ? (CargoResourceData) null : this.PropellantCompartment.Resources[0])
        };
      }
    }

    public bool HasFuel
    {
      get
      {
        return (double) this.currentFuel > 1.40129846432482E-45;
      }
    }

    public bool HasPower
    {
      get
      {
        return (double) this.currentPower > 1.40129846432482E-45;
      }
    }

    public bool HasOxygen
    {
      get
      {
        return (double) this.currentOxygen > 1.40129846432482E-45;
      }
    }

    private float maxFuel
    {
      get
      {
        return this.PropellantCompartment.Capacity;
      }
    }

    private float currentFuel
    {
      get
      {
        return this.PropellantCompartment.Resources.Count > 0 ? this.PropellantCompartment.Resources[0].Quantity : 0.0f;
      }
    }

    private float maxOxygen
    {
      get
      {
        return this.OxygenCompartment.Capacity;
      }
    }

    private float currentOxygen
    {
      get
      {
        return this.OxygenCompartment.Resources.Count > 0 ? this.OxygenCompartment.Resources[0].Quantity : 0.0f;
      }
    }

    public List<CargoCompartmentData> Compartments
    {
      get
      {
        return this._Compartments;
      }
    }

    public float ChargeAmount
    {
      get
      {
        return 1f;
      }
    }

    public Jetpack(IDynamicObjectAuxData data)
    {
      if (data == null)
        return;
      this.SetJetpackData(data as JetpackData);
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      return false;
    }

    protected override void ChangeEquip(Inventory.EquipType equipType)
    {
      if (!(this.DynamicObj.Parent is Player))
        return;
      Player parent = this.DynamicObj.Parent as Player;
      if (equipType == Inventory.EquipType.EquipInventory)
      {
        parent.CurrentJetpack = this;
        if (parent.CurrentHelmet == null)
          return;
        this.Helmet = parent.CurrentHelmet;
        this.Helmet.Jetpack = this;
      }
      else
      {
        if (equipType != Inventory.EquipType.Hands && equipType != Inventory.EquipType.None || parent.CurrentJetpack != this)
          return;
        parent.CurrentJetpack = (Jetpack) null;
        if (this.Helmet != null)
        {
          this.Helmet.Jetpack = (Jetpack) null;
          this.Helmet.UpdateStats(false);
          this.Helmet = (Helmet) null;
        }
      }
    }

    public void ConsumeResources(float? propellant = null, float? power = null, float? oxygen = null)
    {
      if (power.HasValue)
      {
        float num = power.Value;
        if ((double) num < 0.0)
          num = 0.0f;
        this.currentPower = (double) this.currentPower <= (double) num ? 0.0f : this.currentPower - num;
        this.DynamicObj.SendStatsToClient();
      }
      if (oxygen.HasValue)
      {
        float num1 = oxygen.Value;
        if ((double) num1 < 0.0)
          num1 = 0.0f;
        if (this.OxygenCompartment.Resources.Count > 0 && (double) num1 > 0.0)
        {
          double num2 = (double) this.ChangeQuantityBy((int) this.OxygenCompartment.ID, this.OxygenCompartment.Resources[0].ResourceType, -num1, false);
        }
      }
      if (!propellant.HasValue)
        return;
      float num3 = propellant.Value;
      if ((double) num3 < 0.0)
        num3 = 0.0f;
      if (this.PropellantCompartment.Resources.Count > 0 && (double) num3 >= 0.0)
      {
        double num4 = (double) this.ChangeQuantityBy((int) this.PropellantCompartment.ID, this.PropellantCompartment.Resources[0].ResourceType, -num3, false);
      }
    }

    public static bool CanChangeValue(float currentAmount, float amount, float min, float max)
    {
      return amount.IsNotEpsilonZero(float.Epsilon) && ((double) amount >= 0.0 || !currentAmount.IsEpsilonEqual(min, float.Epsilon)) || (double) amount > 0.0 && currentAmount.IsEpsilonEqual(max, float.Epsilon);
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
      float num1 = cargoCompartmentData.Capacity - cargoCompartmentData.Resources.Sum<CargoResourceData>((Func<CargoResourceData, float>) (m => m.Quantity));
      float num2 = quantity;
      float quantity1 = cargoResourceData.Quantity;
      if ((double) quantity > 0.0 && (double) quantity > (double) num1)
        num2 = num1;
      else if ((double) quantity < 0.0 && -(double) num2 > (double) quantity1)
        num2 = -quantity1;
      cargoResourceData.Quantity = quantity1 + num2;
      if (this.ResourceChangedCounter.ContainsKey(compartmentID))
      {
        Dictionary<int, float> resourceChangedCounter = this.ResourceChangedCounter;
        int index = compartmentID;
        resourceChangedCounter[index] = resourceChangedCounter[index] + num2;
      }
      else
        this.ResourceChangedCounter[compartmentID] = num2;
      if ((double) Math.Abs(this.ResourceChangedCounter[compartmentID]) / (double) cargoCompartmentData.Capacity >= 0.00999999977648258)
      {
        this.DynamicObj.SendStatsToClient();
        this.ResourceChangedCounter[compartmentID] = 0.0f;
      }
      if ((double) cargoResourceData.Quantity <= 1.40129846432482E-45 && !cargoCompartmentData.AllowOnlyOneType)
        cargoCompartmentData.Resources.Remove(cargoResourceData);
      return num2;
    }

    public void ChangePowerQuantity(float amount)
    {
      this.currentPower = Math.Min(this.currentPower + amount, this.maxPower);
      this.accumulatedPower = this.accumulatedPower + amount;
      if ((double) this.currentPower < 1.40129846432482E-45)
      {
        this.currentPower = 0.0f;
        this.DynamicObj.SendStatsToClient();
      }
      else if ((double) this.accumulatedPower > (double) this.maxPower * 0.00999999977648258)
        this.DynamicObj.SendStatsToClient();
      else
        this.DynamicObj.StatsChanged = true;
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataJetpack objectDataJetpack = new PersistenceObjectDataJetpack();
      this.FillPersistenceData((PersistenceObjectDataItem) objectDataJetpack);
      objectDataJetpack.JetpackData = this.GetJetpackData();
      return (PersistenceObjectData) objectDataJetpack;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        base.LoadPersistenceData(persistenceData);
        PersistenceObjectDataJetpack objectDataJetpack = persistenceData as PersistenceObjectDataJetpack;
        if (objectDataJetpack == null)
          Dbg.Warning((object) "PersistenceObjectDataJetpack data is null", (object) this.GUID);
        else
          this.SetJetpackData(objectDataJetpack.JetpackData);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }

    private void SetJetpackData(JetpackData jd)
    {
      this.maxPower = jd.MaxPower;
      this.currentPower = Math.Min(jd.CurrentPower, this.maxPower);
      this.OxygenCompartment = jd.OxygenCompartment;
      this.PropellantCompartment = jd.PropellantCompartment;
      this._Compartments = new List<CargoCompartmentData>()
      {
        this.OxygenCompartment,
        this.PropellantCompartment
      };
      this.OxygenConsumption = jd.OxygenConsumption;
      this.PropellantConsumption = jd.PropellantConsumption;
      this.MaxHealth = jd.MaxHealth;
      this.Health = jd.Health;
    }

    private JetpackData GetJetpackData()
    {
      JetpackData jetpackData = new JetpackData();
      jetpackData.MaxPower = this.maxPower;
      jetpackData.CurrentPower = this.currentPower;
      jetpackData.OxygenCompartment = this.OxygenCompartment;
      jetpackData.PropellantCompartment = this.PropellantCompartment;
      jetpackData.OxygenConsumption = this.OxygenConsumption;
      jetpackData.PropellantConsumption = this.PropellantConsumption;
      double maxHealth = (double) this.MaxHealth;
      jetpackData.MaxHealth = (float) maxHealth;
      double health = (double) this.Health;
      jetpackData.Health = (float) health;
      return jetpackData;
    }
  }
}
