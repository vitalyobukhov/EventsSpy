using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WinEventsSpy
{
    partial class MainForm : Form
    {
        private enum FormState
        {
            Idle,
            Capturing,
            Active
        }


        private FormState state;

        private object targetHookLock;
        private GCHandle handleTargetHookHandle;
        private IntPtr targetHookHandle;
        private IntPtr targetProcessHandle;
        private Timer targetTimer;

        private GCHandle handleMouseHookHandle;
        private IntPtr mouseHookHandle;



        public MainForm()
        {
            InitializeComponent();

            state = FormState.Idle;

            targetHookLock = new object();
            targetHookHandle = IntPtr.Zero;
            targetProcessHandle = IntPtr.Zero;
            targetTimer = new Timer();

            mouseHookHandle = IntPtr.Zero;

            targetTimer.Tick += new EventHandler(targetTimer_Tick);
            Application.ThreadExit += new EventHandler(Application_ThreadExit);
            Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
        }


        private void StartCapturing()
        {
            handleMouseHookHandle = GCHandle.Alloc(new WinApi.WinEventDelegate(HandleMouseHook));

            mouseHookHandle = WinApi.SetWinEventHook((uint)WinApi.WIN_EVENT_TYPE.EVENT_SYSTEM_CAPTURESTART,
                (uint)WinApi.WIN_EVENT_TYPE.EVENT_SYSTEM_CAPTURESTART,
                IntPtr.Zero,
                (WinApi.WinEventDelegate)handleMouseHookHandle.Target,
                (uint)0,
                (uint)0,
                (uint)(WinApi.WIN_EVENT_HOOK_FLAG.WINEVENT_OUTOFCONTEXT | WinApi.WIN_EVENT_HOOK_FLAG.WINEVENT_SKIPOWNTHREAD));

            if (mouseHookHandle != IntPtr.Zero)
            {
                state = FormState.Capturing;

                lbxMessages.Items.Clear();
                btnToggleHook.Text = "Cancel";
            }
            else
            {
                MessageBox.Show(this, "Unable to set mouse hook", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CancelCapturing()
        {
            WinApi.UnhookWinEvent(mouseHookHandle);
            handleMouseHookHandle.Free();

            state = FormState.Idle;

            btnToggleHook.Text = "Hook";
        }

        private void StopHook()
        {
            WinApi.UnhookWinEvent(targetHookHandle);
            WinApi.CloseHandle(targetProcessHandle);
            handleTargetHookHandle.Free();

            state = FormState.Idle;

            tbxWindowId.Clear();
            tbxProcessId.Clear();
            tbxThreadId.Clear();
            btnToggleHook.Text = "Hook";

            targetTimer.Stop();
        }

        private void HandleMouseHook(IntPtr hook,
            uint type, IntPtr window, int objectId,
            int childId, uint threadId, uint time)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new WinApi.WinEventDelegate(HandleMouseHook),
                    new object[] { hook, type, window, objectId, childId, threadId, time });
            }
            else
            {
                lock (targetHookLock)
                {
                    string errorMessage = string.Empty;

                    //WinApi.POINT point;
                    //if (!WinApi.GetCursorPos(out point))
                    //{
                    //    errorMessage = @"Unable to get cursor position";
                    //    goto HandleMouseHookError;
                    //}

                    //IntPtr targetWindow = WinApi.WindowFromPoint(point);
                    //if (targetWindow == IntPtr.Zero)
                    //{
                    //    errorMessage = @"Unable to get target window";
                    //    goto HandleMouseHookError;
                    //}

                    //IntPtr childWindow = WinApi.RealChildWindowFromPoint(targetWindow, point);
                    //if (childWindow != IntPtr.Zero)
                    //{
                    //    targetWindow = childWindow;
                    //}

                    uint targetThreadId, targetProcessId;
                    targetThreadId = WinApi.GetWindowThreadProcessId(window, out targetProcessId);
                    if (targetThreadId == 0 || targetProcessId == 0)
                    {
                        errorMessage = @"Unable to get target process id";
                        goto HandleMouseHookError;
                    }

                    targetProcessHandle = WinApi.OpenProcess((uint)WinApi.PROCESS_ACCESS_FLAG.QUERY_INFORMATION,
                                false,
                                targetProcessId);
                    if (targetProcessHandle == IntPtr.Zero)
                    {
                        errorMessage = @"Unable to get target process handle";
                        goto HandleMouseHookError;
                    }

                    handleTargetHookHandle = GCHandle.Alloc(new WinApi.WinEventDelegate(HandleTargetHook));
                    targetHookHandle = WinApi.SetWinEventHook((uint)WinApi.WIN_EVENT_TYPE.EVENT_MIN,
                        (uint)WinApi.WIN_EVENT_TYPE.EVENT_MAX,
                        IntPtr.Zero,
                        (WinApi.WinEventDelegate)handleTargetHookHandle.Target,
                        targetProcessId,
                        threadId,
                        (uint)(WinApi.WIN_EVENT_HOOK_FLAG.WINEVENT_OUTOFCONTEXT));
                    if (targetHookHandle == IntPtr.Zero)
                    {
                        errorMessage = @"Unable to hook target process";
                        WinApi.CloseHandle(targetProcessHandle);
                        goto HandleMouseHookError;
                    }

                    WinApi.UnhookWinEvent(mouseHookHandle);
                    handleMouseHookHandle.Free();

                    state = FormState.Active;

                    tbxWindowId.Text = window.ToInt64().ToString();
                    tbxProcessId.Text = targetProcessId.ToString();
                    tbxThreadId.Text = threadId.ToString();
                    btnToggleHook.Text = "Unhook";

                    targetTimer.Start();

                    return;

                HandleMouseHookError:
                    MessageBox.Show(this, errorMessage, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void HandleTargetHook(IntPtr hook,
            uint type, IntPtr window, int objectId,
            int childId, uint threadId, uint time)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new WinApi.WinEventDelegate(HandleTargetHook),
                    new object[] { hook, type, window, objectId, childId, threadId, time });
            }
            else
            {
                if (Enum.IsDefined(typeof(WinApi.WIN_EVENT_TYPE), type))
                {
                    var item = string.Format(@"{0}    {1}    {2}",
                        DateTime.Now.ToString(@"HH\:mm\:ss.fffffff"),
                        Enum.IsDefined(typeof(WinApi.WIN_EVENT_TYPE), type) ?
                            ((WinApi.WIN_EVENT_TYPE)type).ToString("F").PadRight(31) :
                            type.ToString("X8").PadRight(31),
                        Enum.IsDefined(typeof(WinApi.WIN_EVENT_OBJECT_ID), objectId) ? 
                            ((WinApi.WIN_EVENT_OBJECT_ID)objectId).ToString("F").PadRight(23) : 
                            objectId.ToString("X8").PadRight(23));
                    lbxMessages.Items.Add(item);
                }

                const int messagesMaxCount = 16;
                if (lbxMessages.Items.Count > messagesMaxCount)
                {
                    lbxMessages.Items.RemoveAt(0);
                }
            }
        }

        private void CloseHandles()
        {
            WinApi.UnhookWinEvent(targetHookHandle);
            WinApi.CloseHandle(targetProcessHandle);

            if (handleTargetHookHandle.IsAllocated)
            {
                handleTargetHookHandle.Free();
            }

            WinApi.UnhookWinEvent(mouseHookHandle);
        }


        private void btnToggleHook_Click(object sender, EventArgs e)
        {
            lock (targetHookLock)
            {
                switch (state)
                {
                    case FormState.Idle:
                        StartCapturing();
                        break;

                    case FormState.Capturing:
                        CancelCapturing();
                        break;

                    case FormState.Active:
                        StopHook();
                        break;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        void targetTimer_Tick(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(targetTimer_Tick),
                    new object[] { sender, e });
            }
            else
            {
                uint exitCode;
                if (!WinApi.GetExitCodeProcess(targetProcessHandle, out exitCode) ||
                    (WinApi.PROCESS_EXIT_CODE)exitCode != WinApi.PROCESS_EXIT_CODE.STATUS_PENDING)
                {
                    lock (targetHookLock)
                    {
                        StopHook();
                    }
                }
            }
        }

        private void Application_ThreadExit(object sender, EventArgs e)
        {
            CloseHandles();
        }

        private void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            CloseHandles();
        }
    }
}
