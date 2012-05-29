using System;
using System.Runtime.InteropServices;

namespace WinEventsSpy
{
    static class WinApi
    {
        public delegate void WinEventDelegate(IntPtr hook, 
            uint type, IntPtr window, int objectId, 
            int childId, uint threadId, uint time);

        public enum WIN_EVENT_TYPE: uint
        {
            EVENT_MIN                       = 0x00000001,
            EVENT_MAX                       = 0x7FFFFFFF,
            EVENT_SYSTEM_SOUND              = 0x0001,
            EVENT_SYSTEM_ALERT              = 0x0002,
            EVENT_SYSTEM_FOREGROUND         = 0x0003,
            EVENT_SYSTEM_MENUSTART          = 0x0004,
            EVENT_SYSTEM_MENUEND            = 0x0005,
            EVENT_SYSTEM_MENUPOPUPSTART     = 0x0006,
            EVENT_SYSTEM_MENUPOPUPEND       = 0x0007,
            EVENT_SYSTEM_CAPTURESTART       = 0x0008,
            EVENT_SYSTEM_CAPTUREEND         = 0x0009,
            EVENT_SYSTEM_MOVESIZESTART      = 0x000A,
            EVENT_SYSTEM_MOVESIZEEND        = 0x000B,
            EVENT_SYSTEM_CONTEXTHELPSTART   = 0x000C,
            EVENT_SYSTEM_CONTEXTHELPEND     = 0x000D,
            EVENT_SYSTEM_DRAGDROPSTART      = 0x000E,
            EVENT_SYSTEM_DRAGDROPEND        = 0x000F,
            EVENT_SYSTEM_DIALOGSTART        = 0x0010,
            EVENT_SYSTEM_DIALOGEND          = 0x0011,
            EVENT_SYSTEM_SCROLLINGSTART     = 0x0012,
            EVENT_SYSTEM_SCROLLINGEND       = 0x0013,
            EVENT_SYSTEM_SWITCHSTART        = 0x0014,
            EVENT_SYSTEM_SWITCHEND          = 0x0015,
            EVENT_SYSTEM_MINIMIZESTART      = 0x0016,
            EVENT_SYSTEM_MINIMIZEEND        = 0x0017,
            EVENT_CONSOLE_CARET             = 0x4001,
            EVENT_CONSOLE_UPDATE_REGION     = 0x4002,
            EVENT_CONSOLE_UPDATE_SIMPLE     = 0x4003,
            EVENT_CONSOLE_UPDATE_SCROLL     = 0x4004,
            EVENT_CONSOLE_LAYOUT            = 0x4005,
            EVENT_CONSOLE_START_APPLICATION = 0x4006,
            EVENT_CONSOLE_END_APPLICATION   = 0x4007,
            CONSOLE_APPLICATION_16BIT       = 0x0001,
            CONSOLE_CARET_SELECTION         = 0x0001,
            CONSOLE_CARET_VISIBLE           = 0x0002,
            EVENT_OBJECT_CREATE             = 0x8000,
            EVENT_OBJECT_DESTROY            = 0x8001,
            EVENT_OBJECT_SHOW               = 0x8002,
            EVENT_OBJECT_HIDE               = 0x8003,
            EVENT_OBJECT_REORDER            = 0x8004,
            EVENT_OBJECT_FOCUS              = 0x8005,
            EVENT_OBJECT_SELECTION          = 0x8006,
            EVENT_OBJECT_SELECTIONADD       = 0x8007,
            EVENT_OBJECT_SELECTIONREMOVE    = 0x8008,
            EVENT_OBJECT_SELECTIONWITHIN    = 0x8009,
            EVENT_OBJECT_STATECHANGE        = 0x800A,
            EVENT_OBJECT_LOCATIONCHANGE     = 0x800B,
            EVENT_OBJECT_NAMECHANGE         = 0x800C,
            EVENT_OBJECT_DESCRIPTIONCHANGE  = 0x800D,
            EVENT_OBJECT_VALUECHANGE        = 0x800E,
            EVENT_OBJECT_PARENTCHANGE       = 0x800F,
            EVENT_OBJECT_HELPCHANGE         = 0x8010,
            EVENT_OBJECT_DEFACTIONCHANGE    = 0x8011,
            EVENT_OBJECT_ACCELERATORCHANGE  = 0x8012
        }

        public enum WIN_EVENT_OBJECT_ID : int
        {
	        OBJID_SELF              = 0x00000000,
	        OBJID_SYSMENU           = unchecked((int)0xFFFFFFFF),
	        OBJID_TITLEBAR          = unchecked((int)0xFFFFFFFE),
	        OBJID_MENU              = unchecked((int)0xFFFFFFFD),
	        OBJID_CLIENT            = unchecked((int)0xFFFFFFFC),
	        OBJID_VSCROLL           = unchecked((int)0xFFFFFFFB),
	        OBJID_HSCROLL           = unchecked((int)0xFFFFFFFA),
	        OBJID_SIZEGRIP          = unchecked((int)0xFFFFFFF9),
	        OBJID_CARET             = unchecked((int)0xFFFFFFF8),
	        OBJID_CURSOR            = unchecked((int)0xFFFFFFF7),
	        OBJID_ALERT             = unchecked((int)0xFFFFFFF6),
	        OBJID_SOUND             = unchecked((int)0xFFFFFFF5),
            OBJID_QUERYCLASSNAMEIDX = unchecked((int)0xFFFFFFF4),
	        OBJID_NATIVEOM          = unchecked((int)0xFFFFFFF0)
        }

        public enum WIN_EVENT_HOOK_FLAG: uint
        {
            WINEVENT_OUTOFCONTEXT   = 0x0000,
            WINEVENT_SKIPOWNTHREAD  = 0x0001,
            WINEVENT_SKIPOWNPROCESS = 0x0002,
            WINEVENT_INCONTEXT      = 0x0004
        }

        public enum PROCESS_EXIT_CODE : uint
        {
            STATUS_WAIT = 0x0,
            STATUS_ABANDONED_WAIT = 0x80,
            STATUS_USER_APC = 0xC0,
            STATUS_TIMEOUT = 0x102,
            STATUS_PENDING = 0x103
        }

        [Flags]
        public enum PROCESS_ACCESS_FLAG : uint
        {
            ALL               = 0x001F0FFF,
            TERMINATE         = 0x00000001,
            CREATE_THREAD     = 0x00000002,
            VM_OPERATIONS     = 0x00000008,
            VM_READ           = 0x00000010,
            VM_WRITE          = 0x00000020,
            DUP_HANDLE        = 0x00000040,
            SET_INFORMATION   = 0x00000200,
            QUERY_INFORMATION = 0x00000400,
            SYNCHRONIZE       = 0x00100000
        }

        //[StructLayout(LayoutKind.Sequential)]
        //public struct POINT
        //{
        //    public int X;
        //    public int Y;

        //    public POINT(int x, int y)
        //    {
        //        this.X = x;
        //        this.Y = y;
        //    }
        //}

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventTypeMin, 
            uint eventTypeMax, IntPtr library, WinEventDelegate handler, 
            uint processId, uint threadId, uint flags);

        [DllImport("user32.dll")]
        public static extern bool UnhookWinEvent(IntPtr window);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr window, out uint processId);

        //[DllImport("user32.dll")]
        //public static extern IntPtr WindowFromPoint(POINT point);

        //[DllImport("user32.dll")]
        //public static extern IntPtr RealChildWindowFromPoint(IntPtr parent, POINT point);

        //[DllImport("user32.dll")]
        //public static extern bool GetCursorPos(out POINT point);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetExitCodeProcess(IntPtr process, out uint exitCode);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint access, bool inheritHandle, uint processId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);
    }
}
