using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using WinEventsSpy.PInvoke.Processes.Structures;
using WinEventsSpy.PInvoke;

namespace WinEventsSpy.Wrappers
{
    sealed class ProcessMonitor : IDisposable
    {
        public sealed class ExitedEventArgs : EventArgs
        {
            public GetExitCodeProcessExitCode ExitCode { get; set; }
        }


        private bool started;
        private object stateLock;

        private Timer timer;
        private IntPtr process;

        private bool disposed;


        public uint ProcessId { get; set; }
        public uint PollInterval { get; set; }

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

            timer = new Timer();
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            process = IntPtr.Zero;

            disposed = false;

            ProcessId = 0;
            PollInterval = 1000;
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

                if (PollInterval == 0)
                {
                    throw new InvalidOperationException("PollInterval was not set");
                }

                process = PInvoke.Processes.Functions.OpenProcess(OpenProcessAccessFlag.QUERY_INFORMATION,
                    false, ProcessId);

                if (process == IntPtr.Zero)
                {
                    throw new PInvokeException("Unable to open process by id");
                }

                started = true;

                timer.Start();

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

                PInvoke.Resources.Functions.CloseHandle(process);
                process = IntPtr.Zero;

                started = false;

                timer.Stop();

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


        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            lock (stateLock)
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
                        var eventArgs = new ExitedEventArgs
                        {
                            ExitCode = exitCode
                        };

                        if (Exited != null)
                        {
                            Exited(this, eventArgs);
                        }

                        Stop();
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
                        PInvoke.Resources.Functions.CloseHandle(process);
                    }
                }
                timer.Dispose();

                disposed = true;
            }
        }
    }
}
