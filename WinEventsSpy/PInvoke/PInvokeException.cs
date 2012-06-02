using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace WinEventsSpy.PInvoke
{
    sealed class PInvokeException : Exception
    {
        public PInvokeException(string message)
            : base(message, new Win32Exception(Marshal.GetLastWin32Error()))
        { }

        public PInvokeException(string message, Win32Exception exception)
            : base(message, exception)
        { }
    }
}
