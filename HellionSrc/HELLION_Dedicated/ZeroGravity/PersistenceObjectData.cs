// Decompiled with JetBrains decompiler
// Type: ZeroGravity.PersistenceObjectData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ZeroGravity
{
  public abstract class PersistenceObjectData
  {
    public long GUID;

    public virtual Persistence.ObjectType Type
    {
      get
      {
        return Persistence.ObjectType.NONE;
      }
    }

    public static PersistenceObjectData GetPersistenceData(JObject jo, JsonSerializer serializer)
    {
      Persistence.ObjectType objectType = (Persistence.ObjectType) (int) jo["Type"];
      switch (objectType)
      {
        case Persistence.ObjectType.Ship:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataShip>(serializer);
        case Persistence.ObjectType.Asteroid:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataAsteroid>(serializer);
        case Persistence.ObjectType.DynamicObject:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataDynamicObject>(serializer);
        case Persistence.ObjectType.Item:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataItem>(serializer);
        case Persistence.ObjectType.MachineryPart:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataMachineryPart>(serializer);
        case Persistence.ObjectType.Jetpack:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataJetpack>(serializer);
        case Persistence.ObjectType.Canister:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataCanister>(serializer);
        case Persistence.ObjectType.Outfit:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataOutfit>(serializer);
        case Persistence.ObjectType.Helmet:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataHelmet>(serializer);
        case Persistence.ObjectType.Cargo:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataCargo>(serializer);
        case Persistence.ObjectType.VesselComponent:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataVesselComponent>(serializer);
        case Persistence.ObjectType.Player:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataPlayer>(serializer);
        case Persistence.ObjectType.Weapon:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataWeapon>(serializer);
        case Persistence.ObjectType.Magazine:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataMagazine>(serializer);
        case Persistence.ObjectType.MeleeWeapon:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataMeleeWeapon>(serializer);
        case Persistence.ObjectType.GlowStick:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataGlowStick>(serializer);
        case Persistence.ObjectType.Medpack:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataMedpack>(serializer);
        case Persistence.ObjectType.HandheldAsteroidScanner:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataHandheldAsteroidScanner>(serializer);
        case Persistence.ObjectType.HandDrill:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataHandDrill>(serializer);
        case Persistence.ObjectType.Battery:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataBattery>(serializer);
        case Persistence.ObjectType.HackingTool:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataHackingTool>(serializer);
        case Persistence.ObjectType.RespawnObject:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataRespawnObject>(serializer);
        case Persistence.ObjectType.SpawnPoint:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataSpawnPoint>(serializer);
        case Persistence.ObjectType.Room:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataRoom>(serializer);
        case Persistence.ObjectType.LogItem:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataLogItem>(serializer);
        case Persistence.ObjectType.ArenaController:
          return (PersistenceObjectData) jo.ToObject<PersistenceArenaControllerData>(serializer);
        case Persistence.ObjectType.GenericItem:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataGenericItem>(serializer);
        case Persistence.ObjectType.Door:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataGrenade>(serializer);
        case Persistence.ObjectType.DoomController:
          return (PersistenceObjectData) jo.ToObject<PersistenceDataDoomController>(serializer);
        case Persistence.ObjectType.PortableTurret:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataPortableTurret>(serializer);
        case Persistence.ObjectType.SpawnManager:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataSpawnManager>(serializer);
        case Persistence.ObjectType.PlayerStash:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataSpawnManager>(serializer);
        case Persistence.ObjectType.RepairPoint:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataRepairPoint>(serializer);
        case Persistence.ObjectType.RepairTool:
          return (PersistenceObjectData) jo.ToObject<PersistenceObjectDataRepairTool>(serializer);
        default:
          Dbg.Error((object) "Could not deseralize persistence data", (object) objectType);
          return (PersistenceObjectData) null;
      }
    }
  }
}
