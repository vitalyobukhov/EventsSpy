namespace WinEventsSpy.PInvoke.Windows.Structures
{
    enum WindowStyle: long
    {
        WS_OVERLAPPED = unchecked((int)0x00000000),
        WS_POPUP = unchecked((int)0x80000000),
        WS_CHILD = unchecked((int)0x40000000),
        WS_MINIMIZE = unchecked((int)0x20000000),
        WS_VISIBLE = unchecked((int)0x10000000),
        WS_DISABLED = unchecked((int)0x08000000),
        WS_CLIPSIBLINGS = unchecked((int)0x04000000),
        WS_CLIPCHILDREN = unchecked((int)0x02000000),
        WS_MAXIMIZE = unchecked((int)0x01000000),
        WS_BORDER = unchecked((int)0x00800000),
        WS_DLGFRAME = unchecked((int)0x00400000),
        WS_VSCROLL = unchecked((int)0x00200000),
        WS_HSCROLL = unchecked((int)0x00100000),
        WS_SYSMENU = unchecked((int)0x00080000),
        WS_THICKFRAME = unchecked((int)0x00040000),
        WS_GROUP = unchecked((int)0x00020000),
        WS_TABSTOP = unchecked((int)0x00010000),
        WS_MINIMIZEBOX = unchecked((int)0x00020000),
        WS_MAXIMIZEBOX = unchecked((int)0x00010000)
    }
}
