using System;
using System.Runtime.InteropServices;
using WinEventsSpy.PInvoke.Cursor.Structures;

namespace WinEventsSpy.PInvoke.Cursor
{
    static class Functions
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr LoadCursor(IntPtr module, SystemCursorId systemCursorId);

        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool SetSystemCursor(IntPtr cursor, SystemCursorId systemCursorId);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool DestroyCursor(IntPtr cursor);
    }
}
