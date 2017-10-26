// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.MachineryPartSlotData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

namespace ZeroGravity.Data
{
  public class MachineryPartSlotData : BaseAttachPointData
  {
    public MachineryPartType SlotType;
    public int MinTier;
    public int MaxTier;
    public float BasicModifier;
    public float PartDecay;
    public int SlotIndex;

    public override AttachPointType AttachPointType
    {
      get
      {
        return AttachPointType.MachineryPartSlot;
      }
    }

    public bool IsActive
    {
      get
      {
        return true;
      }
      set
      {
      }
    }
  }
}
