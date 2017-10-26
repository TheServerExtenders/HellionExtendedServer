// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.Inventory
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;

namespace ZeroGravity.Objects
{
  public class Inventory
  {
    private Player parentPlayer;
    private Corpse parentCorpse;

    public Outfit CurrOutfit { get; private set; }

    public InventorySlot HandsSlot { get; private set; }

    public InventorySlot OutfitSlot { get; private set; }

    public SpaceObject Parent
    {
      get
      {
        if (this.parentPlayer != null)
          return (SpaceObject) this.parentPlayer;
        return (SpaceObject) this.parentCorpse;
      }
    }

    public Inventory(Player pl)
    {
      this.parentPlayer = pl;
      this.parentCorpse = (Corpse) null;
      this.HandsSlot = new InventorySlot(InventorySlot.Type.Hands, (short) -1, (List<ItemType>) null, true, (Outfit) null, this);
      this.OutfitSlot = new InventorySlot(InventorySlot.Type.Equip, (short) -2, (List<ItemType>) null, false, (Outfit) null, this);
    }

    public void ChangeParent(Corpse corpse)
    {
      this.parentPlayer = (Player) null;
      this.parentCorpse = corpse;
      if (this.HandsSlot.SlotItem != null)
        this.HandsSlot.SlotItem.DynamicObj.Parent = (SpaceObject) corpse;
      if (this.OutfitSlot.SlotItem != null)
        this.OutfitSlot.SlotItem.DynamicObj.Parent = (SpaceObject) corpse;
      if (this.CurrOutfit == null)
        return;
      foreach (InventorySlot inventorySlot in this.CurrOutfit.Slots.Values)
      {
        if (inventorySlot.SlotItem != null)
          inventorySlot.SlotItem.DynamicObj.Parent = (SpaceObject) corpse;
      }
    }

    public Item GetHandsItemIfType<T>()
    {
      if (this.HandsSlot.SlotItem != null && typeof (T).IsAssignableFrom(this.HandsSlot.SlotItem.GetType()))
        return this.HandsSlot.SlotItem;
      return (Item) null;
    }

    private bool EquipOutfit(Outfit outfit)
    {
      if (this.CurrOutfit != null)
        return false;
      if (outfit.Slot != null && outfit.Slot.SlotItem == outfit)
        outfit.Slot.SlotItem = (Item) null;
      this.CurrOutfit = outfit;
      this.CurrOutfit.SetSlot(this.OutfitSlot);
      if (this.parentPlayer != null)
      {
        this.CurrOutfit.DynamicObj.Parent = (SpaceObject) this.parentPlayer;
        foreach (InventorySlot inventorySlot in this.CurrOutfit.Slots.Values)
        {
          if (inventorySlot.SlotItem != null)
            inventorySlot.SlotItem.DynamicObj.Parent = (SpaceObject) this.parentPlayer;
        }
        this.CurrOutfit.ExternalTemperature = this.parentPlayer.AmbientTemperature.HasValue ? this.parentPlayer.AmbientTemperature.Value : this.parentPlayer.CoreTemperature;
        this.CurrOutfit.InternalTemperature = this.parentPlayer.CoreTemperature;
      }
      foreach (InventorySlot inventorySlot in this.CurrOutfit.Slots.Values)
        inventorySlot.SetInventory(this);
      return true;
    }

    private bool TakeOffOutfit(short slotID)
    {
      if (this.CurrOutfit == null)
        return false;
      foreach (InventorySlot inventorySlot in this.CurrOutfit.Slots.Values)
      {
        if (inventorySlot.SlotItem != null)
          inventorySlot.SlotItem.DynamicObj.Parent = (SpaceObject) this.CurrOutfit.DynamicObj;
        inventorySlot.SetInventory((Inventory) null);
      }
      if ((int) slotID == -1)
      {
        this.CurrOutfit.SetSlot(this.HandsSlot);
      }
      else
      {
        if ((int) slotID != -1111)
          return false;
        this.CurrOutfit.SetSlot((InventorySlot) null);
      }
      this.OutfitSlot.SlotItem = (Item) null;
      this.CurrOutfit = (Outfit) null;
      return true;
    }

    public bool AddItemToInventory(Item item, short slotID)
    {
      item.DynamicObj.PickedUp();
      if (item is Outfit && (int) slotID == -2)
        return this.EquipOutfit(item as Outfit);
      if (item is Outfit && this.CurrOutfit == item && (int) slotID != -2)
        return this.TakeOffOutfit(slotID);
      InventorySlot slot = (InventorySlot) null;
      if ((int) slotID == -1)
        slot = this.HandsSlot;
      else if (this.CurrOutfit != null && this.CurrOutfit.Slots.ContainsKey(slotID))
        slot = this.CurrOutfit.Slots[slotID];
      if (slot == null || !slot.CanStoreItem(item))
        return false;
      if (slot.SlotItem != null && slot.SlotItem != item)
      {
        if (item.DynamicObj.Parent is DynamicObject && (item.DynamicObj.Parent as DynamicObject).Item != null)
        {
          Item obj = (item.DynamicObj.Parent as DynamicObject).Item;
          if (obj == slot.SlotItem || !obj.CanAttachChildItem(slot.SlotItem) || !obj.RemoveChildItem(item) || !obj.AttachChildItem(slot.SlotItem))
            return false;
          slot.SlotItem.DynamicObj.Parent = (SpaceObject) obj.DynamicObj;
        }
        else if (item.Slot == null || !item.Slot.CanStoreItem(slot.SlotItem))
          return false;
        slot.SlotItem.SetSlot(item.Slot);
      }
      item.SetSlot(slot);
      if (this.parentCorpse != null)
        this.parentCorpse.CheckInventoryDestroy();
      return true;
    }

    public bool DropItem(short slotID)
    {
      if ((int) slotID == -2)
        return this.TakeOffOutfit((short) -1111);
      InventorySlot inventorySlot = (InventorySlot) null;
      if ((int) slotID == -1)
        inventorySlot = this.HandsSlot;
      else if (this.CurrOutfit != null && this.CurrOutfit.Slots.ContainsKey(slotID))
        inventorySlot = this.CurrOutfit.Slots[slotID];
      if (inventorySlot == null)
        return false;
      inventorySlot.SlotItem.SetSlot((InventorySlot) null);
      inventorySlot.SlotItem = (Item) null;
      if (this.parentCorpse != null)
        this.parentCorpse.CheckInventoryDestroy();
      return true;
    }

    public enum EquipType
    {
      None,
      Hands,
      EquipInventory,
      Inventory,
    }
  }
}
