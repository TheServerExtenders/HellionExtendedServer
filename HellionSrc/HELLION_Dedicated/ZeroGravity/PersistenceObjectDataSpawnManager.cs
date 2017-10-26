// Decompiled with JetBrains decompiler
// Type: ZeroGravity.PersistenceObjectDataSpawnManager
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;

namespace ZeroGravity
{
  public class PersistenceObjectDataSpawnManager : PersistenceObjectData
  {
    public Dictionary<string, PersistenceObjectDataSpawnManager.SpawnRule> SpawnRules;

    public override Persistence.ObjectType Type
    {
      get
      {
        return Persistence.ObjectType.SpawnManager;
      }
    }

    public class SpawnRule
    {
      public double CurrTimerSec;
      public List<PersistenceObjectDataSpawnManager.SpawnRuleScene> ScenePool;
      public List<PersistenceObjectDataSpawnManager.SpawnRuleLoot> LootPool;
      public List<long> SpawnedVessels;
    }

    public class SpawnRuleScene
    {
      public List<Tuple<long, int>> Vessels;
    }

    public class SpawnRuleLoot
    {
      public List<long> DynamicObjects;
    }
  }
}
