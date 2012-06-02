namespace WinEventsSpy.PInvoke.Processes.Structures
{
    enum GetExitCodeProcessExitCode : uint
    {
        STATUS_WAIT = 0x0,
        STATUS_ABANDONED_WAIT = 0x80,
        STATUS_USER_APC = 0xC0,
        STATUS_TIMEOUT = 0x102,
        STATUS_PENDING = 0x103
    }
}
