// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.GameScenes
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System.Collections.Generic;

namespace ZeroGravity.Data
{
  public static class GameScenes
  {
    public static Dictionary<string, GameScenes.SceneID> Scenes = new Dictionary<string, GameScenes.SceneID>()
    {
      {
        "Assets/Scene/SolarSystemSetup.unity",
        GameScenes.SceneID.SolarSystemSetup
      },
      {
        "Assets/Scene/ItemScene.unity",
        GameScenes.SceneID.ItemScene
      },
      {
        "Assets/Scene/Environment/Station/Module/AltCorp/AltCorp_CorridorModule/AltCorp_CorridorModule.unity",
        GameScenes.SceneID.AltCorp_CorridorModule
      },
      {
        "Assets/Scene/Environment/Station/Module/AltCorp/AltCorp_CorridorIntersectionModule/AltCorp_CorridorIntersectionModule.unity",
        GameScenes.SceneID.AltCorp_CorridorIntersectionModule
      },
      {
        "Assets/Scene/Environment/Station/Module/AltCorp/Altcorp_Corridor45TurnModule/AltCorp_Corridor45TurnModule.unity",
        GameScenes.SceneID.AltCorp_Corridor45TurnModule
      },
      {
        "Assets/Scene/Environment/Ship/AltCorp_Shuttle_SARA/AltCorp_Shuttle_SARA.unity",
        GameScenes.SceneID.AltCorp_Shuttle_SARA
      },
      {
        "Assets/Scene/Environment/Station/Module/AltCorp/ALtCorp_PowerSupply_Module/ALtCorp_PowerSupply_Module.unity",
        GameScenes.SceneID.ALtCorp_PowerSupply_Module
      },
      {
        "Assets/Scene/Environment/Station/Module/AltCorp/AltCorp_LifeSupportModule/AltCorp_LifeSupportModule.unity",
        GameScenes.SceneID.AltCorp_LifeSupportModule
      },
      {
        "Assets/Scene/Environment/Station/Module/AltCorp/AltCorp_Cargo_Module/AltCorp_Cargo_Module.unity",
        GameScenes.SceneID.AltCorp_Cargo_Module
      },
      {
        "Assets/Scene/Environment/Station/Module/AltCorp/AltCorp_CorridorVertical/AltCorp_CorridorVertical.unity",
        GameScenes.SceneID.AltCorp_CorridorVertical
      },
      {
        "Assets/Scene/Environment/Station/Module/AltCorp/AltCorp_Command_Module/AltCorp_Command_Module.unity",
        GameScenes.SceneID.AltCorp_Command_Module
      },
      {
        "Assets/Scene/Environment/Station/Module/AltCorp/AltCorp_Corridor45TurnRightModule/AltCorp_Corridor45TurnRightModule.unity",
        GameScenes.SceneID.AltCorp_Corridor45TurnRightModule
      },
      {
        "Assets/Scene/Environment/Station/Module/AltCorp/AltCorp_StartingModule/AltCorp_StartingModule.unity",
        GameScenes.SceneID.AltCorp_StartingModule
      },
      {
        "Assets/Scene/Environment/Station/Module/Generic/Generic_Debris_Module_JuncRoom001/Generic_Debris_Module_JuncRoom001.unity",
        GameScenes.SceneID.Generic_Debris_JuncRoom001
      },
      {
        "Assets/Scene/Environment/Station/Module/Generic/Generic_Debris_Module_JuncRoom002/Generic_Debris_Module_JuncRoom002.unity",
        GameScenes.SceneID.Generic_Debris_JuncRoom002
      },
      {
        "Assets/Scene/Environment/Station/Module/Generic/Generic_Debris_Module_Corridor001/Generic_Debris_Module_Corridor001.unity",
        GameScenes.SceneID.Generic_Debris_Corridor001
      },
      {
        "Assets/Scene/Environment/Station/Module/Generic/Generic_Debris_Module_Corridor002/Generic_Debris_Module_Corridor002.unity",
        GameScenes.SceneID.Generic_Debris_Corridor002
      },
      {
        "Assets/Scene/Environment/Station/Module/AltCorp/AltCorp_Airlock_Module/AltCorp_Airlock_Module.unity",
        GameScenes.SceneID.AltCorp_AirLock
      },
      {
        "Assets/Scene/Environment/Station/Module/AltCorp/AltCorp_DockableContainer/AltCorp_DockableContainer.unity",
        GameScenes.SceneID.AltCorp_DockableContainer
      },
      {
        "Assets/Scene/Environment/Station/Module/Generic/Generic_Debris_Outpost001/Generic_Debris_Outpost001.unity",
        GameScenes.SceneID.Generic_Debris_Outpost001
      },
      {
        "Assets/Scene/Environment/Station/Module/AltCorp/AltCorp_CrewQuarters_Module/AltCorp_CrewQuarters_Module.unity",
        GameScenes.SceneID.AltCorp_CrewQuarters_Module
      },
      {
        "Assets/Test/Mata_Local/Prefabs01.unity",
        GameScenes.SceneID.MataPrefabs
      },
      {
        "Assets/Scene/Environment/SolarSystem/Asteroids/Asteroid_A002/Asteroid_A002.unity",
        GameScenes.SceneID.Asteroid01
      },
      {
        "Assets/Scene/Environment/SolarSystem/Asteroids/Asteroid_A003/Asteroid_A003.unity",
        GameScenes.SceneID.Asteroid02
      },
      {
        "Assets/Scene/Environment/SolarSystem/Asteroids/Asteroid_A005/Asteroid_A005.unity",
        GameScenes.SceneID.Asteroid03
      },
      {
        "Assets/Scene/Environment/SolarSystem/Asteroids/Asteroid_A006/Asteroid_A006.unity",
        GameScenes.SceneID.Asteroid04
      },
      {
        "Assets/Scene/Environment/SolarSystem/Asteroids/Asteroid_A008/Asteroid_A008.unity",
        GameScenes.SceneID.Asteroid05
      },
      {
        "Assets/Scene/Environment/SolarSystem/Asteroids/Asteroid_A009/Asteroid_A009.unity",
        GameScenes.SceneID.Asteroid06
      },
      {
        "Assets/Scene/Environment/SolarSystem/Asteroids/Asteroid_A011/Asteroid_A011.unity",
        GameScenes.SceneID.Asteroid07
      },
      {
        "Assets/Scene/Environment/SolarSystem/Asteroids/Asteroid_A017/Asteroid_A017.unity",
        GameScenes.SceneID.Asteroid08
      }
    };
    private static Dictionary<GameScenes.SceneID, string> vesselClassNames = new Dictionary<GameScenes.SceneID, string>()
    {
      {
        GameScenes.SceneID.AltCorp_Shuttle_SARA,
        "Arges MkII"
      },
      {
        GameScenes.SceneID.AltCorp_CorridorModule,
        "Corridor I"
      },
      {
        GameScenes.SceneID.AltCorp_CorridorIntersectionModule,
        "Corridor T"
      },
      {
        GameScenes.SceneID.AltCorp_Corridor45TurnModule,
        "Corridor L"
      },
      {
        GameScenes.SceneID.AltCorp_Corridor45TurnRightModule,
        "Corridor L"
      },
      {
        GameScenes.SceneID.AltCorp_CorridorVertical,
        "Corridor S"
      },
      {
        GameScenes.SceneID.ALtCorp_PowerSupply_Module,
        "Power Supply Module"
      },
      {
        GameScenes.SceneID.AltCorp_LifeSupportModule,
        "Life Support Module"
      },
      {
        GameScenes.SceneID.AltCorp_Cargo_Module,
        "Cargo Module"
      },
      {
        GameScenes.SceneID.AltCorp_Command_Module,
        "Command Module"
      },
      {
        GameScenes.SceneID.AltCorp_StartingModule,
        "Hibernation Module"
      },
      {
        GameScenes.SceneID.AltCorp_AirLock,
        "Airlock Module"
      },
      {
        GameScenes.SceneID.AltCorp_DockableContainer,
        "Industrial Container"
      },
      {
        GameScenes.SceneID.AltCorp_CrewQuarters_Module,
        "Crew Quarters Module"
      }
    };

    public static string GetVesselClassName(GameScenes.SceneID sceneID)
    {
      try
      {
        return GameScenes.vesselClassNames[sceneID];
      }
      catch
      {
        return "";
      }
    }

    public enum SceneID
    {
      SolarSystemSetup = -3,
      ItemScene = -2,
      None = -1,
      Slavica = 1,
      AltCorp_Ship_Tamara = 2,
      AltCorp_CorridorModule = 3,
      AltCorp_CorridorIntersectionModule = 4,
      AltCorp_Corridor45TurnModule = 5,
      AltCorp_Shuttle_SARA = 6,
      ALtCorp_PowerSupply_Module = 7,
      AltCorp_LifeSupportModule = 8,
      AltCorp_Cargo_Module = 9,
      AltCorp_CorridorVertical = 10,
      AltCorp_Command_Module = 11,
      AltCorp_Corridor45TurnRightModule = 12,
      AltCorp_StartingModule = 13,
      AltCorp_AirLock = 14,
      Generic_Debris_JuncRoom001 = 15,
      Generic_Debris_JuncRoom002 = 16,
      Generic_Debris_Corridor001 = 17,
      Generic_Debris_Corridor002 = 18,
      AltCorp_DockableContainer = 19,
      MataPrefabs = 20,
      Generic_Debris_Outpost001 = 21,
      AltCorp_CrewQuarters_Module = 22,
      Asteroid01 = 1000,
      Asteroid02 = 1001,
      Asteroid03 = 1002,
      Asteroid04 = 1003,
      Asteroid05 = 1004,
      Asteroid06 = 1005,
      Asteroid07 = 1006,
      Asteroid08 = 1007,
    }

    public static class Ranges
    {
      public const int DebrisFrom = 15;
      public const int DebrisTo = 18;
      public const int AsteroidsFrom = 1000;
      public const int AsteroidsTo = 1007;
      public const int CelestialBodiesFrom = 1;
      public const int CelestialBodiesTo = 19;
      public const int DeathMatchArenaFrom = 3;
      public const int DeathMatchArenaTo = 6;
      public const int RefiningStationsFrom = 6;
      public const int RefiningStationsTo = 9;

      public static bool IsDerelict(GameScenes.SceneID sceneId)
      {
        return sceneId == GameScenes.SceneID.Generic_Debris_JuncRoom001 || sceneId == GameScenes.SceneID.Generic_Debris_JuncRoom002 || sceneId == GameScenes.SceneID.Generic_Debris_Corridor001 || sceneId == GameScenes.SceneID.Generic_Debris_Corridor002;
      }

      public static bool IsShip(GameScenes.SceneID sceneId)
      {
        return sceneId == GameScenes.SceneID.AltCorp_Shuttle_SARA || sceneId == GameScenes.SceneID.AltCorp_Ship_Tamara || sceneId == GameScenes.SceneID.Slavica;
      }

      public static bool IsAsteroid(GameScenes.SceneID sceneId)
      {
        return sceneId >= GameScenes.SceneID.Asteroid01;
      }

      public static bool IsModule(GameScenes.SceneID sceneId)
      {
        return !GameScenes.Ranges.IsShip(sceneId) && !GameScenes.Ranges.IsDerelict(sceneId) && !GameScenes.Ranges.IsAsteroid(sceneId);
      }
    }
  }
}
