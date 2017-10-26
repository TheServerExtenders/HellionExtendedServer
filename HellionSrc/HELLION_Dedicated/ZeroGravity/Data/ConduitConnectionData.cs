// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Data.ConduitConnectionData
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;

namespace ZeroGravity.Data
{
  [Serializable]
  public class ConduitConnectionData
  {
    public List<short> Power;
    public List<short> CPU;
    public List<short> Deuterium;
    public List<short> Oxygen;
    public List<short> Nitrogen;
    public List<short> Air;
  }
}
