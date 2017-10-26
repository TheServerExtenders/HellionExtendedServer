// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Json
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using Newtonsoft.Json;
using System;
using System.IO;

namespace ZeroGravity
{
  public static class Json
  {
    private static AuxDataJsonConverter auxDataJsonConverter = new AuxDataJsonConverter();
    private static AttachPointDataJsonConverter attachPointDataJsonConverter = new AttachPointDataJsonConverter();
    private static PersistenceJsonConverter persistenceJsonConverter = new PersistenceJsonConverter();

    public static string Serialize(object obj, ZeroGravity.Json.Formatting format = ZeroGravity.Json.Formatting.Indented)
    {
      return JsonConvert.SerializeObject(obj, (Newtonsoft.Json.Formatting) format);
    }

    public static void SerializeToFile(object obj, string filePath, ZeroGravity.Json.Formatting format = ZeroGravity.Json.Formatting.Indented)
    {
      File.WriteAllText(filePath, JsonConvert.SerializeObject(obj, (Newtonsoft.Json.Formatting) format, new JsonSerializerSettings()
      {
        NullValueHandling = NullValueHandling.Ignore
      }));
    }

    public static void SerializeToFile(object obj, string filePath, ZeroGravity.Json.Formatting format = ZeroGravity.Json.Formatting.Indented, JsonSerializerSettings settings = null)
    {
      File.WriteAllText(filePath, JsonConvert.SerializeObject(obj, (Newtonsoft.Json.Formatting) format, settings));
    }

    public static T Deserialize<T>(string jsonString)
    {
      return JsonConvert.DeserializeObject<T>(jsonString, (JsonConverter) ZeroGravity.Json.auxDataJsonConverter, (JsonConverter) ZeroGravity.Json.attachPointDataJsonConverter, (JsonConverter) ZeroGravity.Json.persistenceJsonConverter);
    }

    public static T Load<T>(string filePath)
    {
      DateTime utcNow1 = DateTime.UtcNow;
      T obj = JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath), (JsonConverter) ZeroGravity.Json.auxDataJsonConverter, (JsonConverter) ZeroGravity.Json.attachPointDataJsonConverter, (JsonConverter) ZeroGravity.Json.persistenceJsonConverter);
      DateTime utcNow2 = DateTime.UtcNow;
      return obj;
    }

    public enum Formatting
    {
      None,
      Indented,
    }
  }
}
