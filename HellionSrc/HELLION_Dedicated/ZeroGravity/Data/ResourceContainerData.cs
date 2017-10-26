// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.ResourceContainerData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ZeroGravity.ShipComponents;

namespace ZeroGravity.Data
{
  public class ResourceContainerData : ISceneData
  {
    public int InSceneID;
    public DistributionSystemType DistributionSystemType;
    public CargoCompartmentData CargoCompartment;
    public float Output;
    public float NominalOutput;
    public bool IsInUse;
  }
}
