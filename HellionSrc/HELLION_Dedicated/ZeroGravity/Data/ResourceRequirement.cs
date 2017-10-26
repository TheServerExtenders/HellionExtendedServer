// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.ResourceRequirement
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using ZeroGravity.ShipComponents;

namespace ZeroGravity.Data
{
  [Serializable]
  public class ResourceRequirement : ISceneData
  {
    public DistributionSystemType ResourceType;
    public float Nominal;
    public float Standby;
  }
}
