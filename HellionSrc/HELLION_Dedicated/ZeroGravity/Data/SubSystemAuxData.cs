// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.SubSystemAuxData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ZeroGravity.Data
{
  public abstract class SubSystemAuxData : ISceneData
  {
    public abstract SubSystemAuxDataType AuxDataType { get; }

    public static object GetJsonData(JObject jo, JsonSerializer serializer)
    {
      SubSystemAuxDataType systemAuxDataType = (SubSystemAuxDataType) (int) jo["AuxDataType"];
      switch (systemAuxDataType)
      {
        case SubSystemAuxDataType.PDU:
          return (object) jo.ToObject<SubSystemPDUAuxData>(serializer);
        case SubSystemAuxDataType.AirDevice:
          return (object) jo.ToObject<SubSystemAirDevicesAuxData>(serializer);
        case SubSystemAuxDataType.ScrubberDevice:
          return (object) jo.ToObject<SubSystemScrubberDeviceAuxData>(serializer);
        case SubSystemAuxDataType.ACDevice:
          return (object) jo.ToObject<SubSystemACDeviceAuxData>(serializer);
        case SubSystemAuxDataType.RCS:
          return (object) jo.ToObject<SubSystemRCSAuxData>(serializer);
        case SubSystemAuxDataType.Engine:
          return (object) jo.ToObject<SubSystemEngineAuxData>(serializer);
        case SubSystemAuxDataType.FTL:
          return (object) jo.ToObject<SubSystemFTLAuxData>(serializer);
        case SubSystemAuxDataType.Capacitor:
          return (object) jo.ToObject<GeneratorCapacitorAuxData>(serializer);
        case SubSystemAuxDataType.PowerGenerator:
          return (object) jo.ToObject<GeneratorPowerAuxData>(serializer);
        case SubSystemAuxDataType.Refinery:
          return (object) jo.ToObject<SubSystemRefineryAuxData>(serializer);
        case SubSystemAuxDataType.Solar:
          return (object) jo.ToObject<GeneratorSolarAuxData>(serializer);
        case SubSystemAuxDataType.ScrubbedAirGenerator:
          return (object) jo.ToObject<GeneratorScrubbedAirAuxData>(serializer);
        case SubSystemAuxDataType.Fabricator:
          return (object) jo.ToObject<SubSystemFabricatorAuxData>(serializer);
        default:
          throw new Exception("Json deserializer was not implemented for item type " + systemAuxDataType.ToString());
      }
    }
  }
}
