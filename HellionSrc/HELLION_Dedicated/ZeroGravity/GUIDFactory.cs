// Decompiled with JetBrains decompiler
// Type: ZeroGravity.GUIDFactory
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;

namespace ZeroGravity
{
  public static class GUIDFactory
  {
    private static Random rand = new Random();

    public static long NextLongRandom(long min, long max)
    {
      byte[] buffer = new byte[8];
      GUIDFactory.rand.NextBytes(buffer);
      return Math.Abs(BitConverter.ToInt64(buffer, 0) % (max - min)) + min;
    }

    public static long NextObjectGUID()
    {
      long guid;
      do
      {
        guid = GUIDFactory.NextLongRandom(1000L, 999999L);
      }
      while (Server.Instance.DoesObjectExist(guid));
      return guid;
    }

    public static long NextPlayerFakeGUID()
    {
      long guid;
      do
      {
        guid = GUIDFactory.NextLongRandom(1000000000000L, long.MaxValue);
      }
      while (Server.Instance.DoesObjectExist(guid));
      return guid;
    }

    public static long NextVesselGUID()
    {
      long guid;
      do
      {
        guid = GUIDFactory.NextLongRandom(1000000L, 999999999999L);
      }
      while (Server.Instance.DoesObjectExist(guid));
      return guid;
    }

    public static long SteamIdToGuid(string steamId)
    {
      return long.MaxValue & (long) steamId.GetHashCode();
    }

    public static class Range
    {
      public const long PlayerFakeFrom = 1000000000000;
      public const long PlayerFakeTo = 9223372036854775807;
      public const long VesselFrom = 1000000;
      public const long VesselTo = 999999999999;
      public const long ObjectFrom = 1000;
      public const long ObjectTo = 999999;
      public const long SystemFrom = 100;
      public const long SystemTo = 999;
    }
  }
}
