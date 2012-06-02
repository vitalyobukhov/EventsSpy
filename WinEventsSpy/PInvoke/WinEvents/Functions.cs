using System;
using System.Runtime.InteropServices;
using WinEventsSpy.PInvoke.WinEvents.Structures;

namespace WinEventsSpy.PInvoke.WinEvents
{
    static class Functions
    {
        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(SetWinEventHookEventType eventTypeMin,
            SetWinEventHookEventType eventTypeMax, IntPtr library, SetWinEventHookDelegate handler,
            uint processId, uint threadId, SetWinEventHookFlag flags);

        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr hook);
    }
}
