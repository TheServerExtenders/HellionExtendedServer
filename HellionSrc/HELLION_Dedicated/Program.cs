// Decompiled with JetBrains decompiler
// Type: Program
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using ZeroGravity;
using ZeroGravity.Network;

public static class Program
{
  private static Program.EventHandler _handler;

  [DllImport("Kernel32")]
  private static extern bool SetConsoleCtrlHandler(Program.EventHandler handler, bool add);

  private static bool Handler(Program.CtrlType sig)
  {
    if (sig == Program.CtrlType.CTRL_C_EVENT || sig == Program.CtrlType.CTRL_BREAK_EVENT || (sig == Program.CtrlType.CTRL_LOGOFF_EVENT || sig == Program.CtrlType.CTRL_SHUTDOWN_EVENT) || sig == Program.CtrlType.CTRL_CLOSE_EVENT)
    {
      Server.IsRunning = false;
      if (Server.PersistenceSaveInterval > 0.0)
        Server.SavePersistenceDataOnShutdown = true;
      Server.MainLoopEnded.WaitOne(5000);
    }
    return false;
  }

  private static void Main(string[] args)
  {
    Program._handler += new Program.EventHandler(Program.Handler);
    Program.SetConsoleCtrlHandler(Program._handler, true);
    bool flag = false;
    string str1 = (string) null;
    string str2 = (string) null;
    string str3 = (string) null;
    for (int index = 0; index < args.Length; ++index)
    {
      if (args[index].ToLower() == "-configdir" && args.Length > index + 1)
      {
        Server.ConfigDir = args[++index];
        if (!Server.ConfigDir.EndsWith("/"))
          Server.ConfigDir += "/";
      }
      else if (args[index].ToLower() == "-gport" && args.Length > index + 1)
        str1 = args[++index];
      else if (args[index].ToLower() == "-sport" && args.Length > index + 1)
        str2 = args[++index];
      else if (args[index].ToLower() == "-randomships" && args.Length > index + 1)
        str3 = args[++index];
      else if (args[index].ToLower() == "-shutdown")
        flag = true;
      else if (args[index].ToLower() == "-scan")
      {
        Program.ScanInstances();
        Environment.Exit(0);
      }
      else if (args[index].ToLower() == "-cleanup")
      {
        Program.CleanupAllInstances();
        Environment.Exit(0);
      }
      else if (args[index].ToLower() == "-clean" || args[index].ToLower() == "-noload")
        Server.CleanStart = true;
    }
    Server.Properties = new Properties(Server.ConfigDir + "GameServer.ini");
    if (str1 != null)
      Server.Properties.SetProperty("game_client_port", str1);
    if (str2 != null)
      Server.Properties.SetProperty("status_port", str2);
    if (str3 != null)
      Server.Properties.SetProperty("spawn_random_ships_count", str3);
    if (flag)
    {
      Program.ShutdownServerInstance();
    }
    else
    {
      Program.CheckIniFields();
      Dbg.OutputDir = Server.ConfigDir;
      Dbg.Initialize();
      AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(Program.UnhandledExceptionHandler);
      try
      {
        new Server().MainLoop();
      }
      catch (Exception ex)
      {
        Dbg.UnformattedMessage("******************** MAIN EXCEPTION ********************");
        Dbg.Exception(ex);
      }
    }
  }

  private static void CheckIniFields()
  {
    try
    {
      if (Server.Properties.GetProperty<string>("server_name").Trim().IsNullOrEmpty())
        throw new Exception();
    }
    catch
    {
      Console.Out.WriteLine("Invalid 'server_name' field.");
      Environment.Exit(0);
    }
    try
    {
      int property = (int) Server.Properties.GetProperty<ushort>("game_client_port");
    }
    catch
    {
      Console.Out.WriteLine("Invalid 'game_client_port' field.");
      Environment.Exit(0);
    }
    try
    {
      int property = (int) Server.Properties.GetProperty<ushort>("status_port");
    }
    catch
    {
      Console.Out.WriteLine("Invalid 'status_port' field.");
      Environment.Exit(0);
    }
  }

  private static void CleanupAllInstances()
  {
    foreach (FileSystemInfo enumerateFile in new DirectoryInfo(".").EnumerateFiles("*.save"))
      enumerateFile.Delete();
    foreach (string directory in Directory.GetDirectories("."))
    {
      string fileName = Path.GetFileName(directory);
      if (File.Exists(fileName + "/GameServer.ini"))
      {
        foreach (FileSystemInfo enumerateFile in new DirectoryInfo(fileName).EnumerateFiles("*.save"))
          enumerateFile.Delete();
      }
    }
  }

  private static void ScanInstances()
  {
    DirectoryInfo directoryInfo = new DirectoryInfo(".");
    foreach (FileSystemInfo enumerateFile in directoryInfo.EnumerateFiles("Start_*.bat"))
      enumerateFile.Delete();
    foreach (FileSystemInfo enumerateFile in directoryInfo.EnumerateFiles("Stop_*.bat"))
      enumerateFile.Delete();
    string fileName1 = Path.GetFileName(Process.GetCurrentProcess().MainModule.FileName);
    string contents1 = "start cmd /c call \"Start_DEFAULT.bat\"\r\n";
    string contents2 = "start cmd /c call \"Stop_DEFAULT.bat\"\r\n";
    File.WriteAllText("Start_DEFAULT.bat", "@TITLE Game Server: DEFAULT \r\n@" + fileName1);
    File.WriteAllText("Stop_DEFAULT.bat", fileName1 + " -shutdown");
    foreach (string directory in Directory.GetDirectories("."))
    {
      string fileName2 = Path.GetFileName(directory);
      if (File.Exists(fileName2 + "/GameServer.ini"))
      {
        string contents3 = "@TITLE Game Server: " + fileName2 + "\r\n@" + fileName1 + " -configdir \"" + fileName2 + "\"";
        string contents4 = contents3 + " -shutdown";
        string path1 = "Start_" + fileName2 + ".bat";
        string path2 = "Stop_" + fileName2 + ".bat";
        File.WriteAllText(path1, contents3);
        File.WriteAllText(path2, contents4);
        contents1 = contents1 + "start cmd /c call \"" + path1 + "\"\r\n";
        contents2 = contents2 + "start cmd /c call \"" + path2 + "\"\r\n";
      }
    }
    File.WriteAllText("Start_ALL.bat", contents1);
    File.WriteAllText("Stop_ALL.bat", contents2);
  }

  private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
  {
    Dbg.UnformattedMessage("******************** UNHANDLED EXCEPTION ********************");
    Dbg.Exception((Exception) args.ExceptionObject);
  }

  private static void ShutdownServerInstance()
  {
    int property = Server.Properties.GetProperty<int>("status_port");
    if (property <= 0)
      return;
    try
    {
      TcpClient tcpClient = new TcpClient();
      IAsyncResult asyncResult = tcpClient.BeginConnect("127.0.0.1", property, (AsyncCallback) null, (object) null);
      WaitHandle asyncWaitHandle = asyncResult.AsyncWaitHandle;
      try
      {
        if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2.0), false))
        {
          tcpClient.Close();
          throw new TimeoutException();
        }
        tcpClient.EndConnect(asyncResult);
      }
      finally
      {
        asyncWaitHandle.Close();
      }
      NetworkStream stream;
      try
      {
        stream = tcpClient.GetStream();
      }
      catch
      {
        return;
      }
      stream.ReadTimeout = 1000;
      stream.WriteTimeout = 1000;
      byte[] buffer = Serializer.Serialize((NetworkData) new ServerShutDownMessage());
      stream.Write(buffer, 0, buffer.Length);
      stream.Flush();
      stream.Close();
      tcpClient.Close();
    }
    catch
    {
    }
  }

  private enum CtrlType
  {
    CTRL_C_EVENT = 0,
    CTRL_BREAK_EVENT = 1,
    CTRL_CLOSE_EVENT = 2,
    CTRL_LOGOFF_EVENT = 5,
    CTRL_SHUTDOWN_EVENT = 6,
  }

  private delegate bool EventHandler(Program.CtrlType sig);
}
