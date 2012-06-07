using System;

namespace WinEventsSpy.PInvoke.Processes.Structures
{
    [Flags]
    enum OpenProcessAccessFlag : uint
    {
        ALL               = 0x1F0FFF,
        TERMINATE         = 0x000001,
        CREATE_THREAD     = 0x000002,
        VM_OPERATIONS     = 0x000008,
        VM_READ           = 0x000010,
        VM_WRITE          = 0x000020,
        DUP_HANDLE        = 0x000040,
        SET_INFORMATION   = 0x000200,
        QUERY_INFORMATION = 0x000400,
        SYNCHRONIZE       = 0x100000
    }
}
