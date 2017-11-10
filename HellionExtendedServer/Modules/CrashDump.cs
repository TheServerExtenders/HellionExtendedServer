using System;
using System.IO;
using System.Runtime.InteropServices;

namespace HellionExtendedServer
{
    public static class CrashDump
    {
        public static class MINIDUMP_TYPE
        {
            public const int MiniDumpNormal = 0x00000000;
            public const int MiniDumpWithDataSegs = 0x00000001;
            public const int MiniDumpWithFullMemory = 0x00000002;
            public const int MiniDumpWithHandleData = 0x00000004;
            public const int MiniDumpFilterMemory = 0x00000008;
            public const int MiniDumpScanMemory = 0x00000010;
            public const int MiniDumpWithUnloadedModules = 0x00000020;
            public const int MiniDumpWithIndirectlyReferencedMemory = 0x00000040;
            public const int MiniDumpFilterModulePaths = 0x00000080;
            public const int MiniDumpWithProcessThreadData = 0x00000100;
            public const int MiniDumpWithPrivateReadWriteMemory = 0x00000200;
            public const int MiniDumpWithoutOptionalData = 0x00000400;
            public const int MiniDumpWithFullMemoryInfo = 0x00000800;
            public const int MiniDumpWithThreadInfo = 0x00001000;
            public const int MiniDumpWithCodeSegs = 0x00002000;
        }

        [DllImport("dbghelp.dll")]
        public static extern bool MiniDumpWriteDump(IntPtr hProcess,
                                                    Int32 ProcessId,
                                                    IntPtr hFile,
                                                    int DumpType,
                                                    IntPtr ExceptionParam,
                                                    IntPtr UserStreamParam,
                                                    IntPtr CallackParam);

        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            CreateMiniDump(e);
        }

        [DllImport("kernel32.dll")]
        public static extern int GetCurrentThreadId();

        private static void CreateMiniDump(UnhandledExceptionEventArgs e)
        {
            Console.WriteLine("Creating Dump");
            //Back Slashes for windows and C# right???
            //Bug... Maybe

            String path = @"HES\Dump";
            if (!Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
            String now = DateTime.UtcNow.ToString("yyyy_MM_dd__HH_mm_ss");
            using (FileStream fs = new FileStream(String.Format("HES\\Dump\\UnhandledDump_{0}.dmp", now), FileMode.Create))
            {
                using (System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess())
                {
                    //TODO later remove Try Statement
                    try
                    {
                        MiniDumpWriteDump(process.Handle,
                            process.Id,
                            fs.SafeFileHandle.DangerousGetHandle(),
                            MINIDUMP_TYPE.MiniDumpWithFullMemory + MINIDUMP_TYPE.MiniDumpWithFullMemoryInfo + MINIDUMP_TYPE.MiniDumpWithThreadInfo + MINIDUMP_TYPE.MiniDumpWithCodeSegs + MINIDUMP_TYPE.MiniDumpWithFullMemory,
                            IntPtr.Zero,
                            IntPtr.Zero,
                            IntPtr.Zero);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }

            Console.WriteLine("Dump File Created at > " + String.Format("HES\\Dump\\UnhandledDump_{0}.dmp", now));
        }
    }
}