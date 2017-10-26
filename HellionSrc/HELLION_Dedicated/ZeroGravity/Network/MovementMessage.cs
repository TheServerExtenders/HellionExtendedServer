// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.MovementMessage
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using System.Collections.Generic;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class MovementMessage : NetworkData
  {
    public double SolarSystemTime;
    public float Timestamp;
    public List<ObjectTransform> Transforms;
  }
}
