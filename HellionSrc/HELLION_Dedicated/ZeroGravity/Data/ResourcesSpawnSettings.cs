// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.ResourcesSpawnSettings
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using ProtoBuf;
using System;

namespace ZeroGravity.Data
{
  [ProtoContract(ImplicitFields = ImplicitFields.AllPublic)]
  [Serializable]
  public class ResourcesSpawnSettings : ISceneData
  {
    public SpawnSettingsCase Case = SpawnSettingsCase.EnableIf;
    public string Tag;
    public float MinQuantity;
    public float MaxQuantity;
  }
}
