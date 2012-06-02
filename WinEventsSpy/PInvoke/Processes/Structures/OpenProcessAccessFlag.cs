using System;

namespace WinEventsSpy.PInvoke.Processes.Structures
{
    [Flags]
    enum OpenProcessAccessFlag : uint
    {
        ALL = 0x001F0FFF,
        TERMINATE = 0x00000001,
        CREATE_THREAD = 0x00000002,
        VM_OPERATIONS = 0x00000008,
        VM_READ = 0x00000010,
        VM_WRITE = 0x00000020,
        DUP_HANDLE = 0x00000040,
        SET_INFORMATION = 0x00000200,
        QUERY_INFORMATION = 0x00000400,
        SYNCHRONIZE = 0x00100000
    }
}
