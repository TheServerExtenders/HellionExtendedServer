// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.Outfit
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Network;

namespace ZeroGravity.Objects
{
  public class Outfit : Item
  {
    public float DamageReductionTorso = 0.0f;
    public float DamageReductionAbdomen = 0.0f;
    public float DamageReductionArms = 0.0f;
    public float DamageReductionLegs = 0.0f;
    public float DamageResistanceTorso = 1f;
    public float DamageResistanceAbdomen = 1f;
    public float DamageResistanceArms = 1f;
    public float DamageResistanceLegs = 1f;
    public float CollisionResistance = 1f;
    public float InternalTemperature;
    public float ExternalTemperature;

    public override DynamicObjectStats StatsNew
    {
      get
      {
        return (DynamicObjectStats) null;
      }
    }

    public Dictionary<short, InventorySlot> Slots { get; private set; }

    public Outfit()
    {
    }

    public Outfit(IDynamicObjectAuxData data)
    {
      if (data == null)
        return;
      this.SetData(data as OutfitData);
    }

    private void SetData(OutfitData od)
    {
      this.Slots = new Dictionary<short, InventorySlot>();
      foreach (InventorySlotData slot in od.Slots)
        this.Slots.Add(slot.SlotID, new InventorySlot(slot.SlotType, slot.SlotID, slot.ItemTypes, slot.MustBeEmptyToRemoveOutfit, this, (Inventory) null));
      this.DamageReductionTorso = od.DamageReductionTorso;
      this.DamageReductionAbdomen = od.DamageReductionAbdomen;
      this.DamageReductionArms = od.DamageReductionArms;
      this.DamageReductionLegs = od.DamageReductionLegs;
      this.DamageResistanceTorso = od.DamageResistanceTorso;
      this.DamageResistanceAbdomen = od.DamageResistanceAbdomen;
      this.DamageResistanceArms = od.DamageResistanceArms;
      this.DamageResistanceLegs = od.DamageResistanceLegs;
      this.CollisionResistance = od.CollisionResistance;
      this.Health = od.Health;
      this.MaxHealth = od.MaxHealth;
    }

    public override bool ChangeStats(DynamicObjectStats stats)
    {
      return false;
    }

    public override PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataOutfit objectDataOutfit = new PersistenceObjectDataOutfit();
      this.FillPersistenceData((PersistenceObjectDataItem) objectDataOutfit);
      objectDataOutfit.OutfitData = new OutfitData();
      objectDataOutfit.OutfitData.Slots = new List<InventorySlotData>();
      foreach (KeyValuePair<short, InventorySlot> slot in this.Slots)
      {
        List<InventorySlotData> slots = objectDataOutfit.OutfitData.Slots;
        InventorySlotData inventorySlotData = new InventorySlotData();
        inventorySlotData.SlotID = slot.Value.SlotID;
        inventorySlotData.SlotType = slot.Value.SlotType;
        inventorySlotData.ItemTypes = slot.Value.ItemTypes;
        int num = slot.Value.MustBeEmptyToRemoveOutfit ? 1 : 0;
        inventorySlotData.MustBeEmptyToRemoveOutfit = num != 0;
        slots.Add(inventorySlotData);
      }
      objectDataOutfit.OutfitData.ItemType = this.Type;
      objectDataOutfit.OutfitData.DamageReductionTorso = this.DamageReductionTorso;
      objectDataOutfit.OutfitData.DamageReductionAbdomen = this.DamageReductionAbdomen;
      objectDataOutfit.OutfitData.DamageReductionArms = this.DamageReductionArms;
      objectDataOutfit.OutfitData.DamageReductionLegs = this.DamageReductionLegs;
      objectDataOutfit.OutfitData.DamageResistanceTorso = this.DamageResistanceTorso;
      objectDataOutfit.OutfitData.DamageResistanceAbdomen = this.DamageResistanceAbdomen;
      objectDataOutfit.OutfitData.DamageResistanceArms = this.DamageResistanceArms;
      objectDataOutfit.OutfitData.DamageResistanceLegs = this.DamageResistanceLegs;
      objectDataOutfit.OutfitData.CollisionResistance = this.CollisionResistance;
      objectDataOutfit.OutfitData.Health = this.Health;
      objectDataOutfit.OutfitData.MaxHealth = this.MaxHealth;
      return (PersistenceObjectData) objectDataOutfit;
    }

    public override void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      try
      {
        PersistenceObjectDataOutfit objectDataOutfit = persistenceData as PersistenceObjectDataOutfit;
        if (objectDataOutfit == null)
        {
          Dbg.Warning((object) "PersistenceObjectDataOutfit data is null", (object) this.GUID);
        }
        else
        {
          this.SetData(objectDataOutfit.OutfitData);
          base.LoadPersistenceData(persistenceData);
        }
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
    }
  }
}
