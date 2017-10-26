// Decompiled with JetBrains decompiler
// Type: ZeroGravity.PersistenceObjectDataDynamicObject
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;

namespace ZeroGravity
{
  public class PersistenceObjectDataDynamicObject : PersistenceObjectData
  {
    public short ItemID;
    public float[] LocalPosition;
    public float[] LocalRotation;
    public double[] Velocity;
    public double[] AngularVelocity;
    public float? RespawnTime;
    public float? MaxHealth;
    public float? MinHealth;
    public float? WearMultiplier;
    public float[] RespawnPosition;
    public float[] RespawnForward;
    public float[] RespawnUp;
    public IDynamicObjectAuxData RespawnAuxData;
    public List<PersistenceObjectData> ChildObjects;

    public override Persistence.ObjectType Type
    {
      get
      {
        return Persistence.ObjectType.DynamicObject;
      }
    }
  }
}
