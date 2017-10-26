// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Spawn.SpawnManager
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Network;
using ZeroGravity.Objects;
using ZeroGravity.ShipComponents;

namespace ZeroGravity.Spawn
{
  public static class SpawnManager
  {
    private static List<SpawnRule> startingSceneSpawnRules = new List<SpawnRule>();
    public static List<SpawnRule> timedSpawnRules = new List<SpawnRule>();
    public static Dictionary<long, Tuple<SpawnRule, SpawnRuleScene, int>> SpawnedVessels = new Dictionary<long, Tuple<SpawnRule, SpawnRuleScene, int>>();
    public static Dictionary<long, Tuple<SpawnRule, SpawnRuleLoot>> SpawnedDynamicObjects = new Dictionary<long, Tuple<SpawnRule, SpawnRuleLoot>>();
    private static Dictionary<ItemType, Dictionary<int, short>> itemTypeItemID = new Dictionary<ItemType, Dictionary<int, short>>();
    private static Dictionary<string, Dictionary<LootTier, List<LootItemData>>> lootCategories;
    private static List<SpawnRule> spawnRules;
    private static Dictionary<ItemType, Dictionary<int, Dictionary<GameScenes.SceneID, List<int>>>> lootSceneAttachPoints;

    private static void LoadData()
    {
      SpawnManager.lootCategories = SpawnSerialization.LoadLootData();
      SpawnManager.spawnRules = SpawnSerialization.LoadSpawnRuleData();
      for (int index = 0; index < SpawnManager.spawnRules.Count; ++index)
      {
        if (SpawnManager.spawnRules[index].LocationType == SpawnRuleLocationType.StartingScene)
          SpawnManager.startingSceneSpawnRules.Add(SpawnManager.spawnRules[index]);
        else if (SpawnManager.spawnRules[index].RespawnTimerSec > double.Epsilon)
        {
          SpawnManager.spawnRules[index].CurrTimerSec = SpawnManager.spawnRules[index].RespawnTimerSec * MathHelper.RandomRange(0.0, 0.3);
          SpawnManager.timedSpawnRules.Add(SpawnManager.spawnRules[index]);
        }
      }
      foreach (KeyValuePair<short, DynamicObjectData> dynamicObjectsData in StaticData.DynamicObjectsDataList)
      {
        if (dynamicObjectsData.Value.ItemType == ItemType.GenericItem && dynamicObjectsData.Value.DefaultAuxData != null && dynamicObjectsData.Value.DefaultAuxData is GenericItemData)
          SpawnManager.AddItemTypeItemID(dynamicObjectsData.Value.ItemType, (int) (dynamicObjectsData.Value.DefaultAuxData as GenericItemData).SubType, dynamicObjectsData.Key);
        else if (dynamicObjectsData.Value.ItemType == ItemType.MachineryPart && dynamicObjectsData.Value.DefaultAuxData != null && dynamicObjectsData.Value.DefaultAuxData is MachineryPartData)
          SpawnManager.AddItemTypeItemID(dynamicObjectsData.Value.ItemType, (int) (dynamicObjectsData.Value.DefaultAuxData as MachineryPartData).PartType, dynamicObjectsData.Key);
        else if (dynamicObjectsData.Value.ItemType != ItemType.GenericItem && dynamicObjectsData.Value.ItemType != ItemType.MachineryPart && (uint) dynamicObjectsData.Value.ItemType > 0U)
          SpawnManager.AddItemTypeItemID(dynamicObjectsData.Value.ItemType, 0, dynamicObjectsData.Key);
      }
    }

    private static void AddItemTypeItemID(ItemType itemType, int itemSubType, short itemID)
    {
      if (SpawnManager.itemTypeItemID == null)
        SpawnManager.itemTypeItemID = new Dictionary<ItemType, Dictionary<int, short>>();
      if ((itemType == ItemType.GenericItem || itemType == ItemType.MachineryPart) && itemSubType == 0)
        return;
      if (!SpawnManager.itemTypeItemID.ContainsKey(itemType))
        SpawnManager.itemTypeItemID.Add(itemType, new Dictionary<int, short>());
      if (SpawnManager.itemTypeItemID[itemType].ContainsKey(itemSubType))
        return;
      SpawnManager.itemTypeItemID[itemType].Add(itemSubType, itemID);
    }

    private static void AddLootSceneAttachPoint(GameScenes.SceneID sceneID, ItemType itemType, int itemSubType, int attachPointInSceneID)
    {
      if ((itemType == ItemType.GenericItem || itemType == ItemType.MachineryPart) && itemSubType == 0 || sceneID == GameScenes.SceneID.None)
        return;
      if (SpawnManager.lootSceneAttachPoints == null)
        SpawnManager.lootSceneAttachPoints = new Dictionary<ItemType, Dictionary<int, Dictionary<GameScenes.SceneID, List<int>>>>();
      if (!SpawnManager.lootSceneAttachPoints.ContainsKey(itemType))
        SpawnManager.lootSceneAttachPoints.Add(itemType, new Dictionary<int, Dictionary<GameScenes.SceneID, List<int>>>());
      if (!SpawnManager.lootSceneAttachPoints[itemType].ContainsKey(itemSubType))
        SpawnManager.lootSceneAttachPoints[itemType].Add(itemSubType, new Dictionary<GameScenes.SceneID, List<int>>());
      if (!SpawnManager.lootSceneAttachPoints[itemType][itemSubType].ContainsKey(sceneID))
        SpawnManager.lootSceneAttachPoints[itemType][itemSubType].Add(sceneID, new List<int>());
      if (SpawnManager.lootSceneAttachPoints[itemType][itemSubType][sceneID].Contains(attachPointInSceneID))
        return;
      SpawnManager.lootSceneAttachPoints[itemType][itemSubType][sceneID].Add(attachPointInSceneID);
    }

    private static void LoadLootSceneAttachPoints()
    {
      foreach (StructureSceneData structuresData in StaticData.StructuresDataList)
      {
        GameScenes.SceneID itemId = (GameScenes.SceneID) structuresData.ItemID;
        foreach (BaseAttachPointData attachPoint in structuresData.AttachPoints)
        {
          if (attachPoint.ItemTypes != null)
          {
            foreach (ItemType itemType in attachPoint.ItemTypes)
            {
              if (itemType == ItemType.GenericItem && attachPoint.GenericSubTypes != null)
              {
                foreach (GenericItemSubType genericSubType in attachPoint.GenericSubTypes)
                  SpawnManager.AddLootSceneAttachPoint(itemId, itemType, (int) genericSubType, attachPoint.InSceneID);
              }
              else if (itemType == ItemType.MachineryPart && attachPoint.MachineryPartTypes != null)
              {
                foreach (MachineryPartType machineryPartType in attachPoint.MachineryPartTypes)
                  SpawnManager.AddLootSceneAttachPoint(itemId, itemType, (int) machineryPartType, attachPoint.InSceneID);
              }
              else if (itemType != ItemType.GenericItem && itemType != ItemType.MachineryPart)
                SpawnManager.AddLootSceneAttachPoint(itemId, itemType, 0, attachPoint.InSceneID);
            }
          }
        }
      }
    }

    private static void InitializeSpawnRules(bool isPersistenceInitialize)
    {
      foreach (SpawnRule spawnRule in SpawnManager.spawnRules)
      {
        if (spawnRule.LocationType != SpawnRuleLocationType.StartingScene)
          spawnRule.Initialize(isPersistenceInitialize);
      }
    }

    private static void ExecuteSpawnRule(SpawnRule rule)
    {
      if (rule.LocationType == SpawnRuleLocationType.StartingScene)
        return;
      rule.ExecuteRule(false);
    }

    public static VesselAttachPoint GetLootAttachPoint(LootItemData data, ref List<SpaceObjectVessel> vessels)
    {
      if (!SpawnManager.lootSceneAttachPoints.ContainsKey(data.Type) || vessels.Count == 0)
        return (VesselAttachPoint) null;
      int subType = data.GetSubType();
      if (!SpawnManager.lootSceneAttachPoints[data.Type].ContainsKey(subType))
        return (VesselAttachPoint) null;
      List<SpaceObjectVessel> spaceObjectVesselList = new List<SpaceObjectVessel>((IEnumerable<SpaceObjectVessel>) vessels);
      while (spaceObjectVesselList.Count > 0)
      {
        int index1 = MathHelper.RandomRange(0, spaceObjectVesselList.Count);
        if (SpawnManager.lootSceneAttachPoints[data.Type][subType].ContainsKey(spaceObjectVesselList[index1].SceneID))
        {
          SpaceObjectVessel spaceObjectVessel = spaceObjectVesselList[index1];
          List<int> intList = SpawnManager.lootSceneAttachPoints[data.Type][subType][spaceObjectVesselList[index1].SceneID];
          for (int index2 = 0; index2 < intList.Count; ++index2)
          {
            if (spaceObjectVessel.AttachPoints.ContainsKey(intList[index2]) && spaceObjectVessel.AttachPoints[intList[index2]].Item == null)
              return spaceObjectVessel.AttachPoints[intList[index2]];
          }
          spaceObjectVesselList.RemoveAt(index1);
        }
        else
          spaceObjectVesselList.RemoveAt(index1);
      }
      return (VesselAttachPoint) null;
    }

    private static bool AddResourcesToCargoCompartment(ref CargoCompartmentData compartment, LootItemData.CargoResourceData cargo)
    {
      float num1 = MathHelper.RandomRange(cargo.Quantity.Min, cargo.Quantity.Max);
      if ((double) num1 < 1.40129846432482E-45)
        return false;
      List<ResourceType> resourceTypeList = new List<ResourceType>((IEnumerable<ResourceType>) cargo.Resources);
      ResourceType resourceType;
      do
      {
        if (resourceTypeList.Count > 0)
        {
          resourceType = resourceTypeList[MathHelper.RandomRange(0, resourceTypeList.Count)];
          resourceTypeList.Remove(resourceType);
        }
        else
          goto label_21;
      }
      while (!compartment.AllowedResources.Contains(resourceType) || compartment.AllowOnlyOneType && compartment.Resources != null && compartment.Resources.Count != 0 && compartment.Resources[0].ResourceType != resourceType);
      if (compartment.Resources == null)
        compartment.Resources = new List<ZeroGravity.Data.CargoResourceData>();
      float capacity = compartment.Capacity;
      ZeroGravity.Data.CargoResourceData cargoResourceData = (ZeroGravity.Data.CargoResourceData) null;
      foreach (ZeroGravity.Data.CargoResourceData resource in compartment.Resources)
      {
        if (resource.ResourceType == resourceType)
          cargoResourceData = resource;
        else
          capacity -= resource.Quantity;
      }
      float num2 = MathHelper.Clamp(num1, 0.0f, capacity);
      if ((double) num2 <= 1.40129846432482E-45)
        return true;
      if (cargoResourceData == null)
        compartment.Resources.Add(new ZeroGravity.Data.CargoResourceData()
        {
          ResourceType = resourceType,
          Quantity = num2
        });
      else
        cargoResourceData.Quantity = num2;
      return true;
label_21:
      return false;
    }

    private static IDynamicObjectAuxData GetDynamicObjectAuxData(short itemID, LootItemData data)
    {
      if (!StaticData.DynamicObjectsDataList.ContainsKey(itemID))
        return (IDynamicObjectAuxData) null;
      IDynamicObjectAuxData defaultAuxData = StaticData.DynamicObjectsDataList[itemID].DefaultAuxData;
      if (defaultAuxData is JetpackData)
      {
        if (data.Power.HasValue)
        {
          JetpackData jetpackData = defaultAuxData as JetpackData;
          jetpackData.OxygenCompartment.Resources[0].Quantity = 0.0f;
          jetpackData.OxygenCompartment.Resources[0].SpawnSettings = (ResourcesSpawnSettings[]) null;
          jetpackData.PropellantCompartment.Resources[0].Quantity = 0.0f;
          jetpackData.PropellantCompartment.Resources[0].SpawnSettings = (ResourcesSpawnSettings[]) null;
          jetpackData.CurrentPower = System.Math.Max(MathHelper.RandomRange(data.Power.Value.Min, data.Power.Value.Max), 0.0f);
          if ((double) jetpackData.CurrentPower > (double) jetpackData.MaxPower)
            jetpackData.MaxPower = jetpackData.CurrentPower;
        }
        if (data.Cargo != null && data.Cargo.Count > 0)
        {
          JetpackData jetpackData = defaultAuxData as JetpackData;
          foreach (LootItemData.CargoResourceData cargo in data.Cargo)
          {
            if (!SpawnManager.AddResourcesToCargoCompartment(ref jetpackData.OxygenCompartment, cargo))
              SpawnManager.AddResourcesToCargoCompartment(ref jetpackData.PropellantCompartment, cargo);
          }
        }
      }
      else if (defaultAuxData is MagazineData)
      {
        if (data.Count.HasValue)
        {
          MagazineData magazineData = defaultAuxData as MagazineData;
          magazineData.BulletCount = System.Math.Max(MathHelper.RandomRange(data.Count.Value.Min, data.Count.Value.Max), 0);
          if (magazineData.MaxBulletCount < magazineData.BulletCount)
            magazineData.MaxBulletCount = magazineData.BulletCount;
        }
      }
      else if (defaultAuxData is BatteryData)
      {
        if (data.Power.HasValue)
        {
          BatteryData batteryData = defaultAuxData as BatteryData;
          batteryData.CurrentPower = System.Math.Max(MathHelper.RandomRange(data.Power.Value.Min, data.Power.Value.Max), 0.0f);
          if ((double) batteryData.MaxPower < (double) batteryData.CurrentPower)
            batteryData.MaxPower = batteryData.CurrentPower;
        }
      }
      else if (defaultAuxData is CanisterData)
      {
        if (data.Cargo != null && data.Cargo.Count > 0)
        {
          CanisterData canisterData = defaultAuxData as CanisterData;
          canisterData.CargoCompartment.Resources = (List<ZeroGravity.Data.CargoResourceData>) null;
          foreach (LootItemData.CargoResourceData cargo in data.Cargo)
            SpawnManager.AddResourcesToCargoCompartment(ref canisterData.CargoCompartment, cargo);
        }
      }
      else if (defaultAuxData is RepairToolData)
      {
        if (data.Cargo != null && data.Cargo.Count > 0)
        {
          RepairToolData repairToolData = defaultAuxData as RepairToolData;
          repairToolData.FuelCompartment.Resources[0].Quantity = 0.0f;
          repairToolData.FuelCompartment.Resources[0].SpawnSettings = (ResourcesSpawnSettings[]) null;
          foreach (LootItemData.CargoResourceData cargo in data.Cargo)
            SpawnManager.AddResourcesToCargoCompartment(ref repairToolData.FuelCompartment, cargo);
        }
      }
      else if (defaultAuxData is GlowStickData)
      {
        if (data.IsActive.HasValue)
          (defaultAuxData as GlowStickData).IsOn = data.IsActive.Value;
      }
      else if (defaultAuxData is PortableTurretData)
      {
        if (data.IsActive.HasValue)
          (defaultAuxData as PortableTurretData).IsActive = data.IsActive.Value;
      }
      else if (defaultAuxData is GenericItemData && (data.Look != null && data.Look.Count > 0))
        (defaultAuxData as GenericItemData).Look = data.Look[MathHelper.RandomRange(0, data.Look.Count)];
      return defaultAuxData;
    }

    public static bool SpawnDynamicObject(SpawnRule rule, SpawnRuleLoot loot, VesselAttachPoint ap)
    {
      if (ap == null)
      {
        Dbg.Warning((object) "SPAWN MANAGER - Unable to spawn item because atach point is null", (object) loot.Data.Type);
        return false;
      }
      if (!SpawnManager.itemTypeItemID.ContainsKey(loot.Data.Type))
        throw new Exception("SPAWN MANAGER - Unable to spawn item because type is unknown, " + (object) loot.Data.Type);
      int subType = loot.Data.GetSubType();
      if (!SpawnManager.itemTypeItemID[loot.Data.Type].ContainsKey(subType))
        throw new Exception("SPAWN MANAGER - Unable to spawn item because subtype is unknown, " + (object) loot.Data.Type + ", " + (object) subType);
      short itemID = SpawnManager.itemTypeItemID[loot.Data.Type][subType];
      IDynamicObjectAuxData dynamicObjectAuxData = SpawnManager.GetDynamicObjectAuxData(itemID, loot.Data);
      if (dynamicObjectAuxData == null)
        throw new Exception("SPAWN MANAGER - Unable to spawn item because aux data does not exist, " + (object) loot.Data.Type + ", " + (object) subType);
      DynamicObjectSceneData dosd = new DynamicObjectSceneData();
      dosd.ItemID = itemID;
      float[] floatArray1 = Vector3D.Zero.ToFloatArray();
      dosd.Position = floatArray1;
      float[] floatArray2 = Vector3D.Forward.ToFloatArray();
      dosd.Forward = floatArray2;
      float[] floatArray3 = Vector3D.Up.ToFloatArray();
      dosd.Up = floatArray3;
      int inSceneId = ap.InSceneID;
      dosd.AttachPointInSceneId = inSceneId;
      IDynamicObjectAuxData idynamicObjectAuxData = dynamicObjectAuxData;
      dosd.AuxData = idynamicObjectAuxData;
      // ISSUE: variable of the null type
      __Null local = null;
      dosd.SpawnSettings = (DynaminObjectSpawnSettings[]) local;
      DynamicObject dynamicObject = new DynamicObject(dosd, (SpaceObject) ap.Vessel, -1L);
      if (dynamicObject.Item == null)
        return true;
      if (dynamicObject.Item != null && loot.Data.Health.HasValue)
      {
        IDamageable damageable = (IDamageable) dynamicObject.Item;
        if ((double) loot.Data.Health.Value.Max > (double) damageable.MaxHealth)
          damageable.MaxHealth = loot.Data.Health.Value.Max;
        damageable.Health = MathHelper.RandomRange(loot.Data.Health.Value.Min, loot.Data.Health.Value.Max);
      }
      AttachPointDetails data = new AttachPointDetails()
      {
        InSceneID = ap.InSceneID
      };
      dynamicObject.Item.SetAttachPoint(data);
      dynamicObject.APDetails = data;
      if (dynamicObject.Item is MachineryPart)
      {
        (dynamicObject.Item as MachineryPart).WearMultiplier = 1f;
        if (dynamicObject.Item.AttachPointType == AttachPointType.MachineryPartSlot)
          ap.Vessel.FitMachineryPart(dynamicObject.Item.AttachPointKey, dynamicObject.Item as MachineryPart);
      }
      if (rule.LocationType != SpawnRuleLocationType.StartingScene)
      {
        dynamicObject.IsPartOfSpawnSystem = true;
        SpawnManager.SpawnedDynamicObjects.Add(dynamicObject.GUID, new Tuple<SpawnRule, SpawnRuleLoot>(rule, loot));
      }
      Server.Instance.NetworkController.SendToClientsSubscribedTo((NetworkData) new SpawnObjectsResponse()
      {
        Data = {
          dynamicObject.GetSpawnResponseData((Player) null)
        }
      }, -1L, dynamicObject.Parent);
      return true;
    }

    public static List<LootItemData> GetLootItemDataFromCategory(string ruleName, string categoryName, LootTier tier)
    {
      if (!SpawnManager.lootCategories.ContainsKey(categoryName))
      {
        Dbg.Error(string.Format("SPAWN MANAGER - Rule \"{0}\", loot category \"{1}\" does not exist", (object) ruleName, (object) categoryName));
        return new List<LootItemData>();
      }
      if (SpawnManager.lootCategories[categoryName].ContainsKey(tier))
        return SpawnManager.lootCategories[categoryName][tier];
      Dbg.Error(string.Format("SPAWN MANAGER - Rule \"{0}\", loot category \"{1}\" has no tier \"{2}\"", (object) ruleName, (object) categoryName, (object) tier.ToString()));
      return new List<LootItemData>();
    }

    private static ShipSpawnPoint AuthorizeAndLockSpawnPoints(SpaceObjectVessel ves, Player pl)
    {
      if (ves.SpawnPoints.Count <= 0)
        return (ShipSpawnPoint) null;
      foreach (ShipSpawnPoint spawnPoint in ves.SpawnPoints)
      {
        spawnPoint.Type = SpawnPointType.WithAuthorization;
        spawnPoint.State = SpawnPointState.Locked;
        spawnPoint.Player = pl;
      }
      return ves.SpawnPoints[0];
    }

    public static ShipSpawnPoint SetStartingSetupSpawnPoints(SpaceObjectVessel ves, Player pl)
    {
      ShipSpawnPoint shipSpawnPoint1 = SpawnManager.AuthorizeAndLockSpawnPoints(ves, pl);
      SpaceObjectVessel ves1 = ves;
      if (ves1.DockedToMainVessel != null && ves1.DockedToMainVessel is Ship)
        ves1 = (SpaceObjectVessel) (ves1.DockedToMainVessel as Ship);
      if (ves1.AllDockedVessels.Count > 0)
      {
        if (ves1 != ves)
        {
          ShipSpawnPoint shipSpawnPoint2 = SpawnManager.AuthorizeAndLockSpawnPoints(ves1, pl);
          if (shipSpawnPoint1 == null && shipSpawnPoint2 != null)
            shipSpawnPoint1 = shipSpawnPoint2;
        }
        foreach (SpaceObjectVessel allDockedVessel in ves1.AllDockedVessels)
        {
          if (allDockedVessel != ves)
          {
            ShipSpawnPoint shipSpawnPoint2 = SpawnManager.AuthorizeAndLockSpawnPoints(allDockedVessel, pl);
            if (shipSpawnPoint1 == null && shipSpawnPoint2 != null)
              shipSpawnPoint1 = shipSpawnPoint2;
          }
        }
      }
      if (shipSpawnPoint1 != null)
        shipSpawnPoint1.State = SpawnPointState.Authorized;
      return shipSpawnPoint1;
    }

    public static Ship SpawnStartingSetup(Server server)
    {
      if (Server.SetupType != Server.ServerSetupType.Default)
        return CustomServerInitializers.SpawnStartingModule(server, Server.SetupType);
      SpawnRule startingSceneSpawnRule = SpawnManager.startingSceneSpawnRules[0];
      if (SpawnManager.startingSceneSpawnRules.Count > 1)
        startingSceneSpawnRule = SpawnManager.startingSceneSpawnRules[MathHelper.RandomRange(0, SpawnManager.startingSceneSpawnRules.Count)];
      return startingSceneSpawnRule.ExecuteRule(false) as Ship;
    }

    private static string ItemDataDebugString(LootItemData data)
    {
      string str = data.Type != ItemType.GenericItem ? (data.Type != ItemType.MachineryPart ? data.Type.ToString() : data.Type.ToString() + " - " + data.PartType.ToString()) : data.Type.ToString() + " - " + data.GenericSubType.ToString();
      if (data.Health.HasValue)
        str = str + ", Health (" + data.Health.Value.Min.ToString() + ", " + data.Health.Value.Max.ToString() + ")";
      return str;
    }

    private static void PrintDebugInfo(bool printCategories = false, bool printSpawnRules = false, bool printAttachPoints = false, bool printItemTypeIDs = false)
    {
      string str = "";
      if (printCategories && SpawnManager.lootCategories != null && SpawnManager.lootCategories.Count > 0)
      {
        str = str + "\nCategories:\n" + new string('=', 78);
        foreach (KeyValuePair<string, Dictionary<LootTier, List<LootItemData>>> lootCategory in SpawnManager.lootCategories)
        {
          str = str + "\nName: " + lootCategory.Key + "\n";
          foreach (KeyValuePair<LootTier, List<LootItemData>> keyValuePair in lootCategory.Value)
          {
            str = str + "  Tier: " + keyValuePair.Key.ToString() + "\n";
            foreach (LootItemData data in keyValuePair.Value)
              str = str + "    " + SpawnManager.ItemDataDebugString(data) + "\n";
          }
        }
      }
      if (printSpawnRules)
      {
        str = str + "\n\nRules:\n" + new string('=', 78);
        foreach (SpawnRule spawnRule in SpawnManager.spawnRules)
        {
          str = str + "\nName: " + spawnRule.Name + "\n  Orbit: " + spawnRule.Orbit.CelestialBody.ToString() + "\n    PER (" + (object) spawnRule.Orbit.PeriapsisDistance.Min + ", " + (object) spawnRule.Orbit.PeriapsisDistance.Max + ")\n    APO (" + (object) spawnRule.Orbit.ApoapsisDistance.Min + ", " + (object) spawnRule.Orbit.ApoapsisDistance.Max + ")\n    INC (" + (object) spawnRule.Orbit.Inclination.Min + ", " + (object) spawnRule.Orbit.Inclination.Max + ")\n    AOP (" + (object) spawnRule.Orbit.ArgumentOfPeriapsis.Min + ", " + (object) spawnRule.Orbit.ArgumentOfPeriapsis.Max + ")\n    LOA (" + (object) spawnRule.Orbit.LongitudeOfAscendingNode.Min + ", " + (object) spawnRule.Orbit.LongitudeOfAscendingNode.Max + ")\n    TAN (" + (object) spawnRule.Orbit.TrueAnomaly.Min + ", " + (object) spawnRule.Orbit.TrueAnomaly.Max + ")\n  Location type: " + (object) spawnRule.LocationType + ", tag: " + spawnRule.LocationTag + "\n  Respawn Timer: (" + (object) spawnRule.RespawnTimerSec + " - " + (object) spawnRule.CurrTimerSec + ")\n  Clusters (" + (object) spawnRule.NumberOfClusters.Min + ", " + (object) spawnRule.NumberOfClusters.Max + ")\n";
          if (spawnRule.ScenePool != null)
          {
            str += "  Scene Pool:\n";
            foreach (SpawnRuleScene spawnRuleScene in spawnRule.ScenePool)
              str = str + "    " + spawnRuleScene.SceneID.ToString() + ", Count: (" + (object) spawnRuleScene.Count + "," + (object) spawnRuleScene.CountMax + "), Health: (" + (object) spawnRuleScene.HealthMin + "," + (object) spawnRuleScene.HealthMax + ")\n";
          }
          if (spawnRule.LootPool != null)
          {
            str += "  Loot Pool\n";
            foreach (SpawnRuleLoot spawnRuleLoot in spawnRule.LootPool)
              str = str + "    " + SpawnManager.ItemDataDebugString(spawnRuleLoot.Data) + ", Count: (" + (object) spawnRuleLoot.Count + "," + (object) spawnRuleLoot.CountMax + ")\n";
          }
        }
      }
      if (printAttachPoints && SpawnManager.lootSceneAttachPoints != null && SpawnManager.lootSceneAttachPoints.Count > 0)
      {
        str = str + "\n\nItem Attach Points:\n" + new string('=', 78);
        foreach (KeyValuePair<ItemType, Dictionary<int, Dictionary<GameScenes.SceneID, List<int>>>> sceneAttachPoint in SpawnManager.lootSceneAttachPoints)
        {
          foreach (KeyValuePair<int, Dictionary<GameScenes.SceneID, List<int>>> keyValuePair1 in sceneAttachPoint.Value)
          {
            str = str + "\n" + sceneAttachPoint.Key.ToString();
            str = keyValuePair1.Key == 0 || sceneAttachPoint.Key != ItemType.GenericItem ? (keyValuePair1.Key == 0 || sceneAttachPoint.Key != ItemType.MachineryPart ? str + "\n" : str + " - " + ((MachineryPartType) keyValuePair1.Key).ToString() + "\n") : str + " - " + ((GenericItemSubType) keyValuePair1.Key).ToString() + "\n";
            foreach (KeyValuePair<GameScenes.SceneID, List<int>> keyValuePair2 in keyValuePair1.Value)
              str = str + "  " + keyValuePair2.Key.ToString() + " : " + (object) keyValuePair2.Value.Count + "\n";
          }
        }
      }
      if (printItemTypeIDs && SpawnManager.itemTypeItemID != null && SpawnManager.itemTypeItemID.Count > 0)
      {
        str = str + "\n\nItem Type - Item ID:\n" + new string('=', 78);
        foreach (KeyValuePair<ItemType, Dictionary<int, short>> keyValuePair1 in SpawnManager.itemTypeItemID)
        {
          foreach (KeyValuePair<int, short> keyValuePair2 in keyValuePair1.Value)
          {
            str = str + "\n" + keyValuePair1.Key.ToString();
            if (keyValuePair2.Key != 0 && keyValuePair1.Key == ItemType.GenericItem)
              str = str + " - " + ((GenericItemSubType) keyValuePair2.Key).ToString();
            else if (keyValuePair2.Key != 0 && keyValuePair1.Key == ItemType.MachineryPart)
              str = str + " - " + ((MachineryPartType) keyValuePair2.Key).ToString();
            str = str + " ID: " + (object) keyValuePair2.Value;
          }
        }
      }
      Dbg.Info(str + "\n");
    }

    private static void GenerateSampleData(bool force)
    {
      SpawnSerialization.GenerateLootSampleData(force);
      SpawnSerialization.GenerateSpawnRuleSampleData(force);
    }

    public static void Initialize(bool isPersistenceInitialize = false)
    {
      SpawnManager.LoadData();
      if (SpawnManager.spawnRules == null || SpawnManager.spawnRules.Count == 0)
        throw new Exception("SPAWN MANAGER - Spawn rules are not set.");
      if (Server.SetupType == Server.ServerSetupType.Default && SpawnManager.startingSceneSpawnRules.Count == 0)
        throw new Exception("SPAWN MANAGER - There are no starting scene spawn rules.");
      SpawnManager.LoadLootSceneAttachPoints();
      if (!isPersistenceInitialize && SpawnManager.Settings.PrintInfo)
        SpawnManager.PrintDebugInfo(SpawnManager.Settings.PrintCategories, SpawnManager.Settings.PrintSpawnRules, SpawnManager.Settings.PrintItemAttachPoints, SpawnManager.Settings.PrintItemTypeIDs);
      SpawnManager.InitializeSpawnRules(isPersistenceInitialize);
    }

    public static PersistenceObjectData GetPersistenceData()
    {
      PersistenceObjectDataSpawnManager dataSpawnManager = new PersistenceObjectDataSpawnManager();
      dataSpawnManager.SpawnRules = new Dictionary<string, PersistenceObjectDataSpawnManager.SpawnRule>();
      Dictionary<SpawnRuleScene, PersistenceObjectDataSpawnManager.SpawnRuleScene> dictionary1 = new Dictionary<SpawnRuleScene, PersistenceObjectDataSpawnManager.SpawnRuleScene>();
      Dictionary<SpawnRuleLoot, PersistenceObjectDataSpawnManager.SpawnRuleLoot> dictionary2 = new Dictionary<SpawnRuleLoot, PersistenceObjectDataSpawnManager.SpawnRuleLoot>();
      if (SpawnManager.spawnRules != null && SpawnManager.spawnRules.Count > 0)
      {
        foreach (SpawnRule spawnRule1 in SpawnManager.spawnRules)
        {
          if (spawnRule1.LocationType != SpawnRuleLocationType.StartingScene)
          {
            PersistenceObjectDataSpawnManager.SpawnRule spawnRule2 = new PersistenceObjectDataSpawnManager.SpawnRule()
            {
              CurrTimerSec = spawnRule1.CurrTimerSec,
              ScenePool = new List<PersistenceObjectDataSpawnManager.SpawnRuleScene>(),
              LootPool = new List<PersistenceObjectDataSpawnManager.SpawnRuleLoot>(),
              SpawnedVessels = new List<long>()
            };
            if (spawnRule1.ScenePool != null)
            {
              foreach (SpawnRuleScene key in spawnRule1.ScenePool)
              {
                PersistenceObjectDataSpawnManager.SpawnRuleScene spawnRuleScene = new PersistenceObjectDataSpawnManager.SpawnRuleScene()
                {
                  Vessels = new List<Tuple<long, int>>()
                };
                dictionary1.Add(key, spawnRuleScene);
                spawnRule2.ScenePool.Add(spawnRuleScene);
              }
            }
            if (spawnRule1.LootPool != null)
            {
              foreach (SpawnRuleLoot key in spawnRule1.LootPool)
              {
                PersistenceObjectDataSpawnManager.SpawnRuleLoot spawnRuleLoot = new PersistenceObjectDataSpawnManager.SpawnRuleLoot()
                {
                  DynamicObjects = new List<long>()
                };
                dictionary2.Add(key, spawnRuleLoot);
                spawnRule2.LootPool.Add(spawnRuleLoot);
              }
            }
            foreach (SpaceObjectVessel spawnedVessel in spawnRule1.SpawnedVessels)
              spawnRule2.SpawnedVessels.Add(spawnedVessel.GUID);
            dataSpawnManager.SpawnRules.Add(spawnRule1.Name, spawnRule2);
          }
        }
      }
      if (SpawnManager.SpawnedVessels != null && SpawnManager.SpawnedVessels.Count > 0)
      {
        foreach (KeyValuePair<long, Tuple<SpawnRule, SpawnRuleScene, int>> spawnedVessel in SpawnManager.SpawnedVessels)
        {
          if (dictionary1.ContainsKey(spawnedVessel.Value.Item2))
            dictionary1[spawnedVessel.Value.Item2].Vessels.Add(new Tuple<long, int>(spawnedVessel.Key, spawnedVessel.Value.Item3));
        }
      }
      if (SpawnManager.SpawnedDynamicObjects != null && SpawnManager.SpawnedDynamicObjects.Count > 0)
      {
        foreach (KeyValuePair<long, Tuple<SpawnRule, SpawnRuleLoot>> spawnedDynamicObject in SpawnManager.SpawnedDynamicObjects)
        {
          if (dictionary2.ContainsKey(spawnedDynamicObject.Value.Item2))
            dictionary2[spawnedDynamicObject.Value.Item2].DynamicObjects.Add(spawnedDynamicObject.Key);
        }
      }
      return (PersistenceObjectData) dataSpawnManager;
    }

    public static void LoadPersistenceData(PersistenceObjectData persistenceData)
    {
      SpawnManager.Initialize(true);
      PersistenceObjectDataSpawnManager dataSpawnManager = persistenceData as PersistenceObjectDataSpawnManager;
      if (dataSpawnManager == null || dataSpawnManager.SpawnRules == null)
        return;
      foreach (KeyValuePair<string, PersistenceObjectDataSpawnManager.SpawnRule> spawnRule1 in dataSpawnManager.SpawnRules)
      {
        KeyValuePair<string, PersistenceObjectDataSpawnManager.SpawnRule> p_sr = spawnRule1;
        SpawnRule spawnRule2 = SpawnManager.spawnRules.Find((Predicate<SpawnRule>) (m => m.Name == p_sr.Key));
        if (spawnRule2 != null)
        {
          spawnRule2.CurrTimerSec = p_sr.Value.CurrTimerSec;
          if (p_sr.Value.ScenePool != null)
          {
            for (int index = 0; index < p_sr.Value.ScenePool.Count && index < spawnRule2.ScenePool.Count; ++index)
            {
              if (p_sr.Value.ScenePool[index].Vessels != null)
              {
                foreach (Tuple<long, int> vessel1 in p_sr.Value.ScenePool[index].Vessels)
                {
                  SpaceObjectVessel vessel2 = Server.Instance.GetVessel(vessel1.Item1);
                  if (vessel2 != null && spawnRule2.AddVesselToRule(vessel2, spawnRule2.ScenePool[index], vessel1.Item2))
                  {
                    vessel2.IsPartOfSpawnSystem = true;
                    SpawnManager.SpawnedVessels.Add(vessel2.GUID, new Tuple<SpawnRule, SpawnRuleScene, int>(spawnRule2, spawnRule2.ScenePool[index], vessel1.Item2));
                  }
                }
              }
            }
          }
          if (p_sr.Value.LootPool != null)
          {
            for (int index = 0; index < p_sr.Value.LootPool.Count && index < spawnRule2.LootPool.Count; ++index)
            {
              if (p_sr.Value.LootPool[index].DynamicObjects != null)
              {
                foreach (long dynamicObject in p_sr.Value.LootPool[index].DynamicObjects)
                {
                  DynamicObject dobj = Server.Instance.GetObject(dynamicObject) as DynamicObject;
                  if (dobj != null && spawnRule2.AddDynamicObjectToRule(dobj, spawnRule2.LootPool[index]))
                  {
                    dobj.IsPartOfSpawnSystem = true;
                    SpawnManager.SpawnedDynamicObjects.Add(dobj.GUID, new Tuple<SpawnRule, SpawnRuleLoot>(spawnRule2, spawnRule2.LootPool[index]));
                  }
                }
              }
            }
          }
          if (p_sr.Value.SpawnedVessels != null)
          {
            foreach (long spawnedVessel in p_sr.Value.SpawnedVessels)
            {
              SpaceObjectVessel vessel = Server.Instance.GetVessel(spawnedVessel);
              if (vessel != null && !spawnRule2.SpawnedVessels.Contains(vessel))
                spawnRule2.SpawnedVessels.Add(vessel);
            }
          }
        }
      }
      if (!SpawnManager.Settings.PrintInfo)
        return;
      SpawnManager.PrintDebugInfo(SpawnManager.Settings.PrintCategories, SpawnManager.Settings.PrintSpawnRules, SpawnManager.Settings.PrintItemAttachPoints, SpawnManager.Settings.PrintItemTypeIDs);
    }

    public static void UpdateTimers(double deltaTime)
    {
      foreach (SpawnRule timedSpawnRule in SpawnManager.timedSpawnRules)
      {
        timedSpawnRule.CurrTimerSec += deltaTime;
        if (timedSpawnRule.CurrTimerSec >= timedSpawnRule.RespawnTimerSec)
        {
          SpawnManager.ExecuteSpawnRule(timedSpawnRule);
          timedSpawnRule.CurrTimerSec = 0.0;
        }
      }
    }

    public static void RemoveSpawnSystemObject(SpaceObject obj, bool checkChildren)
    {
      obj.IsPartOfSpawnSystem = false;
      if (obj is DynamicObject && SpawnManager.SpawnedDynamicObjects.ContainsKey(obj.GUID))
      {
        if (!SpawnManager.SpawnedDynamicObjects[obj.GUID].Item1.RemoveDynamicObject(obj as DynamicObject, SpawnManager.SpawnedDynamicObjects[obj.GUID].Item2))
          return;
        SpawnManager.SpawnedDynamicObjects.Remove(obj.GUID);
      }
      else
      {
        if (!(obj is SpaceObjectVessel) || !SpawnManager.SpawnedVessels.ContainsKey(obj.GUID))
          return;
        SpaceObjectVessel ves = obj as SpaceObjectVessel;
        if (checkChildren)
        {
          foreach (DynamicObject dynamicObject in ves.DynamicObjects)
          {
            if (dynamicObject.IsPartOfSpawnSystem)
              SpawnManager.RemoveSpawnSystemObject((SpaceObject) dynamicObject, false);
          }
        }
        if (SpawnManager.SpawnedVessels[ves.GUID].Item1.RemoveSpaceObjectVessel(ves, SpawnManager.SpawnedVessels[ves.GUID].Item2, SpawnManager.SpawnedVessels[ves.GUID].Item3))
          SpawnManager.SpawnedVessels.Remove(obj.GUID);
      }
    }

    public static class Settings
    {
      public static double StartingLocationCheckDistance = 1000.0;
      public static double RandomLocationCheckDistance = 10000.0;
      public static double RandomLocationClusterItemCheckDistance = 20.0;
      public static bool PrintCategories = false;
      public static bool PrintSpawnRules = false;
      public static bool PrintItemAttachPoints = false;
      public static bool PrintItemTypeIDs = false;
      public static string StationTag = "StationArena";
      public static string StartingTag = "StartingScene";

      public static bool PrintInfo
      {
        get
        {
          return SpawnManager.Settings.PrintCategories || SpawnManager.Settings.PrintSpawnRules || SpawnManager.Settings.PrintItemAttachPoints || SpawnManager.Settings.PrintItemTypeIDs;
        }
      }
    }
  }
}
