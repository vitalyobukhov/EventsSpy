using System;

namespace WinEventsSpy.PInvoke.WinEvents.Structures
{
    delegate void SetWinEventHookDelegate(IntPtr hook, SetWinEventHookEventType eventType, 
        IntPtr window, int objectId, int childId, uint threadId, uint time);
}
