using System;
using WinEventsSpy.PInvoke.Windows.Structures;
using WinEventsSpy.PInvoke.Windows;
using WinEventsSpy.PInvoke;

namespace WinEventsSpy.Wrappers
{
    // wrapper for window style management
    sealed class WindowStyleManager
    {
        private IntPtr window;


        private static bool IsLong32
        {
            get
            {
                return IntPtr.Size == 4;
            }
        }


        public IntPtr Window
        {
            get
            {
                return window;
            }
            set
            {
                if (!Functions.IsWindow(value))
                {
                    throw new PInvokeException("Value is not window handle");
                }

                window = value;
            }
        }


        public WindowStyleManager()
        {
            window = IntPtr.Zero;
        }

        public WindowStyleManager(IntPtr window)
        {
            Window = window;
        }


        public WindowStyle GetStyle()
        {
            var result = GetWindowLong(GetWindowLongOffset.GWL_STYLE);
            if (result == 0)
            {
                throw new PInvokeException("Unable to get window style");
            }
            return (WindowStyle)result;
        }

        public ExtendedWindowStyle GetExtendedStyle()
        {
            var result = GetWindowLong(GetWindowLongOffset.GWL_EXSTYLE);
            if (result == 0)
            {
                throw new PInvokeException("Unable to get window extended style");
            }
            return (ExtendedWindowStyle)result;
        }

        public void SetStyle(WindowStyle style)
        {
            var result = SetWindowLong(GetWindowLongOffset.GWL_STYLE, (long)style);
            if (result == 0)
            {
                throw new PInvokeException("Unable to set window style");
            }
        }

        public void SetExtendedStyle(ExtendedWindowStyle style)
        {
            var result = SetWindowLong(GetWindowLongOffset.GWL_EXSTYLE, (long)style);
            if (result == 0)
            {
                throw new PInvokeException("Unable to set window extended style");
            }
        }

        public bool HasStyle(WindowStyle style)
        {
            return (GetStyle() & style) != 0;
        }

        public bool HasExtendedStyle(ExtendedWindowStyle style)
        {
            return (GetExtendedStyle() & style) != 0;
        }

        public void AddStyle(WindowStyle style)
        {
            var oldStyle = GetStyle();
            var newStyle = (oldStyle | style);
            SetStyle(newStyle);
        }

        public void AddExtendedStyle(IntPtr window, ExtendedWindowStyle style)
        {
            var oldStyle = GetExtendedStyle();
            var newStyle = (oldStyle | style);
            SetExtendedStyle(newStyle);
        }

        public void RemoveStyle(WindowStyle style)
        {
            var oldStyle = GetStyle();
            var newStyle = ((uint)oldStyle & (~((uint)style)));
            SetStyle((WindowStyle)newStyle);
        }

        public void RemoveExtendedStyle(ExtendedWindowStyle style)
        {
            var oldStyle = GetExtendedStyle();
            var newStyle = (oldStyle & (~style));
            SetExtendedStyle(newStyle);
        }


        private void CheckWindow()
        {
            if (!Functions.IsWindow(window))
            {
                throw new PInvokeException("Window does not exists");
            }
        }

        private long GetWindowLong(GetWindowLongOffset offset)
        {
            CheckWindow();

            return IsLong32 ?
                Functions.GetWindowLong32(window, offset) :
                Functions.GetWindowLong64(window, offset);
        }

        private long SetWindowLong(GetWindowLongOffset offset, long newValue)
        {
            CheckWindow();

            return IsLong32 ?
                Functions.SetWindowLong32(window, offset, (int)newValue) :
                Functions.SetWindowLong64(window, offset, newValue);
        }
    }
}
