namespace WinEventsSpy.PInvoke.Processes.Structures
{
    enum GetExitCodeProcessExitCode : uint
    {
        STATUS_WAIT           = 0x000,
        STATUS_ABANDONED_WAIT = 0x080,
        STATUS_USER_APC       = 0x0C0,
        STATUS_TIMEOUT        = 0x102,
        STATUS_PENDING        = 0x103
    }
}
