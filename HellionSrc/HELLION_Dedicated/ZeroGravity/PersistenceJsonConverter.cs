// Decompiled with JetBrains decompiler
// Type: ZeroGravity.PersistenceJsonConverter
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ZeroGravity
{
  public class PersistenceJsonConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof (PersistenceObjectData);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
        return (object) null;
      try
      {
        if (objectType == typeof (PersistenceObjectData))
          return (object) PersistenceObjectData.GetPersistenceData(JObject.Load(reader), serializer);
      }
      catch (Exception ex)
      {
        Dbg.Exception(ex);
      }
      return (object) null;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      serializer.Serialize(writer, value);
    }
  }
}
