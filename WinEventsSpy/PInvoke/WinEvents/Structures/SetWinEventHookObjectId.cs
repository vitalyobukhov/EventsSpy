namespace WinEventsSpy.PInvoke.WinEvents.Structures
{
    public enum SetWinEventHookStandardObjectId : int
    {
        OBJID_SELF = 0x00000000,
        OBJID_SYSMENU = unchecked((int)0xFFFFFFFF),
        OBJID_TITLEBAR = unchecked((int)0xFFFFFFFE),
        OBJID_MENU = unchecked((int)0xFFFFFFFD),
        OBJID_CLIENT = unchecked((int)0xFFFFFFFC),
        OBJID_VSCROLL = unchecked((int)0xFFFFFFFB),
        OBJID_HSCROLL = unchecked((int)0xFFFFFFFA),
        OBJID_SIZEGRIP = unchecked((int)0xFFFFFFF9),
        OBJID_CARET = unchecked((int)0xFFFFFFF8),
        OBJID_CURSOR = unchecked((int)0xFFFFFFF7),
        OBJID_ALERT = unchecked((int)0xFFFFFFF6),
        OBJID_SOUND = unchecked((int)0xFFFFFFF5),
        OBJID_QUERYCLASSNAMEIDX = unchecked((int)0xFFFFFFF4),
        OBJID_NATIVEOM = unchecked((int)0xFFFFFFF0)
    }
}
