using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using WinEventsSpy.PInvoke.Processes.Structures;
using WinEventsSpy.PInvoke;
using System.Threading;
using Microsoft.Win32.SafeHandles;

namespace WinEventsSpy.Wrappers
{
    // wrapper to monitor process availability
    sealed class ProcessMonitor : IDisposable
    {
        public sealed class ExitedEventArgs : EventArgs
        {
            public GetExitCodeProcessExitCode ExitCode { get; set; }
        }


        private bool started;
        private object stateLock;

        private IntPtr process;
        private ManualResetEvent processEvent;
        private ManualResetEvent stopEvent;

        private bool disposed;


        public uint ProcessId { get; set; }

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
        public event EventHandler<ExitedEventArgs> Exited;


        public ProcessMonitor()
        {
            started = false;
            stateLock = new object();

            disposed = false;

            ProcessId = 0;
        }

        ~ProcessMonitor()
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

                if (ProcessId == 0)
                {
                    throw new InvalidOperationException("ProccesId was not set");
                }

                process = PInvoke.Processes.Functions.OpenProcess(OpenProcessAccessFlag.QUERY_INFORMATION | OpenProcessAccessFlag.SYNCHRONIZE,
                    false, ProcessId);

                if (process == IntPtr.Zero)
                {
                    throw new PInvokeException("Unable to open process by id");
                }

                started = true;

                processEvent = new ManualResetEvent(false);
                processEvent.SafeWaitHandle = new SafeWaitHandle(process, true);
                stopEvent = new ManualResetEvent(false);

                new Thread(new ThreadStart(Monitor)).Start();

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

                started = false;

                stopEvent.Set();

                EndMonitor();

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


        private void EndMonitor()
        {
            PInvoke.Resources.Functions.CloseHandle(process);

            processEvent.Close();
            stopEvent.Close();
        }

        private void Monitor()
        {
            WaitHandle.WaitAny(new[] { processEvent, stopEvent });

            lock(stateLock)
            {
                if (started)
                {
                    GetExitCodeProcessExitCode exitCode;

                    if (!PInvoke.Processes.Functions.GetExitCodeProcess(process, out exitCode))
                    {
                        throw new PInvokeException("Unable to get process exit code");
                    }

                    if (exitCode != GetExitCodeProcessExitCode.STATUS_PENDING)
                    {
                        started = false;

                        EndMonitor();

                        var eventArgs = new ExitedEventArgs
                        {
                            ExitCode = exitCode
                        };

                        if (Exited != null)
                        {
                            Exited(this, eventArgs);
                        }
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
                        EndMonitor();
                    }
                }
                //timer.Dispose();

                disposed = true;
            }
        }
    }
}
