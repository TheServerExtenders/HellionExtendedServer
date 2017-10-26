// Decompiled with JetBrains decompiler
// Type: Dbg
// Assembly: HELLION_Dedicated, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3DB91419-85E5-4D49-97FF-EABAD43D2995
// Assembly location: F:\Hellion\HellionExtendedServer\GameLibraries\HELLION_Dedicated.exe

using System;
using System.Diagnostics;
using System.IO;

public static class Dbg
{
  public static string OutputDir = "";
  public static bool AddTimestamp = true;
  public static string TimestampFormat = "yyyy/MM/dd HH:mm:ss.ffff";
  public static string TimestampSeparator = " - ";

  public static void Initialize()
  {
    Trace.Listeners.Clear();
    TextWriterTraceListener writerTraceListener = new TextWriterTraceListener((TextWriter) new StreamWriter(Dbg.OutputDir + "output_log.txt", false));
    writerTraceListener.TraceOutputOptions = TraceOptions.None;
    ConsoleTraceListener consoleTraceListener = new ConsoleTraceListener(false);
    consoleTraceListener.TraceOutputOptions = TraceOptions.None;
    Trace.Listeners.Add((TraceListener) writerTraceListener);
    Trace.Listeners.Add((TraceListener) consoleTraceListener);
    Trace.AutoFlush = true;
  }

  public static void Destroy()
  {
    Trace.Listeners.Clear();
  }

  private static void WriteToLog(string message)
  {
    try
    {
      StackFrame frame = new StackTrace(2, true).GetFrame(0);
      Trace.WriteLine(string.Format("{0}\r\n\t{1} Ln {2} Col {3}\r\n------------------------------------------------------------------------------", (object) message, (object) Path.GetFileName(frame.GetFileName()), (object) frame.GetFileLineNumber(), (object) frame.GetFileColumnNumber()));
    }
    catch
    {
    }
  }

  private static string ObjectParamsToString(params object[] values)
  {
    string str = "";
    for (int index = 0; index < values.Length; ++index)
      str = str + (index > 0 ? ", " : "") + Dbg.GetString(values[index]);
    return str;
  }

  private static string GetString(object value)
  {
    return value != null ? value.ToString() : "NULL";
  }

  [Conditional("DEBUG")]
  [Conditional("SHOW_ALL_LOGS")]
  public static void Log(string message)
  {
    Dbg.WriteToLog(message);
  }

  [Conditional("DEBUG")]
  [Conditional("SHOW_ALL_LOGS")]
  public static void Log(params object[] values)
  {
    if (values.Length == 1)
      Dbg.WriteToLog(Dbg.GetString(values[0]));
    else
      Dbg.WriteToLog(Dbg.ObjectParamsToString(values));
  }

  [Conditional("DEBUG")]
  [Conditional("SHOW_ALL_LOGS")]
  public static void LogIf(bool condition, string message)
  {
    if (!condition)
      return;
    Dbg.WriteToLog(message);
  }

  [Conditional("DEBUG")]
  [Conditional("SHOW_ALL_LOGS")]
  public static void LogIf(bool condition, params object[] values)
  {
    if (!condition)
      ;
  }

  [Conditional("DEBUG")]
  [Conditional("SHOW_ALL_LOGS")]
  public static void LogArray(object[] values, int printLimit = 10)
  {
    string str = "";
    for (int index = 0; index < values.Length && index < printLimit; ++index)
      str = str + (index > 0 ? ", " : "") + Dbg.GetString(values[index]);
  }

  [Conditional("DEBUG")]
  [Conditional("SHOW_ALL_LOGS")]
  public static void LogArray(short[] values, int printLimit = 10)
  {
    string str = "";
    for (int index = 0; index < values.Length && index < printLimit; ++index)
      str = str + (index > 0 ? ", " : "") + Dbg.GetString((object) values[index]);
  }

  [Conditional("DEBUG")]
  [Conditional("SHOW_ALL_LOGS")]
  public static void LogArray(int[] values, int printLimit = 10)
  {
    string str = "";
    for (int index = 0; index < values.Length && index < printLimit; ++index)
      str = str + (index > 0 ? ", " : "") + Dbg.GetString((object) values[index]);
  }

  [Conditional("DEBUG")]
  [Conditional("SHOW_ALL_LOGS")]
  public static void LogArray(float[] values, int printLimit = 10)
  {
    string str = "";
    for (int index = 0; index < values.Length && index < printLimit; ++index)
      str = str + (index > 0 ? ", " : "") + Dbg.GetString((object) values[index]);
  }

  [Conditional("DEBUG")]
  [Conditional("SHOW_ALL_LOGS")]
  public static void LogArray(double[] values, int printLimit = 10)
  {
    string str = "";
    for (int index = 0; index < values.Length && index < printLimit; ++index)
      str = str + (index > 0 ? ", " : "") + Dbg.GetString((object) values[index]);
  }

  public static void UnformattedMessage(string message)
  {
    Trace.WriteLine(message);
  }

  public static void Info(string message)
  {
    Dbg.WriteToLog((Dbg.AddTimestamp ? DateTime.UtcNow.ToString(Dbg.TimestampFormat + Dbg.TimestampSeparator) : "") + message);
  }

  public static void Info(params object[] values)
  {
    if (values.Length == 1)
      Dbg.WriteToLog((Dbg.AddTimestamp ? DateTime.UtcNow.ToString(Dbg.TimestampFormat + Dbg.TimestampSeparator) : "") + Dbg.GetString(values[0]));
    else
      Dbg.WriteToLog((Dbg.AddTimestamp ? DateTime.UtcNow.ToString(Dbg.TimestampFormat + Dbg.TimestampSeparator) : "") + Dbg.ObjectParamsToString(values));
  }

  public static void Warning(string message)
  {
    Dbg.WriteToLog((Dbg.AddTimestamp ? DateTime.UtcNow.ToString(Dbg.TimestampFormat + Dbg.TimestampSeparator) : "") + "[WARNING] " + message);
  }

  public static void Warning(params object[] values)
  {
    if (values.Length == 1)
      Dbg.WriteToLog((Dbg.AddTimestamp ? DateTime.UtcNow.ToString(Dbg.TimestampFormat + Dbg.TimestampSeparator) : "") + "[WARNING] " + Dbg.GetString(values[0]));
    else
      Dbg.WriteToLog((Dbg.AddTimestamp ? DateTime.UtcNow.ToString(Dbg.TimestampFormat + Dbg.TimestampSeparator) : "") + "[WARNING] " + Dbg.ObjectParamsToString(values));
  }

  public static void Error(string message)
  {
    Dbg.WriteToLog((Dbg.AddTimestamp ? DateTime.UtcNow.ToString(Dbg.TimestampFormat + Dbg.TimestampSeparator) : "") + "[ERROR] " + message);
  }

  public static void Error(params object[] values)
  {
    if (values.Length == 1)
      Dbg.WriteToLog((Dbg.AddTimestamp ? DateTime.UtcNow.ToString(Dbg.TimestampFormat + Dbg.TimestampSeparator) : "") + "[ERROR] " + Dbg.GetString(values[0]));
    else
      Dbg.WriteToLog((Dbg.AddTimestamp ? DateTime.UtcNow.ToString(Dbg.TimestampFormat + Dbg.TimestampSeparator) : "") + "[ERROR] " + Dbg.ObjectParamsToString(values));
  }

  public static void Exception(Exception ex)
  {
    string message = (Dbg.AddTimestamp ? DateTime.UtcNow.ToString(Dbg.TimestampFormat + Dbg.TimestampSeparator) : "") + "[ERROR] " + ex.Message + "\r\n" + ex.StackTrace;
    if (ex.InnerException != null)
      message = message + "\r\nInner Exception:" + ex.InnerException.Message + "\r\n" + ex.InnerException.StackTrace;
    Dbg.WriteToLog(message);
  }
}
