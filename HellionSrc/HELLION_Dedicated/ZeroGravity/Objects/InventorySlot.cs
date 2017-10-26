// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.InventorySlot
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;

namespace ZeroGravity.Objects
{
  public class InventorySlot
  {
    public const short NoneSlotID = -1111;
    public const short HandsSlotID = -1;
    public const short StartSlotID = 1;
    public const short OutfitSlotID = -2;
    private Inventory inventory;

    public Outfit Outfit { get; private set; }

    public InventorySlot.Type SlotType { get; private set; }

    public short SlotID { get; private set; }

    public List<ItemType> ItemTypes { get; private set; }

    public bool MustBeEmptyToRemoveOutfit { get; private set; }

    public Item SlotItem { get; set; }

    public InventorySlot(InventorySlot.Type slotType, short slotID, List<ItemType> itemTypes, bool mustBeEmptyToRemoveOutfit, Outfit outfit, Inventory inventory)
    {
      this.SlotType = slotType;
      this.SlotID = slotID;
      this.MustBeEmptyToRemoveOutfit = mustBeEmptyToRemoveOutfit;
      if (itemTypes != null)
        this.ItemTypes = new List<ItemType>((IEnumerable<ItemType>) itemTypes);
      this.Outfit = outfit;
      this.inventory = inventory;
    }

    public void SetInventory(Inventory inv)
    {
      this.inventory = inv;
    }

    public bool CanStoreItem(Item item)
    {
      return this.SlotType == InventorySlot.Type.Hands || this.ItemTypes.Contains(item.Type);
    }

    public SpaceObject GetParent()
    {
      if (this.inventory != null)
        return this.inventory.Parent;
      if (this.Outfit != null)
        return (SpaceObject) this.Outfit.DynamicObj;
      Dbg.Error((object) "Slot has no parent", (object) this.SlotID, (object) this.SlotType);
      return (SpaceObject) null;
    }

    public Inventory.EquipType GetEquipType()
    {
      if (this.SlotType == InventorySlot.Type.Hands)
        return Inventory.EquipType.Hands;
      if (this.SlotType == InventorySlot.Type.Equip)
        return Inventory.EquipType.EquipInventory;
      return this.SlotType == InventorySlot.Type.General ? Inventory.EquipType.Inventory : Inventory.EquipType.None;
    }

    public enum Type
    {
      Hands,
      General,
      Equip,
    }
  }
}
