// Decompiled with JetBrains decompiler
// Type: ZeroGravity.PersistenceObjectDataRespawnObject
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ZeroGravity.Data;
using ZeroGravity.Objects;

namespace ZeroGravity
{
  public class PersistenceObjectDataRespawnObject : PersistenceObjectData
  {
    public short ItemID;
    public long ParentGUID;
    public SpaceObjectType ParentType;
    public float[] Position;
    public float[] Forward;
    public float[] Up;
    public int? AttachPointID;
    public IDynamicObjectAuxData AuxData;
    public float RespawnTime;
    public double Timer;

    public override Persistence.ObjectType Type
    {
      get
      {
        return Persistence.ObjectType.RespawnObject;
      }
    }
  }
}
