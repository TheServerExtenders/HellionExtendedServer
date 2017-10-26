// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.OutfitData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;

namespace ZeroGravity.Data
{
  public class OutfitData : IDynamicObjectAuxData
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
    public List<InventorySlotData> Slots;
  }
}
