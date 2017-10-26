// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.InventorySlotData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Objects;

namespace ZeroGravity.Data
{
  public class InventorySlotData
  {
    public short SlotID;
    public InventorySlot.Type SlotType;
    public List<ItemType> ItemTypes;
    public bool MustBeEmptyToRemoveOutfit;
  }
}
