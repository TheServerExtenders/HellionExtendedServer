// Decompiled with JetBrains decompiler
// Type: ZeroGravity.PersistenceObjectDataAsteroid
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ZeroGravity.Data;
using ZeroGravity.Network;

namespace ZeroGravity
{
  public class PersistenceObjectDataAsteroid : PersistenceObjectData
  {
    public OrbitData OrbitData;
    public string Name;
    public GameScenes.SceneID SceneID;
    public double[] Forward;
    public double[] Up;

    public override Persistence.ObjectType Type
    {
      get
      {
        return Persistence.ObjectType.Asteroid;
      }
    }
  }
}
