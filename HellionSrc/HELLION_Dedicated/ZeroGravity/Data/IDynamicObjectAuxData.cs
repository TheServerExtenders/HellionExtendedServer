// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.IDynamicObjectAuxData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace ZeroGravity.Data
{
  public abstract class IDynamicObjectAuxData
  {
    public ItemType ItemType;
    public float MaxHealth;
    public float Health;
    public bool Repairable;
    public float MeleeDamage;

    public static object GetJsonData(JObject jo, JsonSerializer serializer)
    {
      ItemType type = (ItemType) (int) jo["ItemType"];
      if (ItemTypeRange.IsMachineryPart(type))
        return (object) jo.ToObject<MachineryPartData>(serializer);
      if (ItemTypeRange.IsHelmet(type))
        return (object) jo.ToObject<HelmetData>(serializer);
      if (ItemTypeRange.IsJetpack(type))
        return (object) jo.ToObject<JetpackData>(serializer);
      if (ItemTypeRange.IsWeapon(type))
        return (object) jo.ToObject<WeaponData>(serializer);
      if (ItemTypeRange.IsMelee(type))
        return (object) jo.ToObject<MeleeWeaponData>(serializer);
      if (ItemTypeRange.IsAmmo(type))
        return (object) jo.ToObject<MagazineData>(serializer);
      if (ItemTypeRange.IsOutfit(type))
        return (object) jo.ToObject<OutfitData>(serializer);
      if (ItemTypeRange.IsBattery(type))
        return (object) jo.ToObject<BatteryData>(serializer);
      if (ItemTypeRange.IsCanister(type))
        return (object) jo.ToObject<CanisterData>(serializer);
      if (ItemTypeRange.IsDrill(type))
        return (object) jo.ToObject<HandDrillData>(serializer);
      if (ItemTypeRange.IsGlowStick(type))
        return (object) jo.ToObject<GlowStickData>(serializer);
      if (ItemTypeRange.IsMedpack(type))
        return (object) jo.ToObject<MedpackData>(serializer);
      if (ItemTypeRange.IsHackingTool(type))
        return (object) jo.ToObject<DisposableHackingToolData>(serializer);
      if (ItemTypeRange.IsAsteroidScanningTool(type))
        return (object) jo.ToObject<HandheldAsteroidScannerData>(serializer);
      if (ItemTypeRange.IsLogItem(type))
        return (object) jo.ToObject<LogItemData>(serializer);
      if (ItemTypeRange.IsGenericItem(type))
        return (object) jo.ToObject<GenericItemData>(serializer);
      if (ItemTypeRange.IsGrenade(type))
        return (object) jo.ToObject<GrenadeData>(serializer);
      if (ItemTypeRange.IsPortableTurret(type))
        return (object) jo.ToObject<PortableTurretData>(serializer);
      if (ItemTypeRange.IsRepairTool(type))
        return (object) jo.ToObject<RepairToolData>(serializer);
      throw new Exception("Json deserializer was not implemented for item type " + type.ToString());
    }
  }
}
