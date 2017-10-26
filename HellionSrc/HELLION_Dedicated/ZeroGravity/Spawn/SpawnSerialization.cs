// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Spawn.SpawnSerialization
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using ZeroGravity.Data;
using ZeroGravity.Math;
using ZeroGravity.Objects;

namespace ZeroGravity.Spawn
{
  public static class SpawnSerialization
  {
    private static string GetDirectory()
    {
      return !Server.ConfigDir.IsNullOrEmpty() && Directory.Exists(Server.ConfigDir + "Data") ? Server.ConfigDir : "";
    }

    public static Dictionary<string, Dictionary<LootTier, List<LootItemData>>> LoadLootData()
    {
      List<SpawnSerialization.LootCategoryData> lootCategoryDataList = ZeroGravity.Json.Load<List<SpawnSerialization.LootCategoryData>>(SpawnSerialization.GetDirectory() + "Data/LootCategories.json");
      if (lootCategoryDataList == null || lootCategoryDataList.Count <= 0)
        return (Dictionary<string, Dictionary<LootTier, List<LootItemData>>>) null;
      Dictionary<string, Dictionary<LootTier, List<LootItemData>>> dictionary1 = new Dictionary<string, Dictionary<LootTier, List<LootItemData>>>();
      foreach (SpawnSerialization.LootCategoryData lootCategoryData in lootCategoryDataList)
      {
        if (lootCategoryData.CategoryName.IsNullOrEmpty())
          Dbg.Error("SPAWN MANAGER - Loot category cannot be empty");
        else if (dictionary1.ContainsKey(lootCategoryData.CategoryName))
        {
          Dbg.Error(string.Format("SPAWN MANAGER - Loot category \"{0}\" already exists", (object) lootCategoryData.CategoryName));
        }
        else
        {
          Dictionary<LootTier, List<LootItemData>> dictionary2 = new Dictionary<LootTier, List<LootItemData>>();
          LootTier result = LootTier.T1;
          foreach (SpawnSerialization.LootTierData tier in lootCategoryData.Tiers)
          {
            if (tier.TierName.IsNullOrEmpty() || !Enum.TryParse<LootTier>(tier.TierName, out result))
              Dbg.Error(string.Format("SPAWN MANAGER - Loot category \"{0}\" tier \"{1}\" is not valid", (object) lootCategoryData.CategoryName, (object) tier.TierName));
            else if (dictionary2.ContainsKey(result))
            {
              Dbg.Error(string.Format("SPAWN MANAGER - Loot category \"{0}\" tier \"{1}\" already exists", (object) lootCategoryData.CategoryName, (object) tier.TierName));
            }
            else
            {
              List<LootItemData> lootItemDataList = new List<LootItemData>();
              foreach (SpawnSerialization.LootItemSerData data in tier.Items)
              {
                LootItemData serializationData = SpawnSerialization.CreateLootItemFromSerializationData(lootCategoryData.CategoryName, result, data);
                if (serializationData != null)
                  lootItemDataList.Add(serializationData);
              }
              dictionary2.Add(result, lootItemDataList);
            }
          }
          dictionary1.Add(lootCategoryData.CategoryName, dictionary2);
        }
      }
      return dictionary1;
    }

    private static LootItemData CreateLootItemFromSerializationData(string categoryName, LootTier tier, SpawnSerialization.LootItemSerData data)
    {
      ItemType result1 = ItemType.None;
      GenericItemSubType result2 = GenericItemSubType.None;
      MachineryPartType result3 = MachineryPartType.None;
      if (!Enum.TryParse<ItemType>(data.ItemType, out result1))
      {
        Dbg.Error(string.Format("SPAWN MANAGER - Loot cateory \"{0}\" tier \"{1}\" has unknown item type \"{2}\"", (object) categoryName, (object) tier.ToString(), (object) data.ItemType));
        return (LootItemData) null;
      }
      if (data.Health.HasValue && ((double) data.Health.Value.Min < 0.0 || (double) data.Health.Value.Max <= 0.0 || (double) data.Health.Value.Min > (double) data.Health.Value.Max))
      {
        Dbg.Error(string.Format("SPAWN MANAGER - Loot category \"{0}\" tier \"{1}\" health is not valid (min: {2}, max: {3})", (object) categoryName, (object) tier.ToString(), (object) data.Health.Value.Min, (object) data.Health.Value.Max));
        return (LootItemData) null;
      }
      if (result1 == ItemType.GenericItem && (data.GenericItemSubType.IsNullOrEmpty() || !Enum.TryParse<GenericItemSubType>(data.GenericItemSubType, out result2)))
      {
        Dbg.Error(string.Format("SPAWN MANAGER - Loot cateory \"{0}\" tier \"{1}\" item \"{2}\" has unknown sub type \"{3}\"", (object) categoryName, (object) tier.ToString(), (object) data.ItemType, (object) data.GenericItemSubType));
        return (LootItemData) null;
      }
      if (result1 == ItemType.MachineryPart && (data.MachineryPartType.IsNullOrEmpty() || !Enum.TryParse<MachineryPartType>(data.MachineryPartType, out result3)))
      {
        Dbg.Error(string.Format("SPAWN MANAGER - Loot cateory \"{0}\" tier \"{1}\" item \"{2}\" has unknown sub type \"{3}\"", (object) categoryName, (object) tier.ToString(), (object) data.ItemType, (object) data.MachineryPartType));
        return (LootItemData) null;
      }
      LootItemData lootItemData = new LootItemData();
      lootItemData.Type = result1;
      lootItemData.GenericSubType = result2;
      lootItemData.PartType = result3;
      SpawnRange<float>? health = data.Health;
      lootItemData.Health = health;
      List<string> look = data.Look;
      lootItemData.Look = look;
      SpawnRange<float>? power = data.Power;
      lootItemData.Power = power;
      SpawnRange<int>? count = data.Count;
      lootItemData.Count = count;
      bool? isActive = data.IsActive;
      lootItemData.IsActive = isActive;
      List<LootItemData.CargoResourceData> cargoData = SpawnSerialization.GenerateCargoData(categoryName, tier, data);
      lootItemData.Cargo = cargoData;
      return lootItemData;
    }

    private static List<LootItemData.CargoResourceData> GenerateCargoData(string categoryName, LootTier tier, SpawnSerialization.LootItemSerData data)
    {
      if (data.Cargo == null)
        return (List<LootItemData.CargoResourceData>) null;
      ResourceType result = ResourceType.None;
      List<LootItemData.CargoResourceData> cargoResourceDataList = new List<LootItemData.CargoResourceData>();
      foreach (SpawnSerialization.LootItemSerData.CargoResourceData cargoResourceData1 in data.Cargo)
      {
        if (cargoResourceData1.Resources == null || cargoResourceData1.Resources.Count == 0)
          Dbg.Error(string.Format("SPAWN MANAGER - Loot cateory \"{0}\" tier \"{1}\", cargo has no resource set", (object) categoryName, (object) tier));
        else if ((double) cargoResourceData1.Quantity.Max < 0.0 || (double) cargoResourceData1.Quantity.Min > (double) cargoResourceData1.Quantity.Max)
        {
          Dbg.Error(string.Format("SPAWN MANAGER - Loot cateory \"{0}\" tier \"{1}\", cargo quantity is not valid (min: {2}, max: {3})", (object) categoryName, (object) tier, (object) cargoResourceData1.Quantity.Min, (object) cargoResourceData1.Quantity.Max));
        }
        else
        {
          LootItemData.CargoResourceData cargoResourceData2 = new LootItemData.CargoResourceData()
          {
            Quantity = cargoResourceData1.Quantity,
            Resources = new List<ResourceType>()
          };
          foreach (string resource in cargoResourceData1.Resources)
          {
            if (!Enum.TryParse<ResourceType>(resource, out result))
              Dbg.Error(string.Format("SPAWN MANAGER - Loot cateory \"{0}\" tier \"{1}\", cargo resource type \"{2}\" does not exit", (object) categoryName, (object) tier, (object) resource));
            else
              cargoResourceData2.Resources.Add(result);
          }
          if (cargoResourceData2.Resources.Count > 0)
            cargoResourceDataList.Add(cargoResourceData2);
        }
      }
      if (cargoResourceDataList.Count > 0)
        return cargoResourceDataList;
      return (List<LootItemData.CargoResourceData>) null;
    }

    public static void GenerateLootSampleData(bool force = false)
    {
      string directory = SpawnSerialization.GetDirectory();
      if (!force && File.Exists(directory + "Data/LootCategories.json"))
        return;
      List<SpawnSerialization.LootCategoryData> lootCategoryDataList = new List<SpawnSerialization.LootCategoryData>();
      SpawnSerialization.LootCategoryData lootCategoryData1 = new SpawnSerialization.LootCategoryData();
      lootCategoryData1.CategoryName = "Sample";
      List<SpawnSerialization.LootTierData> lootTierDataList1 = new List<SpawnSerialization.LootTierData>();
      SpawnSerialization.LootTierData lootTierData1 = new SpawnSerialization.LootTierData();
      lootTierData1.TierName = "T1";
      List<SpawnSerialization.LootItemSerData> lootItemSerDataList1 = new List<SpawnSerialization.LootItemSerData>();
      lootItemSerDataList1.Add(new SpawnSerialization.LootItemSerData()
      {
        ItemType = "MilitaryHandGun01"
      });
      SpawnSerialization.LootItemSerData lootItemSerData1 = new SpawnSerialization.LootItemSerData();
      lootItemSerData1.ItemType = "MilitaryHandGunAmmo01";
      SpawnRange<int>? nullable1 = new SpawnRange<int>?(new SpawnRange<int>(10, 20));
      lootItemSerData1.Count = nullable1;
      lootItemSerDataList1.Add(lootItemSerData1);
      SpawnSerialization.LootItemSerData lootItemSerData2 = new SpawnSerialization.LootItemSerData();
      lootItemSerData2.ItemType = "MachineryPart";
      lootItemSerData2.MachineryPartType = "WarpCell";
      SpawnRange<float>? nullable2 = new SpawnRange<float>?(new SpawnRange<float>(22f, 33f));
      lootItemSerData2.Health = nullable2;
      lootItemSerDataList1.Add(lootItemSerData2);
      SpawnSerialization.LootItemSerData lootItemSerData3 = new SpawnSerialization.LootItemSerData();
      lootItemSerData3.ItemType = "AltairRefinedCanister";
      List<SpawnSerialization.LootItemSerData.CargoResourceData> cargoResourceDataList1 = new List<SpawnSerialization.LootItemSerData.CargoResourceData>();
      SpawnSerialization.LootItemSerData.CargoResourceData cargoResourceData1 = new SpawnSerialization.LootItemSerData.CargoResourceData();
      cargoResourceData1.Resources = new List<string>()
      {
        "Oxygen",
        "Nitrogen"
      };
      SpawnRange<float> spawnRange1 = new SpawnRange<float>(54f, 66f);
      cargoResourceData1.Quantity = spawnRange1;
      cargoResourceDataList1.Add(cargoResourceData1);
      lootItemSerData3.Cargo = cargoResourceDataList1;
      lootItemSerDataList1.Add(lootItemSerData3);
      SpawnSerialization.LootItemSerData lootItemSerData4 = new SpawnSerialization.LootItemSerData();
      lootItemSerData4.ItemType = "GenericItem";
      lootItemSerData4.GenericItemSubType = "Poster";
      lootItemSerData4.Look = new List<string>()
      {
        "Hellion",
        "Bethyr",
        "Burner",
        "Turret",
        "CrewQuaters"
      };
      SpawnRange<float>? nullable3 = new SpawnRange<float>?(new SpawnRange<float>(15f, 35f));
      lootItemSerData4.Health = nullable3;
      lootItemSerDataList1.Add(lootItemSerData4);
      SpawnSerialization.LootItemSerData lootItemSerData5 = new SpawnSerialization.LootItemSerData();
      lootItemSerData5.ItemType = "PortableTurret";
      bool? nullable4 = new bool?(true);
      lootItemSerData5.IsActive = nullable4;
      lootItemSerDataList1.Add(lootItemSerData5);
      SpawnSerialization.LootItemSerData lootItemSerData6 = new SpawnSerialization.LootItemSerData();
      lootItemSerData6.ItemType = "AltairPressurisedJetpack";
      SpawnRange<float>? nullable5 = new SpawnRange<float>?(new SpawnRange<float>(100f, 100f));
      lootItemSerData6.Power = nullable5;
      lootItemSerDataList1.Add(lootItemSerData6);
      lootTierData1.Items = lootItemSerDataList1;
      lootTierDataList1.Add(lootTierData1);
      lootCategoryData1.Tiers = lootTierDataList1;
      lootCategoryDataList.Add(lootCategoryData1);
      lootCategoryDataList.Add(new SpawnSerialization.LootCategoryData()
      {
        CategoryName = "Weapons",
        Tiers = new List<SpawnSerialization.LootTierData>()
        {
          new SpawnSerialization.LootTierData()
          {
            TierName = "T1",
            Items = new List<SpawnSerialization.LootItemSerData>()
            {
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "MilitaryHandGun01"
              }
            }
          },
          new SpawnSerialization.LootTierData()
          {
            TierName = "T2",
            Items = new List<SpawnSerialization.LootItemSerData>()
            {
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "AltairRifle"
              },
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "MilitaryHandGun02"
              },
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "APGrenade"
              }
            }
          },
          new SpawnSerialization.LootTierData()
          {
            TierName = "T3",
            Items = new List<SpawnSerialization.LootItemSerData>()
            {
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "MilitaryAssaultRifle"
              },
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "MilitarySniperRifle"
              },
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "APGrenade"
              },
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "EMPGrenade"
              }
            }
          }
        }
      });
      lootCategoryDataList.Add(new SpawnSerialization.LootCategoryData()
      {
        CategoryName = "Ammo",
        Tiers = new List<SpawnSerialization.LootTierData>()
        {
          new SpawnSerialization.LootTierData()
          {
            TierName = "T1",
            Items = new List<SpawnSerialization.LootItemSerData>()
            {
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "MilitaryHandGunAmmo01"
              }
            }
          },
          new SpawnSerialization.LootTierData()
          {
            TierName = "T2",
            Items = new List<SpawnSerialization.LootItemSerData>()
            {
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "MilitaryHandGunAmmo01"
              },
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "AltairRifleAmmo"
              },
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "MilitaryHandGunAmmo02"
              }
            }
          },
          new SpawnSerialization.LootTierData()
          {
            TierName = "T3",
            Items = new List<SpawnSerialization.LootItemSerData>()
            {
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "AltairRifleAmmo"
              },
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "MilitaryHandGunAmmo02"
              },
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "MilitaryAssaultRifleAmmo"
              },
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "MilitarySniperRifleAmmo"
              }
            }
          }
        }
      });
      SpawnSerialization.LootCategoryData lootCategoryData2 = new SpawnSerialization.LootCategoryData();
      lootCategoryData2.CategoryName = "Parts";
      List<SpawnSerialization.LootTierData> lootTierDataList2 = new List<SpawnSerialization.LootTierData>();
      SpawnSerialization.LootTierData lootTierData2 = new SpawnSerialization.LootTierData();
      lootTierData2.TierName = "T1";
      List<SpawnSerialization.LootItemSerData> lootItemSerDataList2 = new List<SpawnSerialization.LootItemSerData>();
      SpawnSerialization.LootItemSerData lootItemSerData7 = new SpawnSerialization.LootItemSerData();
      lootItemSerData7.ItemType = "MachineryPart";
      lootItemSerData7.MachineryPartType = "ServoMotor";
      SpawnRange<float>? nullable6 = new SpawnRange<float>?(new SpawnRange<float>(45f, 55f));
      lootItemSerData7.Health = nullable6;
      lootItemSerDataList2.Add(lootItemSerData7);
      SpawnSerialization.LootItemSerData lootItemSerData8 = new SpawnSerialization.LootItemSerData();
      lootItemSerData8.ItemType = "MachineryPart";
      lootItemSerData8.MachineryPartType = "AirProcessingController";
      SpawnRange<float>? nullable7 = new SpawnRange<float>?(new SpawnRange<float>(45f, 55f));
      lootItemSerData8.Health = nullable7;
      lootItemSerDataList2.Add(lootItemSerData8);
      SpawnSerialization.LootItemSerData lootItemSerData9 = new SpawnSerialization.LootItemSerData();
      lootItemSerData9.ItemType = "MachineryPart";
      lootItemSerData9.MachineryPartType = "CarbonFilters";
      SpawnRange<float>? nullable8 = new SpawnRange<float>?(new SpawnRange<float>(45f, 55f));
      lootItemSerData9.Health = nullable8;
      lootItemSerDataList2.Add(lootItemSerData9);
      SpawnSerialization.LootItemSerData lootItemSerData10 = new SpawnSerialization.LootItemSerData();
      lootItemSerData10.ItemType = "MachineryPart";
      lootItemSerData10.MachineryPartType = "AirFilterUnit";
      SpawnRange<float>? nullable9 = new SpawnRange<float>?(new SpawnRange<float>(45f, 55f));
      lootItemSerData10.Health = nullable9;
      lootItemSerDataList2.Add(lootItemSerData10);
      SpawnSerialization.LootItemSerData lootItemSerData11 = new SpawnSerialization.LootItemSerData();
      lootItemSerData11.ItemType = "MachineryPart";
      lootItemSerData11.MachineryPartType = "PressureRegulator";
      SpawnRange<float>? nullable10 = new SpawnRange<float>?(new SpawnRange<float>(45f, 55f));
      lootItemSerData11.Health = nullable10;
      lootItemSerDataList2.Add(lootItemSerData11);
      SpawnSerialization.LootItemSerData lootItemSerData12 = new SpawnSerialization.LootItemSerData();
      lootItemSerData12.ItemType = "MachineryPart";
      lootItemSerData12.MachineryPartType = "CoreContainmentFieldGenerator";
      SpawnRange<float>? nullable11 = new SpawnRange<float>?(new SpawnRange<float>(45f, 55f));
      lootItemSerData12.Health = nullable11;
      lootItemSerDataList2.Add(lootItemSerData12);
      SpawnSerialization.LootItemSerData lootItemSerData13 = new SpawnSerialization.LootItemSerData();
      lootItemSerData13.ItemType = "MachineryPart";
      lootItemSerData13.MachineryPartType = "ResourceInjector";
      SpawnRange<float>? nullable12 = new SpawnRange<float>?(new SpawnRange<float>(45f, 55f));
      lootItemSerData13.Health = nullable12;
      lootItemSerDataList2.Add(lootItemSerData13);
      SpawnSerialization.LootItemSerData lootItemSerData14 = new SpawnSerialization.LootItemSerData();
      lootItemSerData14.ItemType = "MachineryPart";
      lootItemSerData14.MachineryPartType = "EMFieldController";
      SpawnRange<float>? nullable13 = new SpawnRange<float>?(new SpawnRange<float>(45f, 55f));
      lootItemSerData14.Health = nullable13;
      lootItemSerDataList2.Add(lootItemSerData14);
      SpawnSerialization.LootItemSerData lootItemSerData15 = new SpawnSerialization.LootItemSerData();
      lootItemSerData15.ItemType = "MachineryPart";
      lootItemSerData15.MachineryPartType = "ThermonuclearCatalyst";
      SpawnRange<float>? nullable14 = new SpawnRange<float>?(new SpawnRange<float>(45f, 55f));
      lootItemSerData15.Health = nullable14;
      lootItemSerDataList2.Add(lootItemSerData15);
      SpawnSerialization.LootItemSerData lootItemSerData16 = new SpawnSerialization.LootItemSerData();
      lootItemSerData16.ItemType = "MachineryPart";
      lootItemSerData16.MachineryPartType = "PlasmaAccelerator";
      SpawnRange<float>? nullable15 = new SpawnRange<float>?(new SpawnRange<float>(45f, 55f));
      lootItemSerData16.Health = nullable15;
      lootItemSerDataList2.Add(lootItemSerData16);
      SpawnSerialization.LootItemSerData lootItemSerData17 = new SpawnSerialization.LootItemSerData();
      lootItemSerData17.ItemType = "MachineryPart";
      lootItemSerData17.MachineryPartType = "HighEnergyLaser";
      SpawnRange<float>? nullable16 = new SpawnRange<float>?(new SpawnRange<float>(45f, 55f));
      lootItemSerData17.Health = nullable16;
      lootItemSerDataList2.Add(lootItemSerData17);
      SpawnSerialization.LootItemSerData lootItemSerData18 = new SpawnSerialization.LootItemSerData();
      lootItemSerData18.ItemType = "MachineryPart";
      lootItemSerData18.MachineryPartType = "SingularityCellDetonator";
      SpawnRange<float>? nullable17 = new SpawnRange<float>?(new SpawnRange<float>(45f, 55f));
      lootItemSerData18.Health = nullable17;
      lootItemSerDataList2.Add(lootItemSerData18);
      lootTierData2.Items = lootItemSerDataList2;
      lootTierDataList2.Add(lootTierData2);
      SpawnSerialization.LootTierData lootTierData3 = new SpawnSerialization.LootTierData();
      lootTierData3.TierName = "T2";
      List<SpawnSerialization.LootItemSerData> lootItemSerDataList3 = new List<SpawnSerialization.LootItemSerData>();
      SpawnSerialization.LootItemSerData lootItemSerData19 = new SpawnSerialization.LootItemSerData();
      lootItemSerData19.ItemType = "MachineryPart";
      lootItemSerData19.MachineryPartType = "ServoMotor";
      SpawnRange<float>? nullable18 = new SpawnRange<float>?(new SpawnRange<float>(65f, 75f));
      lootItemSerData19.Health = nullable18;
      lootItemSerDataList3.Add(lootItemSerData19);
      SpawnSerialization.LootItemSerData lootItemSerData20 = new SpawnSerialization.LootItemSerData();
      lootItemSerData20.ItemType = "MachineryPart";
      lootItemSerData20.MachineryPartType = "AirProcessingController";
      SpawnRange<float>? nullable19 = new SpawnRange<float>?(new SpawnRange<float>(65f, 75f));
      lootItemSerData20.Health = nullable19;
      lootItemSerDataList3.Add(lootItemSerData20);
      SpawnSerialization.LootItemSerData lootItemSerData21 = new SpawnSerialization.LootItemSerData();
      lootItemSerData21.ItemType = "MachineryPart";
      lootItemSerData21.MachineryPartType = "CarbonFilters";
      SpawnRange<float>? nullable20 = new SpawnRange<float>?(new SpawnRange<float>(65f, 75f));
      lootItemSerData21.Health = nullable20;
      lootItemSerDataList3.Add(lootItemSerData21);
      SpawnSerialization.LootItemSerData lootItemSerData22 = new SpawnSerialization.LootItemSerData();
      lootItemSerData22.ItemType = "MachineryPart";
      lootItemSerData22.MachineryPartType = "AirFilterUnit";
      SpawnRange<float>? nullable21 = new SpawnRange<float>?(new SpawnRange<float>(65f, 75f));
      lootItemSerData22.Health = nullable21;
      lootItemSerDataList3.Add(lootItemSerData22);
      SpawnSerialization.LootItemSerData lootItemSerData23 = new SpawnSerialization.LootItemSerData();
      lootItemSerData23.ItemType = "MachineryPart";
      lootItemSerData23.MachineryPartType = "PressureRegulator";
      SpawnRange<float>? nullable22 = new SpawnRange<float>?(new SpawnRange<float>(65f, 75f));
      lootItemSerData23.Health = nullable22;
      lootItemSerDataList3.Add(lootItemSerData23);
      SpawnSerialization.LootItemSerData lootItemSerData24 = new SpawnSerialization.LootItemSerData();
      lootItemSerData24.ItemType = "MachineryPart";
      lootItemSerData24.MachineryPartType = "CoreContainmentFieldGenerator";
      SpawnRange<float>? nullable23 = new SpawnRange<float>?(new SpawnRange<float>(65f, 75f));
      lootItemSerData24.Health = nullable23;
      lootItemSerDataList3.Add(lootItemSerData24);
      SpawnSerialization.LootItemSerData lootItemSerData25 = new SpawnSerialization.LootItemSerData();
      lootItemSerData25.ItemType = "MachineryPart";
      lootItemSerData25.MachineryPartType = "ResourceInjector";
      SpawnRange<float>? nullable24 = new SpawnRange<float>?(new SpawnRange<float>(65f, 75f));
      lootItemSerData25.Health = nullable24;
      lootItemSerDataList3.Add(lootItemSerData25);
      SpawnSerialization.LootItemSerData lootItemSerData26 = new SpawnSerialization.LootItemSerData();
      lootItemSerData26.ItemType = "MachineryPart";
      lootItemSerData26.MachineryPartType = "EMFieldController";
      SpawnRange<float>? nullable25 = new SpawnRange<float>?(new SpawnRange<float>(65f, 75f));
      lootItemSerData26.Health = nullable25;
      lootItemSerDataList3.Add(lootItemSerData26);
      SpawnSerialization.LootItemSerData lootItemSerData27 = new SpawnSerialization.LootItemSerData();
      lootItemSerData27.ItemType = "MachineryPart";
      lootItemSerData27.MachineryPartType = "ThermonuclearCatalyst";
      SpawnRange<float>? nullable26 = new SpawnRange<float>?(new SpawnRange<float>(65f, 75f));
      lootItemSerData27.Health = nullable26;
      lootItemSerDataList3.Add(lootItemSerData27);
      SpawnSerialization.LootItemSerData lootItemSerData28 = new SpawnSerialization.LootItemSerData();
      lootItemSerData28.ItemType = "MachineryPart";
      lootItemSerData28.MachineryPartType = "PlasmaAccelerator";
      SpawnRange<float>? nullable27 = new SpawnRange<float>?(new SpawnRange<float>(65f, 75f));
      lootItemSerData28.Health = nullable27;
      lootItemSerDataList3.Add(lootItemSerData28);
      SpawnSerialization.LootItemSerData lootItemSerData29 = new SpawnSerialization.LootItemSerData();
      lootItemSerData29.ItemType = "MachineryPart";
      lootItemSerData29.MachineryPartType = "HighEnergyLaser";
      SpawnRange<float>? nullable28 = new SpawnRange<float>?(new SpawnRange<float>(65f, 75f));
      lootItemSerData29.Health = nullable28;
      lootItemSerDataList3.Add(lootItemSerData29);
      SpawnSerialization.LootItemSerData lootItemSerData30 = new SpawnSerialization.LootItemSerData();
      lootItemSerData30.ItemType = "MachineryPart";
      lootItemSerData30.MachineryPartType = "SingularityCellDetonator";
      SpawnRange<float>? nullable29 = new SpawnRange<float>?(new SpawnRange<float>(65f, 75f));
      lootItemSerData30.Health = nullable29;
      lootItemSerDataList3.Add(lootItemSerData30);
      lootTierData3.Items = lootItemSerDataList3;
      lootTierDataList2.Add(lootTierData3);
      SpawnSerialization.LootTierData lootTierData4 = new SpawnSerialization.LootTierData();
      lootTierData4.TierName = "T3";
      List<SpawnSerialization.LootItemSerData> lootItemSerDataList4 = new List<SpawnSerialization.LootItemSerData>();
      SpawnSerialization.LootItemSerData lootItemSerData31 = new SpawnSerialization.LootItemSerData();
      lootItemSerData31.ItemType = "MachineryPart";
      lootItemSerData31.MachineryPartType = "ServoMotor";
      SpawnRange<float>? nullable30 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData31.Health = nullable30;
      lootItemSerDataList4.Add(lootItemSerData31);
      SpawnSerialization.LootItemSerData lootItemSerData32 = new SpawnSerialization.LootItemSerData();
      lootItemSerData32.ItemType = "MachineryPart";
      lootItemSerData32.MachineryPartType = "AirProcessingController";
      SpawnRange<float>? nullable31 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData32.Health = nullable31;
      lootItemSerDataList4.Add(lootItemSerData32);
      SpawnSerialization.LootItemSerData lootItemSerData33 = new SpawnSerialization.LootItemSerData();
      lootItemSerData33.ItemType = "MachineryPart";
      lootItemSerData33.MachineryPartType = "CarbonFilters";
      SpawnRange<float>? nullable32 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData33.Health = nullable32;
      lootItemSerDataList4.Add(lootItemSerData33);
      SpawnSerialization.LootItemSerData lootItemSerData34 = new SpawnSerialization.LootItemSerData();
      lootItemSerData34.ItemType = "MachineryPart";
      lootItemSerData34.MachineryPartType = "AirFilterUnit";
      SpawnRange<float>? nullable33 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData34.Health = nullable33;
      lootItemSerDataList4.Add(lootItemSerData34);
      SpawnSerialization.LootItemSerData lootItemSerData35 = new SpawnSerialization.LootItemSerData();
      lootItemSerData35.ItemType = "MachineryPart";
      lootItemSerData35.MachineryPartType = "PressureRegulator";
      SpawnRange<float>? nullable34 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData35.Health = nullable34;
      lootItemSerDataList4.Add(lootItemSerData35);
      SpawnSerialization.LootItemSerData lootItemSerData36 = new SpawnSerialization.LootItemSerData();
      lootItemSerData36.ItemType = "MachineryPart";
      lootItemSerData36.MachineryPartType = "CoreContainmentFieldGenerator";
      SpawnRange<float>? nullable35 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData36.Health = nullable35;
      lootItemSerDataList4.Add(lootItemSerData36);
      SpawnSerialization.LootItemSerData lootItemSerData37 = new SpawnSerialization.LootItemSerData();
      lootItemSerData37.ItemType = "MachineryPart";
      lootItemSerData37.MachineryPartType = "ResourceInjector";
      SpawnRange<float>? nullable36 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData37.Health = nullable36;
      lootItemSerDataList4.Add(lootItemSerData37);
      SpawnSerialization.LootItemSerData lootItemSerData38 = new SpawnSerialization.LootItemSerData();
      lootItemSerData38.ItemType = "MachineryPart";
      lootItemSerData38.MachineryPartType = "EMFieldController";
      SpawnRange<float>? nullable37 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData38.Health = nullable37;
      lootItemSerDataList4.Add(lootItemSerData38);
      SpawnSerialization.LootItemSerData lootItemSerData39 = new SpawnSerialization.LootItemSerData();
      lootItemSerData39.ItemType = "MachineryPart";
      lootItemSerData39.MachineryPartType = "ThermonuclearCatalyst";
      SpawnRange<float>? nullable38 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData39.Health = nullable38;
      lootItemSerDataList4.Add(lootItemSerData39);
      SpawnSerialization.LootItemSerData lootItemSerData40 = new SpawnSerialization.LootItemSerData();
      lootItemSerData40.ItemType = "MachineryPart";
      lootItemSerData40.MachineryPartType = "PlasmaAccelerator";
      SpawnRange<float>? nullable39 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData40.Health = nullable39;
      lootItemSerDataList4.Add(lootItemSerData40);
      SpawnSerialization.LootItemSerData lootItemSerData41 = new SpawnSerialization.LootItemSerData();
      lootItemSerData41.ItemType = "MachineryPart";
      lootItemSerData41.MachineryPartType = "HighEnergyLaser";
      SpawnRange<float>? nullable40 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData41.Health = nullable40;
      lootItemSerDataList4.Add(lootItemSerData41);
      SpawnSerialization.LootItemSerData lootItemSerData42 = new SpawnSerialization.LootItemSerData();
      lootItemSerData42.ItemType = "MachineryPart";
      lootItemSerData42.MachineryPartType = "SingularityCellDetonator";
      SpawnRange<float>? nullable41 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData42.Health = nullable41;
      lootItemSerDataList4.Add(lootItemSerData42);
      lootTierData4.Items = lootItemSerDataList4;
      lootTierDataList2.Add(lootTierData4);
      lootCategoryData2.Tiers = lootTierDataList2;
      lootCategoryDataList.Add(lootCategoryData2);
      SpawnSerialization.LootCategoryData lootCategoryData3 = new SpawnSerialization.LootCategoryData();
      lootCategoryData3.CategoryName = "Fuel";
      List<SpawnSerialization.LootTierData> lootTierDataList3 = new List<SpawnSerialization.LootTierData>();
      SpawnSerialization.LootTierData lootTierData5 = new SpawnSerialization.LootTierData();
      lootTierData5.TierName = "T1";
      List<SpawnSerialization.LootItemSerData> lootItemSerDataList5 = new List<SpawnSerialization.LootItemSerData>();
      SpawnSerialization.LootItemSerData lootItemSerData43 = new SpawnSerialization.LootItemSerData();
      lootItemSerData43.ItemType = "AltairResourceContainer";
      List<SpawnSerialization.LootItemSerData.CargoResourceData> cargoResourceDataList2 = new List<SpawnSerialization.LootItemSerData.CargoResourceData>();
      SpawnSerialization.LootItemSerData.CargoResourceData cargoResourceData2 = new SpawnSerialization.LootItemSerData.CargoResourceData();
      cargoResourceData2.Resources = new List<string>()
      {
        "Nitro"
      };
      SpawnRange<float> spawnRange2 = new SpawnRange<float>(33f, 100f);
      cargoResourceData2.Quantity = spawnRange2;
      cargoResourceDataList2.Add(cargoResourceData2);
      lootItemSerData43.Cargo = cargoResourceDataList2;
      lootItemSerDataList5.Add(lootItemSerData43);
      lootTierData5.Items = lootItemSerDataList5;
      lootTierDataList3.Add(lootTierData5);
      SpawnSerialization.LootTierData lootTierData6 = new SpawnSerialization.LootTierData();
      lootTierData6.TierName = "T2";
      List<SpawnSerialization.LootItemSerData> lootItemSerDataList6 = new List<SpawnSerialization.LootItemSerData>();
      SpawnSerialization.LootItemSerData lootItemSerData44 = new SpawnSerialization.LootItemSerData();
      lootItemSerData44.ItemType = "MachineryPart";
      lootItemSerData44.MachineryPartType = "WarpCell";
      SpawnRange<float>? nullable42 = new SpawnRange<float>?(new SpawnRange<float>(22f, 33f));
      lootItemSerData44.Health = nullable42;
      lootItemSerDataList6.Add(lootItemSerData44);
      SpawnSerialization.LootItemSerData lootItemSerData45 = new SpawnSerialization.LootItemSerData();
      lootItemSerData45.ItemType = "AltairRefinedCanister";
      List<SpawnSerialization.LootItemSerData.CargoResourceData> cargoResourceDataList3 = new List<SpawnSerialization.LootItemSerData.CargoResourceData>();
      SpawnSerialization.LootItemSerData.CargoResourceData cargoResourceData3 = new SpawnSerialization.LootItemSerData.CargoResourceData();
      cargoResourceData3.Resources = new List<string>()
      {
        "Nitro",
        "Hydrogen"
      };
      SpawnRange<float> spawnRange3 = new SpawnRange<float>(22f, 33f);
      cargoResourceData3.Quantity = spawnRange3;
      cargoResourceDataList3.Add(cargoResourceData3);
      lootItemSerData45.Cargo = cargoResourceDataList3;
      lootItemSerDataList6.Add(lootItemSerData45);
      lootTierData6.Items = lootItemSerDataList6;
      lootTierDataList3.Add(lootTierData6);
      SpawnSerialization.LootTierData lootTierData7 = new SpawnSerialization.LootTierData();
      lootTierData7.TierName = "T3";
      List<SpawnSerialization.LootItemSerData> lootItemSerDataList7 = new List<SpawnSerialization.LootItemSerData>();
      SpawnSerialization.LootItemSerData lootItemSerData46 = new SpawnSerialization.LootItemSerData();
      lootItemSerData46.ItemType = "MachineryPart";
      lootItemSerData46.MachineryPartType = "WarpCell";
      SpawnRange<float>? nullable43 = new SpawnRange<float>?(new SpawnRange<float>(54f, 66f));
      lootItemSerData46.Health = nullable43;
      lootItemSerDataList7.Add(lootItemSerData46);
      SpawnSerialization.LootItemSerData lootItemSerData47 = new SpawnSerialization.LootItemSerData();
      lootItemSerData47.ItemType = "AltairRefinedCanister";
      List<SpawnSerialization.LootItemSerData.CargoResourceData> cargoResourceDataList4 = new List<SpawnSerialization.LootItemSerData.CargoResourceData>();
      SpawnSerialization.LootItemSerData.CargoResourceData cargoResourceData4 = new SpawnSerialization.LootItemSerData.CargoResourceData();
      cargoResourceData4.Resources = new List<string>()
      {
        "Nitro",
        "Hydrogen",
        "Deuterium"
      };
      SpawnRange<float> spawnRange4 = new SpawnRange<float>(54f, 66f);
      cargoResourceData4.Quantity = spawnRange4;
      cargoResourceDataList4.Add(cargoResourceData4);
      lootItemSerData47.Cargo = cargoResourceDataList4;
      lootItemSerDataList7.Add(lootItemSerData47);
      lootTierData7.Items = lootItemSerDataList7;
      lootTierDataList3.Add(lootTierData7);
      SpawnSerialization.LootTierData lootTierData8 = new SpawnSerialization.LootTierData();
      lootTierData8.TierName = "T4";
      List<SpawnSerialization.LootItemSerData> lootItemSerDataList8 = new List<SpawnSerialization.LootItemSerData>();
      SpawnSerialization.LootItemSerData lootItemSerData48 = new SpawnSerialization.LootItemSerData();
      lootItemSerData48.ItemType = "MachineryPart";
      lootItemSerData48.MachineryPartType = "WarpCell";
      SpawnRange<float>? nullable44 = new SpawnRange<float>?(new SpawnRange<float>(86f, 100f));
      lootItemSerData48.Health = nullable44;
      lootItemSerDataList8.Add(lootItemSerData48);
      SpawnSerialization.LootItemSerData lootItemSerData49 = new SpawnSerialization.LootItemSerData();
      lootItemSerData49.ItemType = "AltairRefinedCanister";
      List<SpawnSerialization.LootItemSerData.CargoResourceData> cargoResourceDataList5 = new List<SpawnSerialization.LootItemSerData.CargoResourceData>();
      SpawnSerialization.LootItemSerData.CargoResourceData cargoResourceData5 = new SpawnSerialization.LootItemSerData.CargoResourceData();
      cargoResourceData5.Resources = new List<string>()
      {
        "Nitro",
        "Hydrogen",
        "Deuterium"
      };
      SpawnRange<float> spawnRange5 = new SpawnRange<float>(86f, 100f);
      cargoResourceData5.Quantity = spawnRange5;
      cargoResourceDataList5.Add(cargoResourceData5);
      lootItemSerData49.Cargo = cargoResourceDataList5;
      lootItemSerDataList8.Add(lootItemSerData49);
      lootTierData8.Items = lootItemSerDataList8;
      lootTierDataList3.Add(lootTierData8);
      lootCategoryData3.Tiers = lootTierDataList3;
      lootCategoryDataList.Add(lootCategoryData3);
      SpawnSerialization.LootCategoryData lootCategoryData4 = new SpawnSerialization.LootCategoryData();
      lootCategoryData4.CategoryName = "Resources";
      List<SpawnSerialization.LootTierData> lootTierDataList4 = new List<SpawnSerialization.LootTierData>();
      SpawnSerialization.LootTierData lootTierData9 = new SpawnSerialization.LootTierData();
      lootTierData9.TierName = "T1";
      List<SpawnSerialization.LootItemSerData> lootItemSerDataList9 = new List<SpawnSerialization.LootItemSerData>();
      SpawnSerialization.LootItemSerData lootItemSerData50 = new SpawnSerialization.LootItemSerData();
      lootItemSerData50.ItemType = "AltairResourceContainer";
      List<SpawnSerialization.LootItemSerData.CargoResourceData> cargoResourceDataList6 = new List<SpawnSerialization.LootItemSerData.CargoResourceData>();
      SpawnSerialization.LootItemSerData.CargoResourceData cargoResourceData6 = new SpawnSerialization.LootItemSerData.CargoResourceData();
      cargoResourceData6.Resources = new List<string>()
      {
        "Oxygen"
      };
      SpawnRange<float> spawnRange6 = new SpawnRange<float>(33f, 100f);
      cargoResourceData6.Quantity = spawnRange6;
      cargoResourceDataList6.Add(cargoResourceData6);
      lootItemSerData50.Cargo = cargoResourceDataList6;
      lootItemSerDataList9.Add(lootItemSerData50);
      lootTierData9.Items = lootItemSerDataList9;
      lootTierDataList4.Add(lootTierData9);
      SpawnSerialization.LootTierData lootTierData10 = new SpawnSerialization.LootTierData();
      lootTierData10.TierName = "T2";
      List<SpawnSerialization.LootItemSerData> lootItemSerDataList10 = new List<SpawnSerialization.LootItemSerData>();
      SpawnSerialization.LootItemSerData lootItemSerData51 = new SpawnSerialization.LootItemSerData();
      lootItemSerData51.ItemType = "AltairRefinedCanister";
      List<SpawnSerialization.LootItemSerData.CargoResourceData> cargoResourceDataList7 = new List<SpawnSerialization.LootItemSerData.CargoResourceData>();
      SpawnSerialization.LootItemSerData.CargoResourceData cargoResourceData7 = new SpawnSerialization.LootItemSerData.CargoResourceData();
      cargoResourceData7.Resources = new List<string>()
      {
        "Oxygen"
      };
      SpawnRange<float> spawnRange7 = new SpawnRange<float>(22f, 33f);
      cargoResourceData7.Quantity = spawnRange7;
      cargoResourceDataList7.Add(cargoResourceData7);
      lootItemSerData51.Cargo = cargoResourceDataList7;
      lootItemSerDataList10.Add(lootItemSerData51);
      SpawnSerialization.LootItemSerData lootItemSerData52 = new SpawnSerialization.LootItemSerData();
      lootItemSerData52.ItemType = "AltairHandDrillCanister";
      List<SpawnSerialization.LootItemSerData.CargoResourceData> cargoResourceDataList8 = new List<SpawnSerialization.LootItemSerData.CargoResourceData>();
      SpawnSerialization.LootItemSerData.CargoResourceData cargoResourceData8 = new SpawnSerialization.LootItemSerData.CargoResourceData();
      cargoResourceData8.Resources = new List<string>()
      {
        "Ice",
        "DryIce"
      };
      SpawnRange<float> spawnRange8 = new SpawnRange<float>(22f, 33f);
      cargoResourceData8.Quantity = spawnRange8;
      cargoResourceDataList8.Add(cargoResourceData8);
      lootItemSerData52.Cargo = cargoResourceDataList8;
      lootItemSerDataList10.Add(lootItemSerData52);
      lootTierData10.Items = lootItemSerDataList10;
      lootTierDataList4.Add(lootTierData10);
      SpawnSerialization.LootTierData lootTierData11 = new SpawnSerialization.LootTierData();
      lootTierData11.TierName = "T3";
      List<SpawnSerialization.LootItemSerData> lootItemSerDataList11 = new List<SpawnSerialization.LootItemSerData>();
      SpawnSerialization.LootItemSerData lootItemSerData53 = new SpawnSerialization.LootItemSerData();
      lootItemSerData53.ItemType = "AltairRefinedCanister";
      List<SpawnSerialization.LootItemSerData.CargoResourceData> cargoResourceDataList9 = new List<SpawnSerialization.LootItemSerData.CargoResourceData>();
      SpawnSerialization.LootItemSerData.CargoResourceData cargoResourceData9 = new SpawnSerialization.LootItemSerData.CargoResourceData();
      cargoResourceData9.Resources = new List<string>()
      {
        "Oxygen",
        "Nitrogen"
      };
      SpawnRange<float> spawnRange9 = new SpawnRange<float>(54f, 66f);
      cargoResourceData9.Quantity = spawnRange9;
      cargoResourceDataList9.Add(cargoResourceData9);
      lootItemSerData53.Cargo = cargoResourceDataList9;
      lootItemSerDataList11.Add(lootItemSerData53);
      SpawnSerialization.LootItemSerData lootItemSerData54 = new SpawnSerialization.LootItemSerData();
      lootItemSerData54.ItemType = "AltairHandDrillCanister";
      List<SpawnSerialization.LootItemSerData.CargoResourceData> cargoResourceDataList10 = new List<SpawnSerialization.LootItemSerData.CargoResourceData>();
      SpawnSerialization.LootItemSerData.CargoResourceData cargoResourceData10 = new SpawnSerialization.LootItemSerData.CargoResourceData();
      cargoResourceData10.Resources = new List<string>()
      {
        "Ice",
        "DryIce",
        "NitrateMinerals"
      };
      SpawnRange<float> spawnRange10 = new SpawnRange<float>(54f, 66f);
      cargoResourceData10.Quantity = spawnRange10;
      cargoResourceDataList10.Add(cargoResourceData10);
      lootItemSerData54.Cargo = cargoResourceDataList10;
      lootItemSerDataList11.Add(lootItemSerData54);
      lootTierData11.Items = lootItemSerDataList11;
      lootTierDataList4.Add(lootTierData11);
      SpawnSerialization.LootTierData lootTierData12 = new SpawnSerialization.LootTierData();
      lootTierData12.TierName = "T4";
      List<SpawnSerialization.LootItemSerData> lootItemSerDataList12 = new List<SpawnSerialization.LootItemSerData>();
      SpawnSerialization.LootItemSerData lootItemSerData55 = new SpawnSerialization.LootItemSerData();
      lootItemSerData55.ItemType = "AltairRefinedCanister";
      List<SpawnSerialization.LootItemSerData.CargoResourceData> cargoResourceDataList11 = new List<SpawnSerialization.LootItemSerData.CargoResourceData>();
      SpawnSerialization.LootItemSerData.CargoResourceData cargoResourceData11 = new SpawnSerialization.LootItemSerData.CargoResourceData();
      cargoResourceData11.Resources = new List<string>()
      {
        "Oxygen",
        "Nitrogen"
      };
      SpawnRange<float> spawnRange11 = new SpawnRange<float>(86f, 100f);
      cargoResourceData11.Quantity = spawnRange11;
      cargoResourceDataList11.Add(cargoResourceData11);
      lootItemSerData55.Cargo = cargoResourceDataList11;
      lootItemSerDataList12.Add(lootItemSerData55);
      SpawnSerialization.LootItemSerData lootItemSerData56 = new SpawnSerialization.LootItemSerData();
      lootItemSerData56.ItemType = "AltairHandDrillCanister";
      List<SpawnSerialization.LootItemSerData.CargoResourceData> cargoResourceDataList12 = new List<SpawnSerialization.LootItemSerData.CargoResourceData>();
      SpawnSerialization.LootItemSerData.CargoResourceData cargoResourceData12 = new SpawnSerialization.LootItemSerData.CargoResourceData();
      cargoResourceData12.Resources = new List<string>()
      {
        "Ice",
        "DryIce",
        "NitrateMinerals",
        "HeavyIce"
      };
      SpawnRange<float> spawnRange12 = new SpawnRange<float>(86f, 100f);
      cargoResourceData12.Quantity = spawnRange12;
      cargoResourceDataList12.Add(cargoResourceData12);
      lootItemSerData56.Cargo = cargoResourceDataList12;
      lootItemSerDataList12.Add(lootItemSerData56);
      lootTierData12.Items = lootItemSerDataList12;
      lootTierDataList4.Add(lootTierData12);
      lootCategoryData4.Tiers = lootTierDataList4;
      lootCategoryDataList.Add(lootCategoryData4);
      SpawnSerialization.LootCategoryData lootCategoryData5 = new SpawnSerialization.LootCategoryData();
      lootCategoryData5.CategoryName = "Vanity";
      List<SpawnSerialization.LootTierData> lootTierDataList5 = new List<SpawnSerialization.LootTierData>();
      SpawnSerialization.LootTierData lootTierData13 = new SpawnSerialization.LootTierData();
      lootTierData13.TierName = "T1";
      List<SpawnSerialization.LootItemSerData> lootItemSerDataList13 = new List<SpawnSerialization.LootItemSerData>();
      SpawnSerialization.LootItemSerData lootItemSerData57 = new SpawnSerialization.LootItemSerData();
      lootItemSerData57.ItemType = "GenericItem";
      lootItemSerData57.GenericItemSubType = "Poster";
      lootItemSerData57.Look = new List<string>()
      {
        "Hellion",
        "Bethyr",
        "Burner",
        "Turret",
        "CrewQuaters"
      };
      SpawnRange<float>? nullable45 = new SpawnRange<float>?(new SpawnRange<float>(15f, 35f));
      lootItemSerData57.Health = nullable45;
      lootItemSerDataList13.Add(lootItemSerData57);
      SpawnSerialization.LootItemSerData lootItemSerData58 = new SpawnSerialization.LootItemSerData();
      lootItemSerData58.ItemType = "GenericItem";
      lootItemSerData58.GenericItemSubType = "BasketBall";
      SpawnRange<float>? nullable46 = new SpawnRange<float>?(new SpawnRange<float>(15f, 35f));
      lootItemSerData58.Health = nullable46;
      lootItemSerDataList13.Add(lootItemSerData58);
      SpawnSerialization.LootItemSerData lootItemSerData59 = new SpawnSerialization.LootItemSerData();
      lootItemSerData59.ItemType = "GenericItem";
      lootItemSerData59.GenericItemSubType = "Hoop";
      SpawnRange<float>? nullable47 = new SpawnRange<float>?(new SpawnRange<float>(15f, 35f));
      lootItemSerData59.Health = nullable47;
      lootItemSerDataList13.Add(lootItemSerData59);
      SpawnSerialization.LootItemSerData lootItemSerData60 = new SpawnSerialization.LootItemSerData();
      lootItemSerData60.ItemType = "GenericItem";
      lootItemSerData60.GenericItemSubType = "LavaLamp";
      SpawnRange<float>? nullable48 = new SpawnRange<float>?(new SpawnRange<float>(15f, 35f));
      lootItemSerData60.Health = nullable48;
      lootItemSerDataList13.Add(lootItemSerData60);
      SpawnSerialization.LootItemSerData lootItemSerData61 = new SpawnSerialization.LootItemSerData();
      lootItemSerData61.ItemType = "GenericItem";
      lootItemSerData61.GenericItemSubType = "AltCorp_Cup";
      SpawnRange<float>? nullable49 = new SpawnRange<float>?(new SpawnRange<float>(15f, 35f));
      lootItemSerData61.Health = nullable49;
      lootItemSerDataList13.Add(lootItemSerData61);
      lootTierData13.Items = lootItemSerDataList13;
      lootTierDataList5.Add(lootTierData13);
      SpawnSerialization.LootTierData lootTierData14 = new SpawnSerialization.LootTierData();
      lootTierData14.TierName = "T2";
      List<SpawnSerialization.LootItemSerData> lootItemSerDataList14 = new List<SpawnSerialization.LootItemSerData>();
      SpawnSerialization.LootItemSerData lootItemSerData62 = new SpawnSerialization.LootItemSerData();
      lootItemSerData62.ItemType = "GenericItem";
      lootItemSerData62.GenericItemSubType = "Poster";
      lootItemSerData62.Look = new List<string>()
      {
        "Hellion",
        "Bethyr",
        "Burner",
        "Turret",
        "CrewQuaters"
      };
      SpawnRange<float>? nullable50 = new SpawnRange<float>?(new SpawnRange<float>(45f, 65f));
      lootItemSerData62.Health = nullable50;
      lootItemSerDataList14.Add(lootItemSerData62);
      SpawnSerialization.LootItemSerData lootItemSerData63 = new SpawnSerialization.LootItemSerData();
      lootItemSerData63.ItemType = "GenericItem";
      lootItemSerData63.GenericItemSubType = "BasketBall";
      SpawnRange<float>? nullable51 = new SpawnRange<float>?(new SpawnRange<float>(45f, 65f));
      lootItemSerData63.Health = nullable51;
      lootItemSerDataList14.Add(lootItemSerData63);
      SpawnSerialization.LootItemSerData lootItemSerData64 = new SpawnSerialization.LootItemSerData();
      lootItemSerData64.ItemType = "GenericItem";
      lootItemSerData64.GenericItemSubType = "Hoop";
      SpawnRange<float>? nullable52 = new SpawnRange<float>?(new SpawnRange<float>(45f, 65f));
      lootItemSerData64.Health = nullable52;
      lootItemSerDataList14.Add(lootItemSerData64);
      SpawnSerialization.LootItemSerData lootItemSerData65 = new SpawnSerialization.LootItemSerData();
      lootItemSerData65.ItemType = "GenericItem";
      lootItemSerData65.GenericItemSubType = "LavaLamp";
      SpawnRange<float>? nullable53 = new SpawnRange<float>?(new SpawnRange<float>(45f, 65f));
      lootItemSerData65.Health = nullable53;
      lootItemSerDataList14.Add(lootItemSerData65);
      SpawnSerialization.LootItemSerData lootItemSerData66 = new SpawnSerialization.LootItemSerData();
      lootItemSerData66.ItemType = "GenericItem";
      lootItemSerData66.GenericItemSubType = "AltCorp_Cup";
      SpawnRange<float>? nullable54 = new SpawnRange<float>?(new SpawnRange<float>(45f, 65f));
      lootItemSerData66.Health = nullable54;
      lootItemSerDataList14.Add(lootItemSerData66);
      SpawnSerialization.LootItemSerData lootItemSerData67 = new SpawnSerialization.LootItemSerData();
      lootItemSerData67.ItemType = "GenericItem";
      lootItemSerData67.GenericItemSubType = "CoffeeMachine";
      SpawnRange<float>? nullable55 = new SpawnRange<float>?(new SpawnRange<float>(45f, 65f));
      lootItemSerData67.Health = nullable55;
      lootItemSerDataList14.Add(lootItemSerData67);
      SpawnSerialization.LootItemSerData lootItemSerData68 = new SpawnSerialization.LootItemSerData();
      lootItemSerData68.ItemType = "GenericItem";
      lootItemSerData68.GenericItemSubType = "PlantRing";
      SpawnRange<float>? nullable56 = new SpawnRange<float>?(new SpawnRange<float>(45f, 65f));
      lootItemSerData68.Health = nullable56;
      lootItemSerDataList14.Add(lootItemSerData68);
      SpawnSerialization.LootItemSerData lootItemSerData69 = new SpawnSerialization.LootItemSerData();
      lootItemSerData69.ItemType = "GenericItem";
      lootItemSerData69.GenericItemSubType = "PlantCanister";
      SpawnRange<float>? nullable57 = new SpawnRange<float>?(new SpawnRange<float>(45f, 65f));
      lootItemSerData69.Health = nullable57;
      lootItemSerDataList14.Add(lootItemSerData69);
      SpawnSerialization.LootItemSerData lootItemSerData70 = new SpawnSerialization.LootItemSerData();
      lootItemSerData70.ItemType = "GenericItem";
      lootItemSerData70.GenericItemSubType = "TeslaBall";
      SpawnRange<float>? nullable58 = new SpawnRange<float>?(new SpawnRange<float>(45f, 65f));
      lootItemSerData70.Health = nullable58;
      lootItemSerDataList14.Add(lootItemSerData70);
      lootTierData14.Items = lootItemSerDataList14;
      lootTierDataList5.Add(lootTierData14);
      SpawnSerialization.LootTierData lootTierData15 = new SpawnSerialization.LootTierData();
      lootTierData15.TierName = "T3";
      List<SpawnSerialization.LootItemSerData> lootItemSerDataList15 = new List<SpawnSerialization.LootItemSerData>();
      SpawnSerialization.LootItemSerData lootItemSerData71 = new SpawnSerialization.LootItemSerData();
      lootItemSerData71.ItemType = "GenericItem";
      lootItemSerData71.GenericItemSubType = "Poster";
      lootItemSerData71.Look = new List<string>()
      {
        "Hellion",
        "Bethyr",
        "Burner",
        "Turret",
        "CrewQuaters",
        "Everest"
      };
      SpawnRange<float>? nullable59 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData71.Health = nullable59;
      lootItemSerDataList15.Add(lootItemSerData71);
      SpawnSerialization.LootItemSerData lootItemSerData72 = new SpawnSerialization.LootItemSerData();
      lootItemSerData72.ItemType = "GenericItem";
      lootItemSerData72.GenericItemSubType = "BasketBall";
      SpawnRange<float>? nullable60 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData72.Health = nullable60;
      lootItemSerDataList15.Add(lootItemSerData72);
      SpawnSerialization.LootItemSerData lootItemSerData73 = new SpawnSerialization.LootItemSerData();
      lootItemSerData73.ItemType = "GenericItem";
      lootItemSerData73.GenericItemSubType = "Hoop";
      SpawnRange<float>? nullable61 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData73.Health = nullable61;
      lootItemSerDataList15.Add(lootItemSerData73);
      SpawnSerialization.LootItemSerData lootItemSerData74 = new SpawnSerialization.LootItemSerData();
      lootItemSerData74.ItemType = "GenericItem";
      lootItemSerData74.GenericItemSubType = "LavaLamp";
      SpawnRange<float>? nullable62 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData74.Health = nullable62;
      lootItemSerDataList15.Add(lootItemSerData74);
      SpawnSerialization.LootItemSerData lootItemSerData75 = new SpawnSerialization.LootItemSerData();
      lootItemSerData75.ItemType = "GenericItem";
      lootItemSerData75.GenericItemSubType = "AltCorp_Cup";
      SpawnRange<float>? nullable63 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData75.Health = nullable63;
      lootItemSerDataList15.Add(lootItemSerData75);
      SpawnSerialization.LootItemSerData lootItemSerData76 = new SpawnSerialization.LootItemSerData();
      lootItemSerData76.ItemType = "GenericItem";
      lootItemSerData76.GenericItemSubType = "CoffeeMachine";
      SpawnRange<float>? nullable64 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData76.Health = nullable64;
      lootItemSerDataList15.Add(lootItemSerData76);
      SpawnSerialization.LootItemSerData lootItemSerData77 = new SpawnSerialization.LootItemSerData();
      lootItemSerData77.ItemType = "GenericItem";
      lootItemSerData77.GenericItemSubType = "PlantRing";
      SpawnRange<float>? nullable65 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData77.Health = nullable65;
      lootItemSerDataList15.Add(lootItemSerData77);
      SpawnSerialization.LootItemSerData lootItemSerData78 = new SpawnSerialization.LootItemSerData();
      lootItemSerData78.ItemType = "GenericItem";
      lootItemSerData78.GenericItemSubType = "PlantCanister";
      SpawnRange<float>? nullable66 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData78.Health = nullable66;
      lootItemSerDataList15.Add(lootItemSerData78);
      SpawnSerialization.LootItemSerData lootItemSerData79 = new SpawnSerialization.LootItemSerData();
      lootItemSerData79.ItemType = "GenericItem";
      lootItemSerData79.GenericItemSubType = "TeslaBall";
      SpawnRange<float>? nullable67 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData79.Health = nullable67;
      lootItemSerDataList15.Add(lootItemSerData79);
      SpawnSerialization.LootItemSerData lootItemSerData80 = new SpawnSerialization.LootItemSerData();
      lootItemSerData80.ItemType = "GenericItem";
      lootItemSerData80.GenericItemSubType = "PlantZikaLeaf";
      SpawnRange<float>? nullable68 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData80.Health = nullable68;
      lootItemSerDataList15.Add(lootItemSerData80);
      SpawnSerialization.LootItemSerData lootItemSerData81 = new SpawnSerialization.LootItemSerData();
      lootItemSerData81.ItemType = "GenericItem";
      lootItemSerData81.GenericItemSubType = "Picture";
      lootItemSerData81.Look = new List<string>()
      {
        "Atlas",
        "Actaeon"
      };
      SpawnRange<float>? nullable69 = new SpawnRange<float>?(new SpawnRange<float>(85f, 100f));
      lootItemSerData81.Health = nullable69;
      lootItemSerDataList15.Add(lootItemSerData81);
      lootTierData15.Items = lootItemSerDataList15;
      lootTierDataList5.Add(lootTierData15);
      SpawnSerialization.LootTierData lootTierData16 = new SpawnSerialization.LootTierData();
      lootTierData16.TierName = "T4";
      List<SpawnSerialization.LootItemSerData> lootItemSerDataList16 = new List<SpawnSerialization.LootItemSerData>();
      SpawnSerialization.LootItemSerData lootItemSerData82 = new SpawnSerialization.LootItemSerData();
      lootItemSerData82.ItemType = "GenericItem";
      lootItemSerData82.GenericItemSubType = "BookHolder";
      SpawnRange<float>? nullable70 = new SpawnRange<float>?(new SpawnRange<float>(100f, 100f));
      lootItemSerData82.Health = nullable70;
      lootItemSerDataList16.Add(lootItemSerData82);
      lootTierData16.Items = lootItemSerDataList16;
      lootTierDataList5.Add(lootTierData16);
      lootCategoryData5.Tiers = lootTierDataList5;
      lootCategoryDataList.Add(lootCategoryData5);
      lootCategoryDataList.Add(new SpawnSerialization.LootCategoryData()
      {
        CategoryName = "Suits",
        Tiers = new List<SpawnSerialization.LootTierData>()
        {
          new SpawnSerialization.LootTierData()
          {
            TierName = "T1",
            Items = new List<SpawnSerialization.LootItemSerData>()
            {
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "AltairPressurisedSuit"
              },
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "AltairPressurisedHelmet"
              },
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "AltairPressurisedJetpack"
              }
            }
          },
          new SpawnSerialization.LootTierData()
          {
            TierName = "T2",
            Items = new List<SpawnSerialization.LootItemSerData>()
            {
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "AltairEVASuit"
              },
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "AltairEVAHelmet"
              },
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "AltairEVAJetpack"
              }
            }
          }
        }
      });
      lootCategoryDataList.Add(new SpawnSerialization.LootCategoryData()
      {
        CategoryName = "Utilities",
        Tiers = new List<SpawnSerialization.LootTierData>()
        {
          new SpawnSerialization.LootTierData()
          {
            TierName = "T1",
            Items = new List<SpawnSerialization.LootItemSerData>()
            {
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "AltairMedpackSmall"
              }
            }
          },
          new SpawnSerialization.LootTierData()
          {
            TierName = "T2",
            Items = new List<SpawnSerialization.LootItemSerData>()
            {
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "AltairMedpackBig"
              }
            }
          },
          new SpawnSerialization.LootTierData()
          {
            TierName = "T3",
            Items = new List<SpawnSerialization.LootItemSerData>()
            {
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "AltairDisposableHackingTool"
              }
            }
          }
        }
      });
      lootCategoryDataList.Add(new SpawnSerialization.LootCategoryData()
      {
        CategoryName = "Mining",
        Tiers = new List<SpawnSerialization.LootTierData>()
        {
          new SpawnSerialization.LootTierData()
          {
            TierName = "T1",
            Items = new List<SpawnSerialization.LootItemSerData>()
            {
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "AltairHandDrill"
              },
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "AltairHandDrillCanister"
              },
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "AltairHandDrillBattery"
              },
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "AltairHandheldAsteroidScanningTool"
              }
            }
          }
        }
      });
      lootCategoryDataList.Add(new SpawnSerialization.LootCategoryData()
      {
        CategoryName = "Turrets",
        Tiers = new List<SpawnSerialization.LootTierData>()
        {
          new SpawnSerialization.LootTierData()
          {
            TierName = "T1",
            Items = new List<SpawnSerialization.LootItemSerData>()
            {
              new SpawnSerialization.LootItemSerData()
              {
                ItemType = "PortableTurret"
              }
            }
          }
        }
      });
      ZeroGravity.Json.SerializeToFile((object) lootCategoryDataList, directory + "Data/LootCategories.json", ZeroGravity.Json.Formatting.Indented, new JsonSerializerSettings()
      {
        NullValueHandling = NullValueHandling.Ignore
      });
    }

    public static List<SpawnRule> LoadSpawnRuleData()
    {
      List<SpawnSerialization.SpawnRuleData> spawnRuleDataList = ZeroGravity.Json.Load<List<SpawnSerialization.SpawnRuleData>>(SpawnSerialization.GetDirectory() + "Data/SpawnRules.json");
      if (spawnRuleDataList == null || spawnRuleDataList.Count <= 0)
        return (List<SpawnRule>) null;
      List<SpawnRule> spawnRuleList = new List<SpawnRule>();
      CelestialBodyGUID result1 = CelestialBodyGUID.Bethyr;
      SpawnRuleLocationType result2 = SpawnRuleLocationType.Random;
      foreach (SpawnSerialization.SpawnRuleData data in spawnRuleDataList)
      {
        if (data.RuleName.IsNullOrEmpty())
        {
          Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule name is not set"));
        }
        else
        {
          foreach (SpawnRule spawnRule in spawnRuleList)
          {
            if (spawnRule.Name == data.RuleName)
              Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" already exists", (object) data.RuleName));
          }
          if (data.Orbit == null)
            Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" orbit is not set", (object) data.RuleName));
          else if (data.Orbit.CelestialBody.IsNullOrEmpty() || !Enum.TryParse<CelestialBodyGUID>(data.Orbit.CelestialBody, out result1))
            Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" orbit celestial body \"{1}\" does not exist", (object) data.RuleName, (object) data.Orbit.CelestialBody));
          else if (data.Orbit.PeriapsisDistance_Km.Min < 0.0 || data.Orbit.PeriapsisDistance_Km.Max <= 0.0 || data.Orbit.PeriapsisDistance_Km.Min > data.Orbit.PeriapsisDistance_Km.Max)
            Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" orbit periapsis distance is not valid (min: {1}, max: {2})", (object) data.RuleName, (object) data.Orbit.PeriapsisDistance_Km.Min, (object) data.Orbit.PeriapsisDistance_Km.Max));
          else if (data.Orbit.ApoapsisDistance_Km.Min < 0.0 || data.Orbit.ApoapsisDistance_Km.Max <= 0.0 || data.Orbit.ApoapsisDistance_Km.Min > data.Orbit.ApoapsisDistance_Km.Max)
            Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" orbit apoapsis distance is not valid (min: {1}, max: {2})", (object) data.RuleName, (object) data.Orbit.ApoapsisDistance_Km.Min, (object) data.Orbit.ApoapsisDistance_Km.Max));
          else if (data.LocationType.IsNullOrEmpty() || !Enum.TryParse<SpawnRuleLocationType>(data.LocationType, out result2))
            Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" location type \"{1}\" is not valid", (object) data.RuleName, (object) data.LocationType));
          else if ((double) data.Orbit.Inclination_Deg.Min > (double) data.Orbit.Inclination_Deg.Max)
            Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" orbit inclination is not valid (min: {1}, max: {2})", (object) data.RuleName, (object) data.Orbit.Inclination_Deg.Min, (object) data.Orbit.Inclination_Deg.Max));
          else if ((double) data.Orbit.ArgumentOfPeriapsis_Deg.Min > (double) data.Orbit.ArgumentOfPeriapsis_Deg.Max)
            Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" orbit argument of periapsis is not valid (min: {1}, max: {2})", (object) data.RuleName, (object) data.Orbit.ArgumentOfPeriapsis_Deg.Min, (object) data.Orbit.ArgumentOfPeriapsis_Deg.Max));
          else if ((double) data.Orbit.LongitudeOfAscendingNode_Deg.Min > (double) data.Orbit.LongitudeOfAscendingNode_Deg.Max)
            Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" orbit longitude of ascending node is not valid (min: {1}, max: {2})", (object) data.RuleName, (object) data.Orbit.LongitudeOfAscendingNode_Deg.Min, (object) data.Orbit.LongitudeOfAscendingNode_Deg.Max));
          else if ((double) data.Orbit.TrueAnomaly_Deg.Min < 0.0 || (double) data.Orbit.TrueAnomaly_Deg.Max < 0.0 || (double) data.Orbit.TrueAnomaly_Deg.Min > (double) data.Orbit.TrueAnomaly_Deg.Max)
            Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" orbit true anomaly is not valid (min: {1}, max: {2})", (object) data.RuleName, (object) data.Orbit.TrueAnomaly_Deg.Min, (object) data.Orbit.TrueAnomaly_Deg.Max));
          else if (result2 == SpawnRuleLocationType.Random && (data.NumberOfClusters.Min < 0 || data.NumberOfClusters.Max <= 0 || data.NumberOfClusters.Min > data.NumberOfClusters.Max))
          {
            Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" number of clusters are not valid (min: {1}, max: {2})", (object) data.RuleName, (object) data.NumberOfClusters.Min, (object) data.NumberOfClusters.Max));
          }
          else
          {
            List<SpawnRuleScene> retVal1 = (List<SpawnRuleScene>) null;
            if (result2 == SpawnRuleLocationType.Random && !SpawnSerialization.GenerateScenePoolData(data, out retVal1))
            {
              Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" scenes are not valid", (object) data.RuleName));
            }
            else
            {
              List<SpawnRuleLoot> retVal2 = (List<SpawnRuleLoot>) null;
              if (!SpawnSerialization.GenerateLootPoolData(data, result2, out retVal2))
              {
                Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" loot is not valid", (object) data.RuleName));
              }
              else
              {
                CelestialBody celestialBody = Server.Instance.SolarSystem.GetCelestialBody((long) result1);
                if (data.Orbit.PeriapsisDistance_Km.Min > celestialBody.Orbit.GravityInfluenceRadius / 1000.0 || data.Orbit.PeriapsisDistance_Km.Max > celestialBody.Orbit.GravityInfluenceRadius / 1000.0)
                  Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" orbit periapsis distance is larger than gravity influence radius \"{3}\" (min: {1}, max: {2})", (object) data.RuleName, (object) data.Orbit.PeriapsisDistance_Km.Min, (object) data.Orbit.PeriapsisDistance_Km.Max, (object) (celestialBody.Orbit.GravityInfluenceRadius / 1000.0)));
                else if (data.Orbit.ApoapsisDistance_Km.Min > celestialBody.Orbit.GravityInfluenceRadius / 1000.0 || data.Orbit.ApoapsisDistance_Km.Max > celestialBody.Orbit.GravityInfluenceRadius / 1000.0)
                {
                  Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" orbit periapsis distance is larger than gravity influence radius \"{3}\" (min: {1}, max: {2})", (object) data.RuleName, (object) data.Orbit.ApoapsisDistance_Km.Min, (object) data.Orbit.ApoapsisDistance_Km.Max, (object) (celestialBody.Orbit.GravityInfluenceRadius / 1000.0)));
                }
                else
                {
                  SpawnRule spawnRule1 = new SpawnRule();
                  spawnRule1.Name = data.RuleName;
                  spawnRule1.StationName = data.StationName;
                  SpawnRuleOrbit spawnRuleOrbit = new SpawnRuleOrbit();
                  spawnRuleOrbit.CelestialBody = result1;
                  SpawnRange<double> spawnRange1 = new SpawnRange<double>(data.Orbit.PeriapsisDistance_Km.Min * 1000.0, data.Orbit.PeriapsisDistance_Km.Max * 1000.0);
                  spawnRuleOrbit.PeriapsisDistance = spawnRange1;
                  SpawnRange<double> spawnRange2 = new SpawnRange<double>(data.Orbit.ApoapsisDistance_Km.Min * 1000.0, data.Orbit.ApoapsisDistance_Km.Max * 1000.0);
                  spawnRuleOrbit.ApoapsisDistance = spawnRange2;
                  SpawnRange<double> spawnRange3 = new SpawnRange<double>((double) data.Orbit.Inclination_Deg.Min % 360.0, (double) data.Orbit.Inclination_Deg.Max % 360.0);
                  spawnRuleOrbit.Inclination = spawnRange3;
                  SpawnRange<double> spawnRange4 = new SpawnRange<double>((double) data.Orbit.ArgumentOfPeriapsis_Deg.Min % 360.0, (double) data.Orbit.ArgumentOfPeriapsis_Deg.Max % 360.0);
                  spawnRuleOrbit.ArgumentOfPeriapsis = spawnRange4;
                  SpawnRange<double> spawnRange5 = new SpawnRange<double>((double) data.Orbit.LongitudeOfAscendingNode_Deg.Min % 360.0, (double) data.Orbit.LongitudeOfAscendingNode_Deg.Max % 360.0);
                  spawnRuleOrbit.LongitudeOfAscendingNode = spawnRange5;
                  SpawnRange<double> spawnRange6 = new SpawnRange<double>((double) data.Orbit.TrueAnomaly_Deg.Min % 360.0, (double) data.Orbit.TrueAnomaly_Deg.Max % 360.0);
                  spawnRuleOrbit.TrueAnomaly = spawnRange6;
                  spawnRule1.Orbit = spawnRuleOrbit;
                  int num1 = (int) result2;
                  spawnRule1.LocationType = (SpawnRuleLocationType) num1;
                  string locationTag = data.LocationTag;
                  spawnRule1.LocationTag = locationTag;
                  double num2 = System.Math.Max(-1.0, data.RespawnTimer_Minutes * 60.0);
                  spawnRule1.RespawnTimerSec = num2;
                  SpawnRange<int> spawnRange7 = result2 != SpawnRuleLocationType.Random ? new SpawnRange<int>(1, 1) : data.NumberOfClusters;
                  spawnRule1.NumberOfClusters = spawnRange7;
                  List<SpawnRuleScene> spawnRuleSceneList = retVal1;
                  spawnRule1.ScenePool = spawnRuleSceneList;
                  List<SpawnRuleLoot> spawnRuleLootList = retVal2;
                  spawnRule1.LootPool = spawnRuleLootList;
                  int num3 = data.IsVisibleOnRadar ? 1 : 0;
                  spawnRule1.IsVisibleOnRadar = num3 != 0;
                  SpawnRule spawnRule2 = spawnRule1;
                  spawnRuleList.Add(spawnRule2);
                }
              }
            }
          }
        }
      }
      return spawnRuleList;
    }

    private static bool GenerateScenePoolData(SpawnSerialization.SpawnRuleData data, out List<SpawnRuleScene> retVal)
    {
      retVal = new List<SpawnRuleScene>();
      GameScenes.SceneID result = GameScenes.SceneID.None;
      if (data.LocationScenes != null)
      {
        foreach (SpawnSerialization.SpawnRuleSceneData locationScene in data.LocationScenes)
        {
          if (locationScene.Scene.IsNullOrEmpty() || !Enum.TryParse<GameScenes.SceneID>(locationScene.Scene, out result))
            Dbg.Warning(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" scene \"{1}\" is not valid", (object) data.RuleName, (object) locationScene.Scene));
          else if (locationScene.SceneCount.Min < 0 || locationScene.SceneCount.Max <= 0 || locationScene.SceneCount.Min > locationScene.SceneCount.Max)
          {
            Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" scene \"{1}\" count is not valid (min: {2}, max: {3})", (object) data.RuleName, (object) locationScene.Scene, (object) locationScene.SceneCount.Min, (object) locationScene.SceneCount.Max));
          }
          else
          {
            int num = MathHelper.RandomRange(locationScene.SceneCount.Min, locationScene.SceneCount.Max + 1);
            List<SpawnRuleScene> spawnRuleSceneList = retVal;
            SpawnRuleScene spawnRuleScene = new SpawnRuleScene();
            spawnRuleScene.SceneID = result;
            spawnRuleScene.Count = num;
            spawnRuleScene.CountMax = num;
            double min = (double) locationScene.Health.Min;
            spawnRuleScene.HealthMin = (float) min;
            double max = (double) locationScene.Health.Max;
            spawnRuleScene.HealthMax = (float) max;
            spawnRuleSceneList.Add(spawnRuleScene);
          }
        }
      }
      return retVal.Count > 0;
    }

    private static bool GenerateLootPoolData(SpawnSerialization.SpawnRuleData data, SpawnRuleLocationType locType, out List<SpawnRuleLoot> retVal)
    {
      retVal = new List<SpawnRuleLoot>();
      if (data.Loot != null)
      {
        LootTier result = LootTier.T1;
        foreach (SpawnSerialization.SpawnRuleLootData spawnRuleLootData in data.Loot)
        {
          if (spawnRuleLootData.Tier.IsNullOrEmpty() || !Enum.TryParse<LootTier>(spawnRuleLootData.Tier, out result))
            Dbg.Warning(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" tier \"{1}\" is not valid", (object) data.RuleName, (object) spawnRuleLootData.Tier));
          else if (spawnRuleLootData.LootCount.Max <= 0 || spawnRuleLootData.LootCount.Min > spawnRuleLootData.LootCount.Max)
          {
            Dbg.Error(string.Format("SPAWN MANAGER - Spawn rule \"{0}\" tier \"{1}\" loot count is not valid (min: {2}, max: {3})", (object) data.RuleName, (object) spawnRuleLootData.Tier, (object) spawnRuleLootData.LootCount.Min, (object) spawnRuleLootData.LootCount.Max));
          }
          else
          {
            int num = MathHelper.RandomRange(spawnRuleLootData.LootCount.Min, spawnRuleLootData.LootCount.Max + 1);
            if (num > 0)
            {
              List<LootItemData> dataFromCategory = SpawnManager.GetLootItemDataFromCategory(data.RuleName, spawnRuleLootData.CategoryName, result);
              if (dataFromCategory.Count != 0)
              {
                if (dataFromCategory.Count == 1)
                {
                  List<SpawnRuleLoot> spawnRuleLootList = retVal;
                  SpawnRuleLoot spawnRuleLoot = new SpawnRuleLoot();
                  spawnRuleLoot.Count = num;
                  spawnRuleLoot.CountMax = num;
                  LootItemData lootItemData = dataFromCategory[0];
                  spawnRuleLoot.Data = lootItemData;
                  spawnRuleLootList.Add(spawnRuleLoot);
                }
                else
                {
                  int[] numArray = new int[dataFromCategory.Count];
                  for (int index = 0; index < num; ++index)
                    ++numArray[MathHelper.RandomRange(0, numArray.Length)];
                  for (int index = 0; index < numArray.Length; ++index)
                  {
                    if (numArray[index] != 0)
                      retVal.Add(new SpawnRuleLoot()
                      {
                        Count = numArray[index],
                        CountMax = numArray[index],
                        Data = dataFromCategory[index]
                      });
                  }
                }
              }
            }
          }
        }
      }
      return true;
    }

    public static void GenerateSpawnRuleSampleData(bool force = false)
    {
      string directory = SpawnSerialization.GetDirectory();
      if (!force && File.Exists(directory + "Data/SpawnRules.json"))
        return;
      List<SpawnSerialization.SpawnRuleData> spawnRuleDataList = new List<SpawnSerialization.SpawnRuleData>();
      SpawnSerialization.SpawnRuleData spawnRuleData1 = new SpawnSerialization.SpawnRuleData();
      spawnRuleData1.RuleName = "Fresh starts";
      SpawnSerialization.SpawnRuleOrbitData spawnRuleOrbitData1 = new SpawnSerialization.SpawnRuleOrbitData();
      spawnRuleOrbitData1.CelestialBody = "Bethyr";
      SpawnRange<double> spawnRange1 = new SpawnRange<double>(34782.0, 36782.0);
      spawnRuleOrbitData1.PeriapsisDistance_Km = spawnRange1;
      SpawnRange<double> spawnRange2 = new SpawnRange<double>(34782.0, 44268.0);
      spawnRuleOrbitData1.ApoapsisDistance_Km = spawnRange2;
      SpawnRange<float> spawnRange3 = new SpawnRange<float>(-1f, 1f);
      spawnRuleOrbitData1.Inclination_Deg = spawnRange3;
      SpawnRange<float> spawnRange4 = new SpawnRange<float>(-1f, 1f);
      spawnRuleOrbitData1.ArgumentOfPeriapsis_Deg = spawnRange4;
      SpawnRange<float> spawnRange5 = new SpawnRange<float>(-1f, 1f);
      spawnRuleOrbitData1.LongitudeOfAscendingNode_Deg = spawnRange5;
      SpawnRange<float> spawnRange6 = new SpawnRange<float>(0.0f, 359.999f);
      spawnRuleOrbitData1.TrueAnomaly_Deg = spawnRange6;
      spawnRuleData1.Orbit = spawnRuleOrbitData1;
      string str1 = "StartingScene";
      spawnRuleData1.LocationType = str1;
      string str2 = "StartingScene";
      spawnRuleData1.LocationTag = str2;
      int num1 = 0;
      spawnRuleData1.IsVisibleOnRadar = num1 != 0;
      spawnRuleDataList.Add(spawnRuleData1);
      SpawnSerialization.SpawnRuleData spawnRuleData2 = new SpawnSerialization.SpawnRuleData();
      spawnRuleData2.RuleName = "Bethyr derelict outposts T1";
      SpawnSerialization.SpawnRuleOrbitData spawnRuleOrbitData2 = new SpawnSerialization.SpawnRuleOrbitData();
      spawnRuleOrbitData2.CelestialBody = "Bethyr";
      SpawnRange<double> spawnRange7 = new SpawnRange<double>(34282.0, 37782.0);
      spawnRuleOrbitData2.PeriapsisDistance_Km = spawnRange7;
      SpawnRange<double> spawnRange8 = new SpawnRange<double>(34282.0, 43631.0);
      spawnRuleOrbitData2.ApoapsisDistance_Km = spawnRange8;
      SpawnRange<float> spawnRange9 = new SpawnRange<float>(-1.5f, 1.5f);
      spawnRuleOrbitData2.Inclination_Deg = spawnRange9;
      SpawnRange<float> spawnRange10 = new SpawnRange<float>(-1.5f, 1.5f);
      spawnRuleOrbitData2.ArgumentOfPeriapsis_Deg = spawnRange10;
      SpawnRange<float> spawnRange11 = new SpawnRange<float>(-1.5f, 1.5f);
      spawnRuleOrbitData2.LongitudeOfAscendingNode_Deg = spawnRange11;
      SpawnRange<float> spawnRange12 = new SpawnRange<float>(0.0f, 359.999f);
      spawnRuleOrbitData2.TrueAnomaly_Deg = spawnRange12;
      spawnRuleData2.Orbit = spawnRuleOrbitData2;
      string str3 = "Random";
      spawnRuleData2.LocationType = str3;
      double num2 = 180.0;
      spawnRuleData2.RespawnTimer_Minutes = num2;
      SpawnRange<int> spawnRange13 = new SpawnRange<int>(30, 30);
      spawnRuleData2.NumberOfClusters = spawnRange13;
      List<SpawnSerialization.SpawnRuleSceneData> spawnRuleSceneDataList1 = new List<SpawnSerialization.SpawnRuleSceneData>();
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData1 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData1.Scene = "AltCorp_StartingModule";
      SpawnRange<int> spawnRange14 = new SpawnRange<int>(10, 10);
      spawnRuleSceneData1.SceneCount = spawnRange14;
      spawnRuleSceneDataList1.Add(spawnRuleSceneData1);
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData2 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData2.Scene = "AltCorp_AirLock";
      SpawnRange<int> spawnRange15 = new SpawnRange<int>(15, 15);
      spawnRuleSceneData2.SceneCount = spawnRange15;
      spawnRuleSceneDataList1.Add(spawnRuleSceneData2);
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData3 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData3.Scene = "AltCorp_LifeSupportModule";
      SpawnRange<int> spawnRange16 = new SpawnRange<int>(10, 10);
      spawnRuleSceneData3.SceneCount = spawnRange16;
      spawnRuleSceneDataList1.Add(spawnRuleSceneData3);
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData4 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData4.Scene = "AltCorp_CorridorModule";
      SpawnRange<int> spawnRange17 = new SpawnRange<int>(7, 7);
      spawnRuleSceneData4.SceneCount = spawnRange17;
      spawnRuleSceneDataList1.Add(spawnRuleSceneData4);
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData5 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData5.Scene = "AltCorp_Corridor45TurnModule";
      SpawnRange<int> spawnRange18 = new SpawnRange<int>(7, 7);
      spawnRuleSceneData5.SceneCount = spawnRange18;
      spawnRuleSceneDataList1.Add(spawnRuleSceneData5);
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData6 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData6.Scene = "AltCorp_Corridor45TurnRightModule";
      SpawnRange<int> spawnRange19 = new SpawnRange<int>(6, 6);
      spawnRuleSceneData6.SceneCount = spawnRange19;
      spawnRuleSceneDataList1.Add(spawnRuleSceneData6);
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData7 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData7.Scene = "Generic_Debris_JuncRoom001";
      SpawnRange<int> spawnRange20 = new SpawnRange<int>(16, 16);
      spawnRuleSceneData7.SceneCount = spawnRange20;
      spawnRuleSceneDataList1.Add(spawnRuleSceneData7);
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData8 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData8.Scene = "Generic_Debris_JuncRoom002";
      SpawnRange<int> spawnRange21 = new SpawnRange<int>(16, 16);
      spawnRuleSceneData8.SceneCount = spawnRange21;
      spawnRuleSceneDataList1.Add(spawnRuleSceneData8);
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData9 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData9.Scene = "Generic_Debris_Corridor001";
      SpawnRange<int> spawnRange22 = new SpawnRange<int>(16, 16);
      spawnRuleSceneData9.SceneCount = spawnRange22;
      spawnRuleSceneDataList1.Add(spawnRuleSceneData9);
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData10 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData10.Scene = "Generic_Debris_Corridor002";
      SpawnRange<int> spawnRange23 = new SpawnRange<int>(17, 17);
      spawnRuleSceneData10.SceneCount = spawnRange23;
      spawnRuleSceneDataList1.Add(spawnRuleSceneData10);
      spawnRuleData2.LocationScenes = spawnRuleSceneDataList1;
      List<SpawnSerialization.SpawnRuleLootData> spawnRuleLootDataList1 = new List<SpawnSerialization.SpawnRuleLootData>();
      SpawnSerialization.SpawnRuleLootData spawnRuleLootData1 = new SpawnSerialization.SpawnRuleLootData();
      spawnRuleLootData1.CategoryName = "Resources";
      spawnRuleLootData1.Tier = "T1";
      SpawnRange<int> spawnRange24 = new SpawnRange<int>(50, 50);
      spawnRuleLootData1.LootCount = spawnRange24;
      spawnRuleLootDataList1.Add(spawnRuleLootData1);
      SpawnSerialization.SpawnRuleLootData spawnRuleLootData2 = new SpawnSerialization.SpawnRuleLootData();
      spawnRuleLootData2.CategoryName = "Fuel";
      spawnRuleLootData2.Tier = "T1";
      SpawnRange<int> spawnRange25 = new SpawnRange<int>(30, 30);
      spawnRuleLootData2.LootCount = spawnRange25;
      spawnRuleLootDataList1.Add(spawnRuleLootData2);
      SpawnSerialization.SpawnRuleLootData spawnRuleLootData3 = new SpawnSerialization.SpawnRuleLootData();
      spawnRuleLootData3.CategoryName = "Weapons";
      spawnRuleLootData3.Tier = "T1";
      SpawnRange<int> spawnRange26 = new SpawnRange<int>(25, 25);
      spawnRuleLootData3.LootCount = spawnRange26;
      spawnRuleLootDataList1.Add(spawnRuleLootData3);
      SpawnSerialization.SpawnRuleLootData spawnRuleLootData4 = new SpawnSerialization.SpawnRuleLootData();
      spawnRuleLootData4.CategoryName = "Ammo";
      spawnRuleLootData4.Tier = "T1";
      SpawnRange<int> spawnRange27 = new SpawnRange<int>(70, 70);
      spawnRuleLootData4.LootCount = spawnRange27;
      spawnRuleLootDataList1.Add(spawnRuleLootData4);
      SpawnSerialization.SpawnRuleLootData spawnRuleLootData5 = new SpawnSerialization.SpawnRuleLootData();
      spawnRuleLootData5.CategoryName = "Resources";
      spawnRuleLootData5.Tier = "T2";
      SpawnRange<int> spawnRange28 = new SpawnRange<int>(35, 35);
      spawnRuleLootData5.LootCount = spawnRange28;
      spawnRuleLootDataList1.Add(spawnRuleLootData5);
      SpawnSerialization.SpawnRuleLootData spawnRuleLootData6 = new SpawnSerialization.SpawnRuleLootData();
      spawnRuleLootData6.CategoryName = "Vanity";
      spawnRuleLootData6.Tier = "T1";
      SpawnRange<int> spawnRange29 = new SpawnRange<int>(15, 15);
      spawnRuleLootData6.LootCount = spawnRange29;
      spawnRuleLootDataList1.Add(spawnRuleLootData6);
      spawnRuleData2.Loot = spawnRuleLootDataList1;
      int num3 = 0;
      spawnRuleData2.IsVisibleOnRadar = num3 != 0;
      spawnRuleDataList.Add(spawnRuleData2);
      SpawnSerialization.SpawnRuleData spawnRuleData3 = new SpawnSerialization.SpawnRuleData();
      spawnRuleData3.RuleName = "Bethyr fuel outpost T1";
      SpawnSerialization.SpawnRuleOrbitData spawnRuleOrbitData3 = new SpawnSerialization.SpawnRuleOrbitData();
      spawnRuleOrbitData3.CelestialBody = "Bethyr";
      SpawnRange<double> spawnRange30 = new SpawnRange<double>(40782.0, 40782.0);
      spawnRuleOrbitData3.PeriapsisDistance_Km = spawnRange30;
      SpawnRange<double> spawnRange31 = new SpawnRange<double>(63787.0, 63787.0);
      spawnRuleOrbitData3.ApoapsisDistance_Km = spawnRange31;
      SpawnRange<float> spawnRange32 = new SpawnRange<float>(88f, 88f);
      spawnRuleOrbitData3.Inclination_Deg = spawnRange32;
      SpawnRange<float> spawnRange33 = new SpawnRange<float>(0.0f, 0.0f);
      spawnRuleOrbitData3.ArgumentOfPeriapsis_Deg = spawnRange33;
      SpawnRange<float> spawnRange34 = new SpawnRange<float>(0.0f, 0.0f);
      spawnRuleOrbitData3.LongitudeOfAscendingNode_Deg = spawnRange34;
      SpawnRange<float> spawnRange35 = new SpawnRange<float>(0.0f, 359.999f);
      spawnRuleOrbitData3.TrueAnomaly_Deg = spawnRange35;
      spawnRuleData3.Orbit = spawnRuleOrbitData3;
      SpawnRange<int> spawnRange36 = new SpawnRange<int>(1, 1);
      spawnRuleData3.NumberOfClusters = spawnRange36;
      double num4 = 180.0;
      spawnRuleData3.RespawnTimer_Minutes = num4;
      string str4 = "Emergency_Staging_Post_D8";
      spawnRuleData3.LocationType = str4;
      List<SpawnSerialization.SpawnRuleLootData> spawnRuleLootDataList2 = new List<SpawnSerialization.SpawnRuleLootData>();
      SpawnSerialization.SpawnRuleLootData spawnRuleLootData7 = new SpawnSerialization.SpawnRuleLootData();
      spawnRuleLootData7.CategoryName = "Resources";
      spawnRuleLootData7.Tier = "T1";
      SpawnRange<int> spawnRange37 = new SpawnRange<int>(50, 50);
      spawnRuleLootData7.LootCount = spawnRange37;
      spawnRuleLootDataList2.Add(spawnRuleLootData7);
      SpawnSerialization.SpawnRuleLootData spawnRuleLootData8 = new SpawnSerialization.SpawnRuleLootData();
      spawnRuleLootData8.CategoryName = "Fuel";
      spawnRuleLootData8.Tier = "T1";
      SpawnRange<int> spawnRange38 = new SpawnRange<int>(30, 30);
      spawnRuleLootData8.LootCount = spawnRange38;
      spawnRuleLootDataList2.Add(spawnRuleLootData8);
      SpawnSerialization.SpawnRuleLootData spawnRuleLootData9 = new SpawnSerialization.SpawnRuleLootData();
      spawnRuleLootData9.CategoryName = "Resources";
      spawnRuleLootData9.Tier = "T2";
      SpawnRange<int> spawnRange39 = new SpawnRange<int>(35, 35);
      spawnRuleLootData9.LootCount = spawnRange39;
      spawnRuleLootDataList2.Add(spawnRuleLootData9);
      spawnRuleData3.Loot = spawnRuleLootDataList2;
      int num5 = 0;
      spawnRuleData3.IsVisibleOnRadar = num5 != 0;
      spawnRuleDataList.Add(spawnRuleData3);
      SpawnSerialization.SpawnRuleData spawnRuleData4 = new SpawnSerialization.SpawnRuleData();
      spawnRuleData4.RuleName = "Bethyr loot outpost T2";
      SpawnSerialization.SpawnRuleOrbitData spawnRuleOrbitData4 = new SpawnSerialization.SpawnRuleOrbitData();
      spawnRuleOrbitData4.CelestialBody = "Bethyr";
      SpawnRange<double> spawnRange40 = new SpawnRange<double>(40782.0, 40782.0);
      spawnRuleOrbitData4.PeriapsisDistance_Km = spawnRange40;
      SpawnRange<double> spawnRange41 = new SpawnRange<double>(63787.0, 63787.0);
      spawnRuleOrbitData4.ApoapsisDistance_Km = spawnRange41;
      SpawnRange<float> spawnRange42 = new SpawnRange<float>(88f, 88f);
      spawnRuleOrbitData4.Inclination_Deg = spawnRange42;
      SpawnRange<float> spawnRange43 = new SpawnRange<float>(0.0f, 0.0f);
      spawnRuleOrbitData4.ArgumentOfPeriapsis_Deg = spawnRange43;
      SpawnRange<float> spawnRange44 = new SpawnRange<float>(0.0f, 0.0f);
      spawnRuleOrbitData4.LongitudeOfAscendingNode_Deg = spawnRange44;
      SpawnRange<float> spawnRange45 = new SpawnRange<float>(0.0f, 359.999f);
      spawnRuleOrbitData4.TrueAnomaly_Deg = spawnRange45;
      spawnRuleData4.Orbit = spawnRuleOrbitData4;
      SpawnRange<int> spawnRange46 = new SpawnRange<int>(1, 1);
      spawnRuleData4.NumberOfClusters = spawnRange46;
      double num6 = 180.0;
      spawnRuleData4.RespawnTimer_Minutes = num6;
      string str5 = "Automated_Refinery_B7";
      spawnRuleData4.LocationType = str5;
      List<SpawnSerialization.SpawnRuleLootData> spawnRuleLootDataList3 = new List<SpawnSerialization.SpawnRuleLootData>();
      SpawnSerialization.SpawnRuleLootData spawnRuleLootData10 = new SpawnSerialization.SpawnRuleLootData();
      spawnRuleLootData10.CategoryName = "Resources";
      spawnRuleLootData10.Tier = "T1";
      SpawnRange<int> spawnRange47 = new SpawnRange<int>(50, 50);
      spawnRuleLootData10.LootCount = spawnRange47;
      spawnRuleLootDataList3.Add(spawnRuleLootData10);
      SpawnSerialization.SpawnRuleLootData spawnRuleLootData11 = new SpawnSerialization.SpawnRuleLootData();
      spawnRuleLootData11.CategoryName = "Fuel";
      spawnRuleLootData11.Tier = "T1";
      SpawnRange<int> spawnRange48 = new SpawnRange<int>(30, 30);
      spawnRuleLootData11.LootCount = spawnRange48;
      spawnRuleLootDataList3.Add(spawnRuleLootData11);
      SpawnSerialization.SpawnRuleLootData spawnRuleLootData12 = new SpawnSerialization.SpawnRuleLootData();
      spawnRuleLootData12.CategoryName = "Weapons";
      spawnRuleLootData12.Tier = "T1";
      SpawnRange<int> spawnRange49 = new SpawnRange<int>(25, 25);
      spawnRuleLootData12.LootCount = spawnRange49;
      spawnRuleLootDataList3.Add(spawnRuleLootData12);
      SpawnSerialization.SpawnRuleLootData spawnRuleLootData13 = new SpawnSerialization.SpawnRuleLootData();
      spawnRuleLootData13.CategoryName = "Ammo";
      spawnRuleLootData13.Tier = "T1";
      SpawnRange<int> spawnRange50 = new SpawnRange<int>(70, 70);
      spawnRuleLootData13.LootCount = spawnRange50;
      spawnRuleLootDataList3.Add(spawnRuleLootData13);
      SpawnSerialization.SpawnRuleLootData spawnRuleLootData14 = new SpawnSerialization.SpawnRuleLootData();
      spawnRuleLootData14.CategoryName = "Resources";
      spawnRuleLootData14.Tier = "T2";
      SpawnRange<int> spawnRange51 = new SpawnRange<int>(35, 35);
      spawnRuleLootData14.LootCount = spawnRange51;
      spawnRuleLootDataList3.Add(spawnRuleLootData14);
      SpawnSerialization.SpawnRuleLootData spawnRuleLootData15 = new SpawnSerialization.SpawnRuleLootData();
      spawnRuleLootData15.CategoryName = "Vanity";
      spawnRuleLootData15.Tier = "T1";
      SpawnRange<int> spawnRange52 = new SpawnRange<int>(15, 15);
      spawnRuleLootData15.LootCount = spawnRange52;
      spawnRuleLootDataList3.Add(spawnRuleLootData15);
      spawnRuleData4.Loot = spawnRuleLootDataList3;
      int num7 = 1;
      spawnRuleData4.IsVisibleOnRadar = num7 != 0;
      spawnRuleDataList.Add(spawnRuleData4);
      SpawnSerialization.SpawnRuleData spawnRuleData5 = new SpawnSerialization.SpawnRuleData();
      spawnRuleData5.RuleName = "Bethyr asteroids";
      SpawnSerialization.SpawnRuleOrbitData spawnRuleOrbitData5 = new SpawnSerialization.SpawnRuleOrbitData();
      spawnRuleOrbitData5.CelestialBody = "Bethyr";
      SpawnRange<double> spawnRange53 = new SpawnRange<double>(40982.0, 41782.0);
      spawnRuleOrbitData5.PeriapsisDistance_Km = spawnRange53;
      SpawnRange<double> spawnRange54 = new SpawnRange<double>(40982.0, 48109.0);
      spawnRuleOrbitData5.ApoapsisDistance_Km = spawnRange54;
      SpawnRange<float> spawnRange55 = new SpawnRange<float>(10f, 10f);
      spawnRuleOrbitData5.Inclination_Deg = spawnRange55;
      SpawnRange<float> spawnRange56 = new SpawnRange<float>(0.0f, 0.0f);
      spawnRuleOrbitData5.ArgumentOfPeriapsis_Deg = spawnRange56;
      SpawnRange<float> spawnRange57 = new SpawnRange<float>(0.0f, 0.0f);
      spawnRuleOrbitData5.LongitudeOfAscendingNode_Deg = spawnRange57;
      SpawnRange<float> spawnRange58 = new SpawnRange<float>(0.0f, 359.999f);
      spawnRuleOrbitData5.TrueAnomaly_Deg = spawnRange58;
      spawnRuleData5.Orbit = spawnRuleOrbitData5;
      string str6 = "Random";
      spawnRuleData5.LocationType = str6;
      double num8 = -1.0;
      spawnRuleData5.RespawnTimer_Minutes = num8;
      SpawnRange<int> spawnRange59 = new SpawnRange<int>(8, 8);
      spawnRuleData5.NumberOfClusters = spawnRange59;
      List<SpawnSerialization.SpawnRuleSceneData> spawnRuleSceneDataList2 = new List<SpawnSerialization.SpawnRuleSceneData>();
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData11 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData11.Scene = "Asteroid01";
      SpawnRange<int> spawnRange60 = new SpawnRange<int>(1, 1);
      spawnRuleSceneData11.SceneCount = spawnRange60;
      spawnRuleSceneDataList2.Add(spawnRuleSceneData11);
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData12 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData12.Scene = "Asteroid02";
      SpawnRange<int> spawnRange61 = new SpawnRange<int>(1, 1);
      spawnRuleSceneData12.SceneCount = spawnRange61;
      spawnRuleSceneDataList2.Add(spawnRuleSceneData12);
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData13 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData13.Scene = "Asteroid03";
      SpawnRange<int> spawnRange62 = new SpawnRange<int>(1, 1);
      spawnRuleSceneData13.SceneCount = spawnRange62;
      spawnRuleSceneDataList2.Add(spawnRuleSceneData13);
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData14 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData14.Scene = "Asteroid04";
      SpawnRange<int> spawnRange63 = new SpawnRange<int>(1, 1);
      spawnRuleSceneData14.SceneCount = spawnRange63;
      spawnRuleSceneDataList2.Add(spawnRuleSceneData14);
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData15 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData15.Scene = "Asteroid05";
      SpawnRange<int> spawnRange64 = new SpawnRange<int>(1, 1);
      spawnRuleSceneData15.SceneCount = spawnRange64;
      spawnRuleSceneDataList2.Add(spawnRuleSceneData15);
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData16 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData16.Scene = "Asteroid06";
      SpawnRange<int> spawnRange65 = new SpawnRange<int>(1, 1);
      spawnRuleSceneData16.SceneCount = spawnRange65;
      spawnRuleSceneDataList2.Add(spawnRuleSceneData16);
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData17 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData17.Scene = "Asteroid07";
      SpawnRange<int> spawnRange66 = new SpawnRange<int>(1, 1);
      spawnRuleSceneData17.SceneCount = spawnRange66;
      spawnRuleSceneDataList2.Add(spawnRuleSceneData17);
      SpawnSerialization.SpawnRuleSceneData spawnRuleSceneData18 = new SpawnSerialization.SpawnRuleSceneData();
      spawnRuleSceneData18.Scene = "Asteroid08";
      SpawnRange<int> spawnRange67 = new SpawnRange<int>(1, 1);
      spawnRuleSceneData18.SceneCount = spawnRange67;
      spawnRuleSceneDataList2.Add(spawnRuleSceneData18);
      spawnRuleData5.LocationScenes = spawnRuleSceneDataList2;
      int num9 = 0;
      spawnRuleData5.IsVisibleOnRadar = num9 != 0;
      spawnRuleDataList.Add(spawnRuleData5);
      ZeroGravity.Json.SerializeToFile((object) spawnRuleDataList, directory + "Data/SpawnRules.json", ZeroGravity.Json.Formatting.Indented, new JsonSerializerSettings()
      {
        NullValueHandling = NullValueHandling.Ignore
      });
    }

    public class LootItemSerData
    {
      public string ItemType;
      public string GenericItemSubType;
      public string MachineryPartType;
      public SpawnRange<float>? Health;
      public List<string> Look;
      public SpawnRange<float>? Power;
      public SpawnRange<int>? Count;
      public bool? IsActive;
      public List<SpawnSerialization.LootItemSerData.CargoResourceData> Cargo;

      public class CargoResourceData
      {
        public List<string> Resources;
        public SpawnRange<float> Quantity;
      }
    }

    public class LootTierData
    {
      public string TierName;
      public List<SpawnSerialization.LootItemSerData> Items;
    }

    public class LootCategoryData
    {
      public string CategoryName;
      public List<SpawnSerialization.LootTierData> Tiers;
    }

    public class SpawnRuleOrbitData
    {
      public string CelestialBody;
      public SpawnRange<double> PeriapsisDistance_Km;
      public SpawnRange<double> ApoapsisDistance_Km;
      public SpawnRange<float> Inclination_Deg;
      public SpawnRange<float> ArgumentOfPeriapsis_Deg;
      public SpawnRange<float> LongitudeOfAscendingNode_Deg;
      public SpawnRange<float> TrueAnomaly_Deg;
    }

    public class SpawnRuleSceneData
    {
      public string Scene;
      public SpawnRange<int> SceneCount;
      public SpawnRange<float> Health;
    }

    public class SpawnRuleLootData
    {
      public string CategoryName;
      public string Tier;
      public SpawnRange<int> LootCount;
    }

    public class SpawnRuleData
    {
      public string RuleName;
      public string StationName;
      public SpawnSerialization.SpawnRuleOrbitData Orbit;
      public string LocationType;
      public string LocationTag;
      public double RespawnTimer_Minutes;
      public SpawnRange<int> NumberOfClusters;
      public List<SpawnSerialization.SpawnRuleSceneData> LocationScenes;
      public List<SpawnSerialization.SpawnRuleLootData> Loot;
      public bool IsVisibleOnRadar;
    }
  }
}
