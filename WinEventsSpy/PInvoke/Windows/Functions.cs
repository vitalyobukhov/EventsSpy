using System;
using System.Runtime.InteropServices;
using WinEventsSpy.PInvoke.Windows.Structures;

namespace WinEventsSpy.PInvoke.Windows
{
    static class Functions
    {
        [DllImport("user32.dll", EntryPoint = "GetWindowLong", SetLastError = true)]
        public static extern int GetWindowLong32(IntPtr window, GetWindowLongOffset offset);

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr", SetLastError = true)]
        public static extern long GetWindowLong64(IntPtr window, GetWindowLongOffset offset);

        [DllImport("user32.dll", EntryPoint = "SetWindowLong", SetLastError = true)]
        public static extern int SetWindowLong32(IntPtr window, GetWindowLongOffset offset, int newValue);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", SetLastError = true)]
        public static extern long SetWindowLong64(IntPtr window, GetWindowLongOffset offset, long newValue);

        [DllImport("user32.dll")]
        public static extern bool IsWindow(IntPtr handle);
    }
}
