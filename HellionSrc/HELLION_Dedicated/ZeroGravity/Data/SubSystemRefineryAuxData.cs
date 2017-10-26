// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.SubSystemRefineryAuxData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;

namespace ZeroGravity.Data
{
  public class SubSystemRefineryAuxData : SubSystemAuxData
  {
    public int CargoBayInSceneID;
    public float Capacity;
    public float ProcessingTime;
    public List<RefinedResourcesData> Resources;

    public override SubSystemAuxDataType AuxDataType
    {
      get
      {
        return SubSystemAuxDataType.Refinery;
      }
    }
  }
}
