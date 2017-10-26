// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.Item
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Network;
using ZeroGravity.ShipComponents;

namespace ZeroGravity.Objects
{
  public abstract class Item : IPersistantObject, IDamageable
  {
    private AttachPointType _attachPointType = AttachPointType.None;
    private float _MaxHealth = 100f;
    private float _Health = 100f;
    private float _Armor = 1f;
    public ItemType Type;
    public float MeleeDamage;

    public InventorySlot Slot { get; protected set; }

    public abstract DynamicObjectStats StatsNew { get; }

    public VesselObjectID AttachPointKey { get; private set; }

    public AttachPointType AttachPointType
    {
      get
      {
        return this._attachPointType;
      }
      private set
      {
        this._attachPointType = value;
      }
    }

    public long GUID
    {
      get
      {
        return this.DynamicObj.GUID;
      }
    }

    public DynamicObject DynamicObj { get; private set; }

    public float MaxHealth
    {
      get
      {
        return this._MaxHealth;
      }
      set
      {
        this._MaxHealth = (double) value < 0.0 ? 0.0f : value;
      }
    }

    public float Health
    {
      get
      {
        return this._Health;
      }
      set
      {
        this._Health = (double) value > (double) this.MaxHealth ? this.MaxHealth : ((double) value < 0.0 ? 0.0f : value);
      }
    }

    public float Armor
    {
      get
      {
        return this._Armor;
      }
      set
      {
        this._Armor = (double) value < 0.0 ? 0.0f : value;
      }
    }

    public abstract bool ChangeStats(DynamicObjectStats stats);

    public virtual void SetSlot(InventorySlot slot)
    {
      if (this.Slot != null && this.Slot.SlotItem == this)
        this.Slot.SlotItem = (Item) null;
      this.Slot = slot;
      if (slot != null)
        this.Slot.SlotItem = this;
      if (slot != null)
      {
        this.DynamicObj.Parent = slot.GetParent();
        this.ChangeEquip(slot.GetEquipType());
      }
      else
        this.ChangeEquip(Inventory.EquipType.None);
    }

    public virtual void SetAttachPoint(AttachPointDetails data)
    {
      if (data == null || data.InSceneID <= 0)
      {
        if (this.AttachPointKey != null)
        {
          SpaceObjectVessel vessel = Server.Instance.GetVessel(this.AttachPointKey.VesselGUID);
          if (vessel != null && vessel.AttachPoints.ContainsKey(this.AttachPointKey.InSceneID))
            vessel.AttachPoints[this.AttachPointKey.InSceneID].Item = (Item) null;
        }
        this.AttachPointKey = (VesselObjectID) null;
        this.AttachPointType = AttachPointType.None;
      }
      else
      {
        if (!(this.DynamicObj.Parent is SpaceObjectVessel))
          return;
        this.AttachPointKey = new VesselObjectID(this.DynamicObj.Parent.GUID, data.InSceneID);
        AttachPointType attachPointType = AttachPointType.None;
        (this.DynamicObj.Parent as SpaceObjectVessel).AttachPointsTypes.TryGetValue(this.AttachPointKey, out attachPointType);
        this.AttachPointType = attachPointType;
        if (attachPointType == AttachPointType.ResourcesAutoTransferPoint && this is Canister && this.DynamicObj.Parent is Ship)
          this.AutoTransferResources();
        if ((this.DynamicObj.Parent as SpaceObjectVessel).AttachPoints.ContainsKey(this.AttachPointKey.InSceneID))
          (this.DynamicObj.Parent as SpaceObjectVessel).AttachPoints[this.AttachPointKey.InSceneID].Item = this;
      }
    }

    private void AutoTransferResources()
    {
      ICargo fromCargo = this as ICargo;
      CargoCompartmentData compartment1 = fromCargo.GetCompartment(new int?());
      foreach (ResourceContainer resourceContainer in (this.DynamicObj.Parent as Ship).DistributionManager.GetResourceContainers())
      {
        foreach (CargoCompartmentData compartment2 in resourceContainer.Compartments)
        {
          if (compartment2.Type == CargoCompartmentType.RCS || compartment2.Type == CargoCompartmentType.AirGeneratorOxygen || (compartment2.Type == CargoCompartmentType.AirGeneratorNitrogen || compartment2.Type == CargoCompartmentType.PowerGenerator) || compartment2.Type == CargoCompartmentType.Engine)
          {
            foreach (CargoResourceData cargoResourceData in new List<CargoResourceData>((IEnumerable<CargoResourceData>) compartment1.Resources))
              Server.Instance.TransferResources(fromCargo, compartment1.ID, (ICargo) resourceContainer, compartment2.ID, cargoResourceData.ResourceType, cargoResourceData.Quantity);
          }
        }
      }
    }

    public virtual bool AttachChildItem(Item item)
    {
      return false;
    }

    public virtual bool RemoveChildItem(Item item)
    {
      return false;
    }

    public virtual bool CanAttachChildItem(Item item)
    {
      return false;
    }

    public virtual void Use()
    {
    }

    public static Item Create(DynamicObject dobj, ItemType type, IDynamicObjectAuxData data)
    {
      Item obj = (Item) null;
      if (ItemTypeRange.IsHelmet(type))
        obj = (Item) new Helmet(data);
      else if (ItemTypeRange.IsJetpack(type))
        obj = (Item) new Jetpack(data);
      else if (ItemTypeRange.IsWeapon(type))
        obj = (Item) new Weapon(data);
      else if (ItemTypeRange.IsOutfit(type))
        obj = (Item) new Outfit(data);
      else if (ItemTypeRange.IsAmmo(type))
        obj = (Item) new Magazine(data);
      else if (ItemTypeRange.IsMachineryPart(type))
        obj = (Item) new MachineryPart(data);
      else if (ItemTypeRange.IsBattery(type))
        obj = (Item) new Battery(data);
      else if (ItemTypeRange.IsCanister(type))
        obj = (Item) new Canister(data);
      else if (ItemTypeRange.IsDrill(type))
        obj = (Item) new HandDrill(data);
      else if (ItemTypeRange.IsMelee(type))
        obj = (Item) new MeleeWeapon(data);
      else if (ItemTypeRange.IsGlowStick(type))
        obj = (Item) new GlowStick(data);
      else if (ItemTypeRange.IsMedpack(type))
        obj = (Item) new Medpack(data);
      else if (ItemTypeRange.IsHackingTool(type))
        obj = (Item) new DisposableHackingTool(data);
      else if (ItemTypeRange.IsAsteroidScanningTool(type))
        obj = (Item) new HandheldAsteroidScanner(data);
      else if (ItemTypeRange.IsLogItem(type))
        obj = (Item) new LogItem(data);
      else if (ItemTypeRange.IsGenericItem(type))
        obj = (Item) new GenericItem(data);
      else if (ItemTypeRange.IsGrenade(type))
        obj = (Item) new Grenade(data);
      else if (ItemTypeRange.IsPortableTurret(type))
        obj = (Item) new PortableTurret(data);
      else if (ItemTypeRange.IsRepairTool(type))
        obj = (Item) new RepairTool(data);
      if (obj != null)
      {
        obj.Type = type;
        obj.DynamicObj = dobj;
      }
      return obj;
    }

    protected virtual void ChangeEquip(Inventory.EquipType equipType)
    {
    }

    public virtual void SendAllStats()
    {
    }

    public void FillPersistenceData(PersistenceObjectDataItem data)
    {
      this.DynamicObj.FillPersistenceData((PersistenceObjectDataDynamicObject) data);
      data.GUID = this.GUID;
      if ((uint) this.AttachPointType > 0U)
      {
        data.AttachPointType = this.AttachPointType;
        data.AttachPointID = new int?(this.AttachPointKey.InSceneID);
      }
      if (this.Slot == null)
        return;
      data.SlotID = new short?(this.Slot.SlotID);
    }

    public virtual PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataItem data = new PersistenceObjectDataItem();
      this.FillPersistenceData(data);
      return (PersistenceObjectData) data;
    }

    public virtual void DestroyItem()
    {
      this.SetSlot((InventorySlot) null);
      this.SetAttachPoint((AttachPointDetails) null);
      this.DynamicObj.DestroyDynamicObject();
    }

    public virtual void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        PersistenceObjectDataItem persistenceObjectDataItem = persistenceData as PersistenceObjectDataItem;
        this.DynamicObj.LoadPersistenceData((PersistenceObjectData) persistenceObjectDataItem);
        AttachPointDetails data = (AttachPointDetails) null;
        if (persistenceObjectDataItem.AttachPointID.HasValue && persistenceObjectDataItem.AttachPointID.Value > 0)
        {
          data = new AttachPointDetails()
          {
            InSceneID = persistenceObjectDataItem.AttachPointID.Value
          };
          try
          {
            this.SetAttachPoint(data);
          }
          catch
          {
          }
        }
        this.DynamicObj.APDetails = data;
        if (this.DynamicObj.Parent is DynamicObject && (this.DynamicObj.Parent as DynamicObject).Item != null)
        {
          DynamicObject parent = this.DynamicObj.Parent as DynamicObject;
          parent.Item.AttachChildItem(this);
          if (persistenceObjectDataItem.SlotID.HasValue && parent.Item is Outfit && (parent.Item as Outfit).Slots.ContainsKey(persistenceObjectDataItem.SlotID.Value))
            this.SetSlot((parent.Item as Outfit).Slots[persistenceObjectDataItem.SlotID.Value]);
        }
        if (!(this.DynamicObj.Parent is Player) || !persistenceObjectDataItem.SlotID.HasValue)
          return;
        try
        {
          (this.DynamicObj.Parent as Player).PlayerInventory.AddItemToInventory(this, persistenceObjectDataItem.SlotID.Value);
        }
        catch (Exception ex)
        {
          Dbg.Exception(ex);
        }
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }

    public virtual void TakeDamage(Dictionary<TypeOfDamage, float> damages)
    {
      float num = 0.0f;
      foreach (KeyValuePair<TypeOfDamage, float> damage in damages)
        num += damage.Value;
      this.Health = this.Health - num;
      if (this.StatsNew != null)
      {
        this.StatsNew.Health = new float?(this.Health);
        this.StatsNew.Damages = damages;
      }
      this.DynamicObj.SendStatsToClient();
    }
  }
}
