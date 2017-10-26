// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.MeshData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

namespace ZeroGravity.Data
{
  public class MeshData : ISceneData
  {
    public bool AffectingCenterOfMass = true;
    public float[] CenterPosition;
    public float[] Vertices;
    public int[] Indices;
    public float[] Bounds;
    public float[] Rotation;
    public float[] Scale;
  }
}
