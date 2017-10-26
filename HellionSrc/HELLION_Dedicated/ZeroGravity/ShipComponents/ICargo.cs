// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.ICargo
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;

namespace ZeroGravity.ShipComponents
{
  public interface ICargo
  {
    List<CargoCompartmentData> Compartments { get; }

    CargoCompartmentData GetCompartment(int? id = null);

    float ChangeQuantityBy(int compartmentID, ResourceType resourceType, float quantity, bool wholeAmount = false);
  }
}
