// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.StatusPortConnectionListener
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ZeroGravity.Network
{
  public class StatusPortConnectionListener
  {
    private volatile bool runThread;
    private Thread listeningThread;
    private TcpListener tcpListener;

    public void Stop()
    {
      this.runThread = false;
      this.tcpListener.Stop();
    }

    public void Start(int port)
    {
      this.runThread = true;
      this.tcpListener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
      this.tcpListener.Start();
      this.listeningThread = new Thread(new ThreadStart(this.Listen));
      this.listeningThread.IsBackground = true;
      this.listeningThread.Priority = ThreadPriority.AboveNormal;
      this.listeningThread.Start();
    }

    private void Listen()
    {
      while (Server.IsRunning && this.runThread)
      {
        try
        {
          Socket soc = this.tcpListener.AcceptSocket();
          if (!this.runThread)
            break;
          new StatusPortThread(soc).Start();
        }
        catch (Exception ex)
        {
        }
      }
    }
  }
}
