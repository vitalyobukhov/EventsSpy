using System;
using System.Runtime.InteropServices;
using WinEventsSpy.PInvoke.Resources.Structures;

namespace WinEventsSpy.PInvoke.Resources
{
    static class Functions
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CopyImage(IntPtr image, ImageType imageType, int width, int height, CopyImageFlag flags);

        //[DllImport("kernel32.dll", SetLastError = true)]
        //public static extern bool CloseHandle(IntPtr handle);
    }
}
