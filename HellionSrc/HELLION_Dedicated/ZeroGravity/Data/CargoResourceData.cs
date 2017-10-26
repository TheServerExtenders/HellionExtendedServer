// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.CargoResourceData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using System;
using System.Collections.Generic;

namespace ZeroGravity.Data
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  [Serializable]
  public class CargoResourceData : ISceneData
  {
    public ResourceType ResourceType;
    public float Quantity;
    public ResourcesSpawnSettings[] SpawnSettings;

    public static Dictionary<ResourceType, float> ListToDictionary(List<CargoResourceData> list)
    {
      Dictionary<ResourceType, float> dictionary = new Dictionary<ResourceType, float>();
      if (list != null)
      {
        foreach (CargoResourceData cargoResourceData in list)
          dictionary[cargoResourceData.ResourceType] = cargoResourceData.Quantity;
      }
      return dictionary;
    }

    public static List<CargoResourceData> DictionaryToList(Dictionary<ResourceType, float> dictionary)
    {
      List<CargoResourceData> cargoResourceDataList = new List<CargoResourceData>();
      if (dictionary != null)
      {
        foreach (KeyValuePair<ResourceType, float> keyValuePair in dictionary)
          cargoResourceDataList.Add(new CargoResourceData()
          {
            ResourceType = keyValuePair.Key,
            Quantity = keyValuePair.Value
          });
      }
      return cargoResourceDataList;
    }
  }
}
