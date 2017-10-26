// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.IResourceProvider
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;

namespace ZeroGravity.ShipComponents
{
  public interface IResourceProvider
  {
    HashSet<IResourceConsumer> ConnectedConsumers { get; }

    float Output { get; set; }

    float NominalOutput { get; set; }

    float MaxOutput { get; }

    float OperationRate { get; set; }

    DistributionSystemType OutputType { get; }
  }
}
