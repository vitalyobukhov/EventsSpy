using System;
using WinEventsSpy.PInvoke.Windows.Structures;
using WinEventsSpy.PInvoke.Windows;
using WinEventsSpy.PInvoke;

namespace WinEventsSpy.Wrappers
{
    static class WindowStyleManager
    {
        private static bool IsLong32
        {
            get
            {
                return IntPtr.Size == 4;
            }
        }


        public static WindowStyle GetStyle(IntPtr window)
        {
            var result = GetWindowLong(window, GetWindowLongOffset.GWL_STYLE);
            if (result == 0)
            {
                throw new PInvokeException("Unable to get window style");
            }
            return (WindowStyle)result;
        }

        public static ExtendedWindowStyle GetExtendedStyle(IntPtr window)
        {
            var result = GetWindowLong(window, GetWindowLongOffset.GWL_EXSTYLE);
            if (result == 0)
            {
                throw new PInvokeException("Unable to get window extended style");
            }
            return (ExtendedWindowStyle)result;
        }

        public static void SetStyle(IntPtr window, WindowStyle style)
        {
            var result = SetWindowLong(window, GetWindowLongOffset.GWL_STYLE, (long)style);
            if (result == 0)
            {
                throw new PInvokeException("Unable to set window style");
            }
        }

        public static void SetExtendedStyle(IntPtr window, ExtendedWindowStyle style)
        {
            var result = SetWindowLong(window, GetWindowLongOffset.GWL_EXSTYLE, (long)style);
            if (result == 0)
            {
                throw new PInvokeException("Unable to set window extended style");
            }
        }

        public static bool HasStyle(IntPtr window, WindowStyle style)
        {
            return (GetStyle(window) & style) != 0;
        }

        public static bool HasExtendedStyle(IntPtr window, ExtendedWindowStyle style)
        {
            return (GetExtendedStyle(window) & style) != 0;
        }

        public static void AddStyle(IntPtr window, WindowStyle style)
        {
            var oldStyle = GetStyle(window);
            var newStyle = (oldStyle | style);
            SetStyle(window, newStyle);
        }

        public static void AddExtendedStyle(IntPtr window, ExtendedWindowStyle style)
        {
            var oldStyle = GetExtendedStyle(window);
            var newStyle = (oldStyle | style);
            SetExtendedStyle(window, newStyle);
        }

        public static void RemoveStyle(IntPtr window, WindowStyle style)
        {
            var oldStyle = GetStyle(window);
            var newStyle = ((uint)oldStyle & (~((uint)style)));
            SetStyle(window, (WindowStyle)newStyle);
        }

        public static void RemoveExtendedStyle(IntPtr window, ExtendedWindowStyle style)
        {
            var oldStyle = GetExtendedStyle(window);
            var newStyle = (oldStyle & (~style));
            SetExtendedStyle(window, newStyle);
        }


        private static long GetWindowLong(IntPtr window, GetWindowLongOffset offset)
        {
            return IsLong32 ?
                Functions.GetWindowLong32(window, offset) :
                Functions.GetWindowLong64(window, offset);
        }

        private static long SetWindowLong(IntPtr window, GetWindowLongOffset offset, long newValue)
        {
            return IsLong32 ?
                Functions.SetWindowLong32(window, offset, (int)newValue) :
                Functions.SetWindowLong64(window, offset, newValue);
        }
    }
}
