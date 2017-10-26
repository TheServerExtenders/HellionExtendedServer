// Decompiled with JetBrains decompiler
// Type: ZeroGravity.AttachPointDataJsonConverter
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using ZeroGravity.Data;

namespace ZeroGravity
{
  public class AttachPointDataJsonConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType)
    {
      return objectType == typeof (BaseAttachPointData);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
        return (object) null;
      try
      {
        JObject jobject = JObject.Load(reader);
        switch ((AttachPointType) (int) jobject["AttachPointType"])
        {
          case AttachPointType.Simple:
            return (object) jobject.ToObject<AttachPointData>(serializer);
          case AttachPointType.MachineryPartSlot:
            return (object) jobject.ToObject<MachineryPartSlotData>(serializer);
          case AttachPointType.ResourcesTransferPoint:
            return (object) jobject.ToObject<ResourcesTransferPointData>(serializer);
          case AttachPointType.ResourcesAutoTransferPoint:
            return (object) jobject.ToObject<ResourcesAutoTransferPointData>(serializer);
          case AttachPointType.BatteryRechargePoint:
            return (object) jobject.ToObject<BatteryRechargePointData>(serializer);
        }
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
