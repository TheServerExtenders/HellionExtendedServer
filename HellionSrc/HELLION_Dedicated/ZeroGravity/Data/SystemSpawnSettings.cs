// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.SystemSpawnSettings
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;

namespace ZeroGravity.Data
{
  [Serializable]
  public class SystemSpawnSettings : ISceneData
  {
    public SpawnSettingsCase Case = SpawnSettingsCase.EnableIf;
    public float ResourceRequirementMultiplier = 1f;
    public string Tag;
  }
}
