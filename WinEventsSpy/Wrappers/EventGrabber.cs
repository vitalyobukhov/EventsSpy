using System;
using System.Runtime.InteropServices;
using WinEventsSpy.PInvoke;
using WinEventsSpy.PInvoke.WinEvents.Structures;

namespace WinEventsSpy.Wrappers
{
    sealed class EventGrabber : IDisposable
    {
        public sealed class GrabbedEventArgs : EventArgs
        {
            public SetWinEventHookEventType EventType { get; set; }
            public int ChildId { get; set; }
            public uint ThreadId { get; set; }
            public uint Time { get; set; }

            private int objectId;
            public int ObjectId
            {
                get
                {
                    return objectId;
                }
                set
                {
                    objectId = value;

                    if (Enum.IsDefined(typeof(SetWinEventHookStandardObjectId), objectId))
                    {
                        StandardObject = (SetWinEventHookStandardObjectId)objectId;
                    }
                    else
                    {
                        StandardObject = null;
                    }
                }
            }

            public SetWinEventHookStandardObjectId? StandardObject { get; private set; }

            private IntPtr window;
            public IntPtr Window
            {
                get
                {
                    return window;
                }
                set
                {
                    window = value;

                    uint processId;
                    PInvoke.Processes.Functions.GetWindowThreadProcessId(Window, out processId);
                    this.processId = processId;
                }
            }

            private uint processId;
            public uint ProcessId
            {
                get
                {
                    if (Window == IntPtr.Zero)
                    {
                        throw new InvalidOperationException("Window was not set");
                    }
                    else
                    {
                        if (processId == 0)
                        {
                            throw new PInvokeException("Unable to get process id from Window");
                        }

                        return processId;
                    }
                }
            }
        }


        private bool started;
        private object stateLock;

        private GCHandle handleHook;
        private IntPtr hook;

        private bool disposed;


        public SetWinEventHookEventType MinEventType { get; set; }
        public SetWinEventHookEventType MaxEventType { get; set; }
        public uint? ProcessId { get; set; }
        public uint? ThreadId { get; set; }

        public bool IsStarted
        {
            get
            {
                lock (stateLock)
                {
                    return started;
                }
            }
        }


        public event EventHandler Started;
        public event EventHandler Stopped;
        public event EventHandler<GrabbedEventArgs> Grabbed;


        public EventGrabber()
        {
            started = false;
            stateLock = new object();

            handleHook = GCHandle.Alloc(new SetWinEventHookDelegate(HandleHook));
            hook = IntPtr.Zero;

            disposed = false;

            MinEventType = SetWinEventHookEventType.EVENT_MIN;
            MaxEventType = SetWinEventHookEventType.EVENT_MAX;
        }

        ~EventGrabber()
        {
            Dispose(false);
        }


        public void Start()
        {
            lock (stateLock)
            {
                if (started)
                {
                    throw new InvalidOperationException("Already started");
                }
                
                if ((uint)MinEventType > (uint)MaxEventType)
                {
                    throw new InvalidOperationException("MinEventType is greater then MaxEventType");
                }

                hook = PInvoke.WinEvents.Functions.SetWinEventHook(MinEventType,
                    MaxEventType, IntPtr.Zero, (SetWinEventHookDelegate)handleHook.Target,
                    ProcessId ?? 0, ThreadId ?? 0, 
                    SetWinEventHookFlag.WINEVENT_OUTOFCONTEXT | SetWinEventHookFlag.WINEVENT_SKIPOWNPROCESS);

                if (hook == IntPtr.Zero)
                {
                    throw new PInvokeException("Unable to set event hook");
                }

                started = true;

                if (Started != null)
                {
                    Started(this, EventArgs.Empty);
                }
            }
        }

        public void Stop()
        {
            lock (stateLock)
            {
                if (!started)
                {
                    throw new InvalidOperationException("Already stopped");
                }

                if (!PInvoke.WinEvents.Functions.UnhookWinEvent(hook))
                {
                    throw new PInvokeException("Unable to remove event hook");
                }
                hook = IntPtr.Zero;

                started = false;

                if (Stopped != null)
                {
                    Stopped(this, EventArgs.Empty);
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        private void HandleHook(IntPtr hook, SetWinEventHookEventType eventType, 
            IntPtr window, int objectId, int childId, uint threadId, uint time)
        {
            lock (stateLock)
            {
                if (started)
                {
                    var eventArgs = new GrabbedEventArgs
                    {
                        EventType = eventType,
                        Window = window,
                        ObjectId = objectId,
                        ChildId = childId,
                        ThreadId = threadId,
                        Time = time
                    };

                    if (Grabbed != null)
                    {
                        Grabbed(this, eventArgs);
                    }
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                }

                //dispose unmanaged resources
                lock (stateLock)
                {
                    if (started)
                    {
                        PInvoke.WinEvents.Functions.UnhookWinEvent(hook);
                    }
                }
                handleHook.Free();

                disposed = true;
            }
        }
    }
}
