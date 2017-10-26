// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Spawn.LootItemData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;

namespace ZeroGravity.Spawn
{
  public class LootItemData
  {
    public ItemType Type;
    public GenericItemSubType GenericSubType;
    public MachineryPartType PartType;
    public SpawnRange<float>? Health;
    public List<string> Look;
    public SpawnRange<float>? Power;
    public SpawnRange<int>? Count;
    public bool? IsActive;
    public List<LootItemData.CargoResourceData> Cargo;

    public int GetSubType()
    {
      if (this.Type == ItemType.GenericItem)
        return (int) this.GenericSubType;
      if (this.Type == ItemType.MachineryPart)
        return (int) this.PartType;
      return 0;
    }

    public class CargoResourceData
    {
      public List<ResourceType> Resources;
      public SpawnRange<float> Quantity;
    }
  }
}
