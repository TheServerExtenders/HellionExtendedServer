// Decompiled with JetBrains decompiler
// Type: ZeroGravity.StaticData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using System.IO;
using ZeroGravity.Data;

namespace ZeroGravity
{
  public static class StaticData
  {
    private static List<CelestialBodyData> _CelestialBodyDataList = (List<CelestialBodyData>) null;
    private static Dictionary<string, ServerCollisionData> _CollisionDataList = (Dictionary<string, ServerCollisionData>) null;
    private static List<StructureSceneData> _StructuresDataList = (List<StructureSceneData>) null;
    private static List<AsteroidSceneData> _AsteroidDataList = (List<AsteroidSceneData>) null;
    private static Dictionary<short, DynamicObjectData> _DynamicObjectsDataList = (Dictionary<short, DynamicObjectData>) null;
    private static Dictionary<ItemType, Dictionary<ResourceType, float>> _itemRecepiesList = (Dictionary<ItemType, Dictionary<ResourceType, float>>) null;

    public static List<CelestialBodyData> CelestialBodyDataList
    {
      get
      {
        if (StaticData._CelestialBodyDataList == null)
          StaticData.LoadData();
        return StaticData._CelestialBodyDataList;
      }
    }

    public static Dictionary<string, ServerCollisionData> CollisionDataList
    {
      get
      {
        if (StaticData.CollisionDataList == null)
          StaticData.LoadData();
        return StaticData._CollisionDataList;
      }
    }

    public static List<StructureSceneData> StructuresDataList
    {
      get
      {
        if (StaticData._StructuresDataList == null)
          StaticData.LoadData();
        return StaticData._StructuresDataList;
      }
    }

    public static List<AsteroidSceneData> AsteroidDataList
    {
      get
      {
        if (StaticData._AsteroidDataList == null)
          StaticData.LoadData();
        return StaticData._AsteroidDataList;
      }
    }

    public static Dictionary<short, DynamicObjectData> DynamicObjectsDataList
    {
      get
      {
        if (StaticData._DynamicObjectsDataList == null)
          StaticData.LoadData();
        return StaticData._DynamicObjectsDataList;
      }
    }

    public static Dictionary<ItemType, Dictionary<ResourceType, float>> ItemRecepiesList
    {
      get
      {
        if (StaticData._itemRecepiesList == null)
          StaticData.LoadData();
        return StaticData._itemRecepiesList;
      }
    }

    public static void LoadData()
    {
      string str = !Server.ConfigDir.IsNullOrEmpty() && Directory.Exists(Server.ConfigDir + "Data") ? Server.ConfigDir : "";
      StaticData._CelestialBodyDataList = Json.Load<List<CelestialBodyData>>(str + "Data/CelestialBodies.json");
      StaticData._StructuresDataList = Json.Load<List<StructureSceneData>>(str + "Data/Structures.json");
      StaticData._CollisionDataList = new Dictionary<string, ServerCollisionData>();
      StaticData._AsteroidDataList = Json.Load<List<AsteroidSceneData>>(str + "Data/Asteroids.json");
      List<ItemRecepieData> itemRecepieDataList = Json.Load<List<ItemRecepieData>>(str + "Data/ItemRecepies.json");
      StaticData._itemRecepiesList = new Dictionary<ItemType, Dictionary<ResourceType, float>>();
      foreach (ItemRecepieData itemRecepieData in itemRecepieDataList)
      {
        ItemType result;
        if (Enum.TryParse<ItemType>(itemRecepieData.Type, out result))
          StaticData._itemRecepiesList.Add(result, itemRecepieData.Recepie);
      }
      List<DynamicObjectData> dynamicObjectDataList = Json.Load<List<DynamicObjectData>>(str + "Data/DynamicObjects.json");
      StaticData._DynamicObjectsDataList = new Dictionary<short, DynamicObjectData>();
      foreach (DynamicObjectData dynamicObjectData in dynamicObjectDataList)
        StaticData._DynamicObjectsDataList.Add(dynamicObjectData.ItemID, dynamicObjectData);
      foreach (StructureSceneData structuresData in StaticData._StructuresDataList)
      {
        if (!StaticData._CollisionDataList.ContainsKey(structuresData.Collision))
        {
          structuresData.Colliders = Json.Load<ServerCollisionData>(str + "Data/Collision/" + structuresData.Collision + ".json");
          StaticData._CollisionDataList.Add(structuresData.Collision, structuresData.Colliders);
        }
      }
      foreach (AsteroidSceneData asteroidData in StaticData._AsteroidDataList)
      {
        if (asteroidData != null && asteroidData.Collision != null && !StaticData._CollisionDataList.ContainsKey(asteroidData.Collision))
        {
          asteroidData.Colliders = Json.Load<ServerCollisionData>(str + "Data/Collision/" + asteroidData.Collision + ".json");
          StaticData._CollisionDataList.Add(asteroidData.Collision, asteroidData.Colliders);
        }
      }
    }
  }
}
