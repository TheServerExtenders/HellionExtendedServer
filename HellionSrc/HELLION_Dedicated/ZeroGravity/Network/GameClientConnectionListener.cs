// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.GameClientConnectionListener
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ZeroGravity.Network
{
  public class GameClientConnectionListener
  {
    private volatile bool runThread;
    private Thread listeningThread;
    private TcpListener tcpListener;

    public void Stop()
    {
      this.runThread = false;
      this.tcpListener.Stop();
      this.listeningThread.Interrupt();
    }

    public void StopImmidiate()
    {
      if (!this.listeningThread.IsAlive)
        return;
      this.runThread = false;
      this.tcpListener.Stop();
      this.listeningThread.Interrupt();
      this.listeningThread.Abort();
    }

    public void Start(int port)
    {
      this.runThread = true;
      this.tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
      this.tcpListener.Start();
      this.listeningThread = new Thread(new ThreadStart(this.Listen));
      this.listeningThread.IsBackground = true;
      this.listeningThread.Start();
    }

    private void Listen()
    {
      try
      {
        while (Server.IsRunning && this.runThread)
        {
          Socket soc = this.tcpListener.AcceptSocket();
          if (!this.runThread)
            break;
          new GameClientThread(soc, 3f).Start();
        }
      }
      catch (Exception ex)
      {
      }
    }
  }
}
