// Decompiled with JetBrains decompiler
// Type: ZeroGravity.PersistenceObjectDataVesselComponent
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ZeroGravity.Network;

namespace ZeroGravity
{
  public class PersistenceObjectDataVesselComponent : PersistenceObjectData
  {
    public int InSceneID;
    public SystemStatus Status;
    public float StatusChangeCountdown;
    public bool AutoReactivate;
    public bool Defective;

    public override Persistence.ObjectType Type
    {
      get
      {
        return Persistence.ObjectType.VesselComponent;
      }
    }
  }
}
