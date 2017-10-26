// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.AsteroidSceneData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

namespace ZeroGravity.Data
{
  public class AsteroidSceneData : ISceneData
  {
    public short ItemID;
    public string ScenePath;
    public string SceneName;
    public string GameName;
    public int BoundingRes;
    public float Radius;
    public float[] BoundingBoxSize;
    public double RadarSignature;
    public string Collision;
    public ServerCollisionData Colliders;
  }
}
