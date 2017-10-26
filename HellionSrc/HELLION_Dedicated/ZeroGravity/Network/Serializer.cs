// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.Serializer
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;

namespace ZeroGravity.Network
{
  public static class Serializer
  {
    private static double statisticsLogUpdateTime = -1.0;
    private static Dictionary<Type, Serializer.StatisticsHelper> statistics = new Dictionary<Type, Serializer.StatisticsHelper>();
    public const int SizeOfMessageLength = 4;
    private static DateTime lastStatisticUpdateTime;

    public static NetworkData Deserialize(MemoryStream ms)
    {
      NetworkData networkData = (NetworkData) null;
      ms.Position = 0L;
      try
      {
        networkData = ProtoBuf.Serializer.Deserialize<NetworkDataTransportWrapper>((Stream) ms).data;
      }
      catch (Exception ex)
      {
        Dbg.Error((object) "Failed to deserialize communication data", (object) ex.Message, (object) ex.StackTrace);
      }
      return networkData;
    }

    public static NetworkData ReceiveData(Socket soc)
    {
      if (soc == null || !soc.Connected)
        return (NetworkData) null;
      return Serializer.ReceiveData((Stream) new NetworkStream(soc));
    }

    public static NetworkData ReceiveData(Stream str)
    {
      byte[] buffer1 = new byte[4];
      int offset1 = 0;
      do
      {
        int num = str.Read(buffer1, offset1, buffer1.Length - offset1);
        if (num == 0)
          throw new Serializer.ZeroDataException("Received zero data message.");
        offset1 += num;
      }
      while (offset1 < buffer1.Length);
      byte[] buffer2 = new byte[(int) BitConverter.ToUInt32(buffer1, 0)];
      int offset2 = 0;
      do
      {
        int num = str.Read(buffer2, offset2, buffer2.Length - offset2);
        if (num == 0)
          throw new Serializer.ZeroDataException("Received zero data message.");
        offset2 += num;
      }
      while (offset2 < buffer2.Length);
      using (MemoryStream ms = new MemoryStream())
      {
        ms.Write(buffer2, 0, buffer2.Length);
        return Serializer.Deserialize(ms);
      }
    }

    public static byte[] Serialize(NetworkData data)
    {
      using (MemoryStream memoryStream1 = new MemoryStream())
      {
        using (MemoryStream memoryStream2 = new MemoryStream())
        {
          try
          {
            NetworkDataTransportWrapper instance = new NetworkDataTransportWrapper()
            {
              data = data
            };
            ProtoBuf.Serializer.Serialize<NetworkDataTransportWrapper>((Stream) memoryStream2, instance);
          }
          catch (Exception ex)
          {
            Dbg.Error((object) "Failed to serialize communication data", (object) ex.Message, (object) ex.StackTrace);
            return (byte[]) null;
          }
          if (Serializer.statisticsLogUpdateTime > 0.0)
          {
            Type type = data.GetType();
            if (Serializer.statistics.ContainsKey(type))
            {
              Serializer.statistics[type].ByteSum += memoryStream2.Length;
              ++Serializer.statistics[type].PacketNubmer;
              Serializer.statistics[type].BytesSinceLastCheck += memoryStream2.Length;
            }
            else
              Serializer.statistics[type] = new Serializer.StatisticsHelper(memoryStream2.Length);
            DateTime utcNow = DateTime.UtcNow;
            TimeSpan timeSpan = utcNow.Subtract(Serializer.lastStatisticUpdateTime);
            if (timeSpan.TotalSeconds >= Serializer.statisticsLogUpdateTime)
            {
              string str = "Serialize packet statistics: \n";
              foreach (KeyValuePair<Type, Serializer.StatisticsHelper> statistic in Serializer.statistics)
              {
                double num1 = (double) statistic.Value.BytesSinceLastCheck / 1024.0;
                utcNow = DateTime.UtcNow;
                timeSpan = utcNow.Subtract(Serializer.lastStatisticUpdateTime);
                double totalSeconds = timeSpan.TotalSeconds;
                double num2 = num1 / totalSeconds;
                str = str + (object) statistic.Key + ": " + num2.ToString("0.0") + " (" + (object) (int) ((double) statistic.Value.ByteSum / 1024.0) + "), \n";
                statistic.Value.BytesSinceLastCheck = 0L;
              }
              Serializer.lastStatisticUpdateTime = DateTime.UtcNow;
            }
          }
          memoryStream1.Write(BitConverter.GetBytes((uint) memoryStream2.Length), 0, 4);
          memoryStream1.Write(memoryStream2.ToArray(), 0, (int) memoryStream2.Length);
          memoryStream1.Flush();
          return memoryStream1.ToArray();
        }
      }
    }

    public class ZeroDataException : Exception
    {
      public ZeroDataException(string message)
        : base(message)
      {
      }
    }

    public class CorruptedPackageException : Exception
    {
      public CorruptedPackageException(string message)
        : base(message)
      {
      }
    }

    public class StatisticsHelper
    {
      public long ByteSum;
      public int PacketNubmer;
      public long BytesSinceLastCheck;

      public StatisticsHelper(long bytes)
      {
        this.ByteSum = bytes;
        this.PacketNubmer = 1;
        this.BytesSinceLastCheck = bytes;
      }
    }
  }
}
