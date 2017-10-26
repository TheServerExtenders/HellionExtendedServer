// Decompiled with JetBrains decompiler
// Type: ZeroGravity.ShipComponents.PDUGroup
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;

namespace ZeroGravity.ShipComponents
{
  public class PDUGroup : IComparable<PDUGroup>
  {
    public int Priority = -1;
    public List<VesselComponent> PowerConsumers;

    public int CompareTo(PDUGroup other)
    {
      return this.Priority.CompareTo(other.Priority);
    }
  }
}
