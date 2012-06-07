using System;

namespace WinEventsSpy.PInvoke.Resources.Structures
{
    [Flags]
    enum CopyImageFlag : uint
    {
        LR_NONE             = 0x0000,
        LR_COPYDELETEORG    = 0x0008,
        LR_COPYFROMRESOURCE = 0x4000,
        LR_COPYRETURNORG    = 0x0004,
        LR_CREATEDIBSECTION = 0x2000,
        LR_DEFAULTSIZE      = 0x0040,
        LR_MONOCHROME       = 0x0001
    }
}
