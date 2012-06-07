using System;
using System.Collections.Generic;
using WinEventsSpy.PInvoke.Cursor.Structures;
using WinEventsSpy.PInvoke.Resources.Structures;
using System.ComponentModel;
using System.Runtime.InteropServices;
using WinEventsSpy.PInvoke;
using Microsoft.Win32;
using System.Security.AccessControl;

namespace WinEventsSpy.Wrappers
{
    // wrapper to work with system cursor
    sealed class SystemCursor : IDisposable
    {
        private Dictionary<SystemCursorId, IntPtr> originalCursorPointers;
        private Dictionary<SystemCursorId, string> originalCursorPaths;

        private bool disposed;


        public SystemCursor()
        {
            originalCursorPointers = new Dictionary<SystemCursorId, IntPtr>();

            foreach (object value in Enum.GetValues(typeof(SystemCursorId)))
            {
                originalCursorPointers.Add((SystemCursorId)value, IntPtr.Zero);
            }

            disposed = false;
        }

        ~SystemCursor()
        {
            Dispose(false);
        }


        public void Change(IntPtr newCursor, SystemCursorId targetCursorId)
        {
            if (newCursor == IntPtr.Zero)
            {
                throw new ArgumentNullException("newCursor");
            }

            if (originalCursorPointers[targetCursorId] == IntPtr.Zero)
            {
                var originalCursor = PInvoke.Cursor.Functions.LoadSystemCursor(IntPtr.Zero, targetCursorId);
                if (originalCursor != IntPtr.Zero)
                {
                    var originalCursorCopy = PInvoke.Resources.Functions.CopyImage(originalCursor,
                        ImageType.IMAGE_CURSOR, 0, 0, CopyImageFlag.LR_NONE);
                    if (originalCursorCopy != IntPtr.Zero)
                    {
                        if (PInvoke.Cursor.Functions.SetSystemCursor(newCursor, targetCursorId))
                        {
                            originalCursorPointers[targetCursorId] = originalCursorCopy;
                        }
                        else
                        {
                            throw new PInvokeException("Unable to set system cursor");
                        }
                    }
                    else
                    {
                        throw new PInvokeException("Unable to copy original cursor");
                    }
                }
                else
                {
                    throw new PInvokeException("Unable to load original cursor");
                }
            }
            else
            {
                if (!PInvoke.Cursor.Functions.SetSystemCursor(newCursor, targetCursorId))
                {
                    throw new PInvokeException("Unable to set system cursor");
                }
            }
        }

        public void Copy(SystemCursorId sourceCursorId, SystemCursorId targetCursorId)
        {
            var sourceCursor = PInvoke.Cursor.Functions.LoadSystemCursor(IntPtr.Zero, sourceCursorId);
            if (sourceCursor != IntPtr.Zero)
            {
                var sourceCursorCopy = PInvoke.Resources.Functions.CopyImage(sourceCursor,
                    PInvoke.Resources.Structures.ImageType.IMAGE_CURSOR, 0, 0,
                    PInvoke.Resources.Structures.CopyImageFlag.LR_NONE);
                if (sourceCursorCopy != IntPtr.Zero)
                {
                    Change(sourceCursorCopy, targetCursorId);
                    PInvoke.Cursor.Functions.DestroyCursor(sourceCursorCopy);
                }
                else
                {
                    throw new PInvokeException("Unable to copy source cursor");
                }
            }
            else
            {
                throw new PInvokeException("Unable to load source cursor");
            }
        }

        public void RestoreOne(SystemCursorId cursorId)
        {
            IntPtr cursorHandle = originalCursorPointers[cursorId];
            if (cursorHandle != IntPtr.Zero)
            {
                var changed = PInvoke.Cursor.Functions.SetSystemCursor(cursorHandle, cursorId);
                PInvoke.Cursor.Functions.DestroyCursor(cursorHandle);
                originalCursorPointers[cursorId] = IntPtr.Zero;

                if (!changed)
                {
                    throw new PInvokeException("Unable to set system cursor");
                }
            }
            else
            {
                throw new InvalidOperationException("This cursor was not changed");
            }
        }

        private bool TryRestoreOne(SystemCursorId cursorId)
        {
            var result = false;

            IntPtr cursorHandle = originalCursorPointers[cursorId];
            if (cursorHandle != IntPtr.Zero)
            {
                result = PInvoke.Cursor.Functions.SetSystemCursor(cursorHandle, cursorId);
                PInvoke.Cursor.Functions.DestroyCursor(cursorHandle);
                originalCursorPointers[cursorId] = IntPtr.Zero;
            }

            return result;
        }

        public bool TryRestoreAll()
        {
            var result = true;

            foreach (var cursorId in Enum.GetValues(typeof(SystemCursorId)))
            {
                result &= TryRestoreOne((SystemCursorId)cursorId);
            }

            return result;
        }

        public void ReloadOne(SystemCursorId cursorId)
        {
            if (originalCursorPaths == null)
            {
                try
                {
                    InitReload();
                }
                catch (Exception ex)
                {
                    throw new PInvokeException("Unable to read cursor paths from system registry", ex);
                }
            }

            var cursorPath = originalCursorPaths[cursorId];
            if (cursorPath == null)
            {
                throw new PInvokeException("Unable to read cursor path from system registry", null);
            }

            var newCursor = PInvoke.Cursor.Functions.LoadCursorFromFile(cursorPath);
            if (newCursor == IntPtr.Zero)
            {
                throw new PInvokeException("Unable to load original cursor from file", null);
            }

            IntPtr originalCursor = originalCursorPointers[cursorId];
            if (originalCursor != IntPtr.Zero)
            {
                PInvoke.Cursor.Functions.DestroyCursor(originalCursor);
                originalCursorPointers[cursorId] = IntPtr.Zero;
            }

            if (!PInvoke.Cursor.Functions.SetSystemCursor(newCursor, cursorId))
            {
                throw new PInvokeException("Unable to set system cursor", null);
            }
        }

        public bool TryReloadAll()
        {
            var result = true;

            foreach (var cursorId in Enum.GetValues(typeof(SystemCursorId)))
            {
                try
                {
                    ReloadOne((SystemCursorId)cursorId);
                }
                catch
                {
                    result = false;
                }
            }

            return result;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        private void InitReload()
        {
            using (var cursorsKey = Registry.CurrentUser.
                OpenSubKey("Control Panel", RegistryKeyPermissionCheck.Default, RegistryRights.ReadKey).
                OpenSubKey("Cursors", RegistryKeyPermissionCheck.Default, RegistryRights.ReadKey))
            {
                originalCursorPaths = new Dictionary<SystemCursorId, string>();

                foreach (object value in Enum.GetValues(typeof(SystemCursorId)))
                {
                    var systemCursorId = (SystemCursorId)value;
                    originalCursorPaths.Add(systemCursorId,
                        (string)cursorsKey.GetValue(systemCursorId.ToString(), null));
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
                TryRestoreAll();

                disposed = true;
            }
        }
    }
}
