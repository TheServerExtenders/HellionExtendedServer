// Decompiled with JetBrains decompiler
// Type: ZeroGravity.PersistenceObjectDataCargo
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;
using ZeroGravity.Data;

namespace ZeroGravity
{
  public class PersistenceObjectDataCargo : PersistenceObjectData
  {
    public int InSceneID;
    public List<CargoCompartmentData> CargoCompartments;

    public override Persistence.ObjectType Type
    {
      get
      {
        return Persistence.ObjectType.Cargo;
      }
    }
  }
}
