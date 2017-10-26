// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.CourseItemData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using System.Collections.Generic;

namespace ZeroGravity.Network
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  public class CourseItemData
  {
    public float StartOrbitAngle = 0.0f;
    public float EndOrbitAngle = 0.0f;
    public double TravelTime = 0.0;
    public int ManeuverIndex = 0;
    public List<int> WarpCells = (List<int>) null;
    public OrbitData StartOrbit = (OrbitData) null;
    public OrbitData EndOrbit = (OrbitData) null;
    public long GUID;
    public ManeuverType Type;
  }
}
