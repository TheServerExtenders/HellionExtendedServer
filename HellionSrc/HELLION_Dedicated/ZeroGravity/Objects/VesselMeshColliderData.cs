// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Objects.VesselMeshColliderData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using BulletSharp;
using ZeroGravity.Math;

namespace ZeroGravity.Objects
{
  public class VesselMeshColliderData : VesselColliderData
  {
    public Vector3[] Vertices;
    public int[] Indices;
    public QuaternionD Rotation;
    public Vector3D Scale;
  }
}
