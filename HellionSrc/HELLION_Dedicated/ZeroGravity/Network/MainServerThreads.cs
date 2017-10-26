// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.MainServerThreads
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;

namespace ZeroGravity.Network
{
  public class MainServerThreads
  {
    public MainServerThreads(int maxPool = 5)
    {
      ThreadPool.SetMaxThreads(maxPool, maxPool);
    }

    public void Send(NetworkData data)
    {
      ThreadPool.QueueUserWorkItem(new WaitCallback(this.ThreadPoolCallback), (object) data);
    }

    private void ThreadPoolCallback(object data)
    {
      NetworkData data1 = data as NetworkData;
      try
      {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Connect(Server.Instance.NetworkController.MainServerAddres, Server.Instance.NetworkController.MainServerPort);
        Stream str = (Stream) new NetworkStream(socket);
        byte[] buffer = Serializer.Serialize(data1);
        str.Write(buffer, 0, buffer.Length);
        str.Flush();
        try
        {
          NetworkData data2 = Serializer.ReceiveData(str);
          if (data2 == null)
            return;
          Server.Instance.NetworkController.EventSystem.Invoke(data2);
        }
        catch (SocketException ex)
        {
          Dbg.Error((object) "Connection broken", (object) ex.Message, (object) ex.StackTrace);
        }
        catch (Serializer.ZeroDataException ex)
        {
        }
      }
      catch (Exception ex)
      {
        Dbg.Error((object) "Unable to connect to main server", (object) ex.Message, (object) ex.StackTrace);
        if (Server.CheckInPassed)
          return;
        Server.IsRunning = false;
      }
    }
  }
}
