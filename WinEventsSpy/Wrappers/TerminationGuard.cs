using System;
using System.Reflection;
using System.IO;
using System.Runtime.InteropServices;

namespace WinEventsSpy.Wrappers
{
    sealed class TerminationGuard : IDisposable
    {
        private string guardFilepath;
        private object guardLock;

        private bool disposed;


        public bool IsSet
        {
            get
            {
                return File.Exists(guardFilepath);
            }
        }


        public TerminationGuard()
        {
            guardLock = new object();

            disposed = false;
        }

        ~TerminationGuard()
        {
            Dispose(false);
        }


        public void Set()
        {
            lock (guardLock)
            {
                TryInit();

                if (File.Exists(guardFilepath))
                {
                    throw new InvalidOperationException("Instance was previously set");
                }
                else
                {
                    File.Create(guardFilepath);
                }
            }
        }

        public bool TrySet()
        {
            var result = false;

            lock (guardLock)
            {
                TryInit();

                if (!File.Exists(guardFilepath))
                {
                    File.Create(guardFilepath);
                    result = true;
                }
            }

            return result;
        }

        public void Release()
        {
            lock (guardLock)
            {
                TryInit();

                if (!File.Exists(guardFilepath))
                {
                    throw new InvalidOperationException("Instance was not previously set");
                }
                else
                {
                    File.Delete(guardFilepath);
                }
            }
        }

        public bool TryRelease()
        {
            var result = false;

            lock (guardLock)
            {
                TryInit();
                if (File.Exists(guardFilepath))
                {
                    File.Delete(guardFilepath);
                    result = true;
                }
            }

            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        private void TryInit()
        {
            if (guardFilepath == null)
            {
                var assemblyGuidAttribute = (GuidAttribute)Assembly.GetExecutingAssembly().
                    GetCustomAttributes(typeof(GuidAttribute), false).GetValue(0);
                var guardFilename = string.Format("{0}.tmp",
                    new Guid(assemblyGuidAttribute.Value).ToString("N"));
                guardFilepath = Path.Combine(Path.GetTempPath(), guardFilename);
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
                TryRelease();

                disposed = true;
            }
        }
    }
}
