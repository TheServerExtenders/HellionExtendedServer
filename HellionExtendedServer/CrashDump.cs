using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HellionExtendedServer.Common;

namespace HellionExtendedServer.DBGHelp_Sample
{
    static class Debuging
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
            CreateMiniDump();
        }

        private static void CreateMiniDump()
        {
            Console.WriteLine("Creating Dump"+ Directory.GetCurrentDirectory());
            //Back Slashes for windows and C# right???
            //Bug... Maybe

            Log.Instance.Error("asdasdasds");
            String path = @"HES\Dump";
            if (!Directory.Exists(path)) System.IO.Directory.CreateDirectory(path);
            Log.Instance.Error("asdasdasds");
            String now = DateTime.UtcNow.ToString("yyyy_MM_dd__HH_mm_ss");
            Log.Instance.Error("asdasdasds>>"+now);
            using (FileStream fs = new FileStream(String.Format( "HES\\Dump\\UnhandledDump_{0}.dmp",now), FileMode.Create))
            {
                Log.Instance.Error("asdasdasds");
                using (System.Diagnostics.Process process = System.Diagnostics.Process.GetCurrentProcess())
                {
                    try
                    {

                        Log.Instance.Error("asdasdasds");
                        MiniDumpWriteDump(process.Handle,
                            process.Id,
                            fs.SafeFileHandle.DangerousGetHandle(),
                            MINIDUMP_TYPE.MiniDumpWithFullMemory+MINIDUMP_TYPE.MiniDumpWithFullMemoryInfo,
                            //MINIDUMP_TYPE.MiniDumpWithFullMemory,
                            IntPtr.Zero,
                            IntPtr.Zero,
                            IntPtr.Zero);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                Log.Instance.Error("asdsssssssssssssssssssssssssssssssssasdasds");
                }
            }

            Console.WriteLine("Finished (my) Dump XD");
        }
    }
}
