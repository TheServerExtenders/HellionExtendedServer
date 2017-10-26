// Decompiled with JetBrains decompiler
// Type: GameServerConsole.Scripts.Data.StashHelper
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ZeroGravity;

namespace GameServerConsole.Scripts.Data
{
  public static class StashHelper
  {
    public static string fileName = "PlayerStash.data";

    public static void Save(StashPlayerVessels data)
    {
      string str = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Server.ConfigDir);
      List<StashPlayerVessels> stashPlayerVesselsList;
      if ((uint) new DirectoryInfo(str).GetFiles(StashHelper.fileName).Length > 0U)
      {
        stashPlayerVesselsList = Json.Load<List<StashPlayerVessels>>(Path.Combine(str, StashHelper.fileName));
      }
      else
      {
        stashPlayerVesselsList = new List<StashPlayerVessels>();
        stashPlayerVesselsList.Add(data);
      }
      Json.SerializeToFile((object) stashPlayerVesselsList, Path.Combine(str, StashHelper.fileName), Json.Formatting.Indented);
    }

    public static StashPlayerVessels Load(long GUID)
    {
      string str = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), Server.ConfigDir);
      if ((uint) new DirectoryInfo(str).GetFiles(StashHelper.fileName).Length <= 0U)
        return (StashPlayerVessels) null;
      List<StashPlayerVessels> stashPlayerVesselsList = Json.Load<List<StashPlayerVessels>>(Path.Combine(str, StashHelper.fileName));
      StashPlayerVessels stashPlayerVessels = stashPlayerVesselsList.Find((Predicate<StashPlayerVessels>) (m => m.PlayerGUID == GUID));
      stashPlayerVesselsList.Remove(stashPlayerVessels);
      Json.SerializeToFile((object) stashPlayerVesselsList, Path.Combine(str, StashHelper.fileName), Json.Formatting.Indented);
      return stashPlayerVessels;
    }
  }
}
