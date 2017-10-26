// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.BaseAttachPointData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;

namespace ZeroGravity.Data
{
  public abstract class BaseAttachPointData : ISceneData
  {
    public int InSceneID;
    public List<ItemType> ItemTypes;
    public List<GenericItemSubType> GenericSubTypes;
    public List<MachineryPartType> MachineryPartTypes;

    public abstract AttachPointType AttachPointType { get; }
  }
}
