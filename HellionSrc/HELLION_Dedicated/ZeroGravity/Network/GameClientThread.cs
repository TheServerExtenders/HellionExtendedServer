// Decompiled with JetBrains decompiler
// Type: ZeroGravity.Network.GameClientThread
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Net.Sockets;
using System.Threading;
using ZeroGravity.Helpers;

namespace ZeroGravity.Network
{
  public class GameClientThread
  {
    private EventWaitHandle waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
    private ThreadSafeQueue<NetworkData> NetworkDataQueue = new ThreadSafeQueue<NetworkData>();
    private volatile bool runThread;
    private Socket socket;
    private Thread listeningThread;
    private Thread sendingThread;
    private NetworkController.Client client;
    private float timeOut;

    public GameClientThread(Socket soc, float time = 3f)
    {
      this.socket = soc;
      this.timeOut = time;
    }

    public void Send(NetworkData data)
    {
      this.NetworkDataQueue.EnqueueSafe(data);
      this.waitHandle.Set();
    }

    public void ClearEverytingAndSend(NetworkData data)
    {
      this.NetworkDataQueue.Clear();
      this.NetworkDataQueue.EnqueueSafe(data);
      this.waitHandle.Set();
    }

    public void Start()
    {
      this.runThread = true;
      this.sendingThread = new Thread(new ThreadStart(this.Send));
      this.sendingThread.IsBackground = true;
      this.sendingThread.Start();
      this.listeningThread = new Thread(new ThreadStart(this.Listen));
      this.listeningThread.IsBackground = true;
      this.listeningThread.Start();
    }

    public void Stop()
    {
      this.runThread = false;
      this.waitHandle.Set();
      if (this.socket != null)
        this.socket.Close();
      this.listeningThread.Interrupt();
      this.sendingThread.Interrupt();
    }

    public void StopImmediate()
    {
      this.runThread = false;
      this.waitHandle.Set();
      if (this.socket != null)
        this.socket.Close();
      this.listeningThread.Interrupt();
      this.listeningThread.Abort();
      this.sendingThread.Interrupt();
      this.sendingThread.Abort();
    }

    private void Send()
    {
      while (Server.IsRunning && this.runThread)
      {
        try
        {
          this.waitHandle.WaitOne();
        }
        catch
        {
        }
        if (Server.IsRunning && this.runThread)
        {
          while (this.NetworkDataQueue.Count > 0)
          {
            NetworkData data;
            try
            {
              data = this.NetworkDataQueue.DequeueSafe();
            }
            catch (Exception ex)
            {
              Dbg.Info((object) "Problem occured while dequeueing network data", (object) ex.Message, (object) ex.StackTrace);
              this.Disconnect();
              return;
            }
            if (data is LogOutResponse)
            {
              byte[] buffer = Serializer.Serialize(data);
              try
              {
                if (!this.socket.BeginSend(buffer, 0, buffer.Length, SocketFlags.None, (AsyncCallback) null, (object) null).AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds((double) this.timeOut), false))
                  throw new TimeoutException();
              }
              catch (TimeoutException ex)
              {
              }
              catch (Exception ex)
              {
                if (!(ex is SocketException) && !(ex is ObjectDisposedException))
                  Dbg.Error((object) "SendToGameClient logout exception", (object) (this.client == null || this.client.Player == null ? "" : this.client.Player.GUID.ToString()), (object) ex.Message, (object) ex.StackTrace);
              }
              finally
              {
                this.Disconnect();
              }
            }
            else
            {
              try
              {
                this.socket.Send(Serializer.Serialize(data));
              }
              catch (ArgumentNullException ex)
              {
                Dbg.Error((object) "Serialized data buffer is null", (object) data.GetType().ToString(), (object) data);
              }
              catch (Exception ex)
              {
                if (!(ex is SocketException) && !(ex is ObjectDisposedException))
                  Dbg.Error((object) "SendToGameClient exception", (object) (this.client == null || this.client.Player == null ? "" : this.client.Player.GUID.ToString()), (object) ex.Message, (object) ex.StackTrace);
                this.Disconnect();
              }
            }
          }
        }
        else
          break;
      }
      if (this.client == null || this.client.Player == null)
        ;
    }

    private void Disconnect()
    {
      Server.Instance.NetworkController.DisconnectClient(this.client);
    }

    private void Listen()
    {
      this.client = Server.Instance.NetworkController.AddClient(this.socket, this);
      try
      {
        while (Server.IsRunning && this.runThread)
        {
          NetworkData data = Serializer.ReceiveData(this.socket);
          if (data != null)
          {
            data.Sender = this.client.Player == null ? Server.Instance.NetworkController.GetClientGuid(this.client) : this.client.Player.GUID;
            Server.Instance.NetworkController.EventSystem.Invoke(data);
          }
          else
            break;
        }
      }
      catch (Exception ex)
      {
        this.Disconnect();
      }
      if (this.client == null || this.client.Player == null)
        ;
    }
  }
}
