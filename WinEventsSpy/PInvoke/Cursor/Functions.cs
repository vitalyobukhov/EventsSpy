using System;
using System.Runtime.InteropServices;
using WinEventsSpy.PInvoke.Cursor.Structures;

namespace WinEventsSpy.PInvoke.Cursor
{
    static class Functions
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr LoadCursor(IntPtr module, IntPtr cursor);

        [DllImport("user32.dll", EntryPoint = "LoadCursor", SetLastError = true)]
        public static extern IntPtr LoadSystemCursor(IntPtr module, SystemCursorId systemCursorId);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr LoadCursorFromFile(string filename);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetSystemCursor(IntPtr cursor, SystemCursorId systemCursorId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyCursor(IntPtr cursor);
    }
}
