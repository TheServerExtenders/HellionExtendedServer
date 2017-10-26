// Decompiled with JetBrains decompiler
// Type: ZeroGravity.AuxDataJsonConverter
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using ZeroGravity.Data;

namespace ZeroGravity
{
  public class AuxDataJsonConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof (IDynamicObjectAuxData) || objectType == typeof (SubSystemAuxData);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
        return (object) null;
      try
      {
        if (objectType == typeof (IDynamicObjectAuxData))
          return IDynamicObjectAuxData.GetJsonData(JObject.Load(reader), serializer);
        if (objectType == typeof (SubSystemAuxData))
          return SubSystemAuxData.GetJsonData(JObject.Load(reader), serializer);
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
