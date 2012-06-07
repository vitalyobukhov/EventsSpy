using System;

namespace WinEventsSpy.PInvoke.WinEvents.Structures
{
    [Flags]
    enum SetWinEventHookFlag : uint
    {
        WINEVENT_OUTOFCONTEXT   = 0x0,
        WINEVENT_SKIPOWNTHREAD  = 0x1,
        WINEVENT_SKIPOWNPROCESS = 0x2,
        WINEVENT_INCONTEXT      = 0x4
    }
}
