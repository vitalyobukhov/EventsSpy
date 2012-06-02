using System;
using System.Runtime.InteropServices;
using WinEventsSpy.PInvoke.Processes.Structures;

namespace WinEventsSpy.PInvoke.Processes
{
    static class Functions
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeProcess(IntPtr process, out GetExitCodeProcessExitCode exitCode);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(OpenProcessAccessFlag accessFlags, bool inheritHandle, uint processId);
    }
}
