using System;
using System.Collections.Generic;
using WinEventsSpy.PInvoke.Cursor.Structures;
using WinEventsSpy.PInvoke.Resources.Structures;
using System.ComponentModel;
using System.Runtime.InteropServices;
using WinEventsSpy.PInvoke;

namespace WinEventsSpy.Wrappers
{
    sealed class SystemCursor : IDisposable
    {
        private Dictionary<SystemCursorId, IntPtr> originalCursors;

        private bool disposed;


        public SystemCursor()
        {
            originalCursors = new Dictionary<SystemCursorId, IntPtr>();
            foreach (object value in Enum.GetValues(typeof(SystemCursorId)))
            {
                originalCursors.Add((SystemCursorId)value, IntPtr.Zero);
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

            if (originalCursors[targetCursorId] == IntPtr.Zero)
            {
                var originalCursor = PInvoke.Cursor.Functions.LoadCursor(IntPtr.Zero, targetCursorId);
                if (originalCursor != IntPtr.Zero)
                {
                    var originalCursorCopy = PInvoke.Resources.Functions.CopyImage(originalCursor,
                        ImageType.IMAGE_CURSOR, 0, 0, CopyImageFlag.LR_NONE);
                    if (originalCursorCopy != IntPtr.Zero)
                    {
                        if (PInvoke.Cursor.Functions.SetSystemCursor(newCursor, targetCursorId))
                        {
                            originalCursors[targetCursorId] = originalCursorCopy;
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
            var sourceCursor = PInvoke.Cursor.Functions.LoadCursor(IntPtr.Zero, sourceCursorId);
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
            IntPtr cursorHandle = originalCursors[cursorId];
            if (cursorHandle != IntPtr.Zero)
            {
                var changed = PInvoke.Cursor.Functions.SetSystemCursor(cursorHandle, cursorId);
                PInvoke.Cursor.Functions.DestroyCursor(cursorHandle);
                originalCursors[cursorId] = IntPtr.Zero;

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

        public bool TryChange(IntPtr newCursor, SystemCursorId targetCursorId)
        {
            var result = false;

            try
            {
                Change(newCursor, targetCursorId);
            }
            catch (Win32Exception)
            { }

            return result;
        }

        public bool TryCopy(SystemCursorId sourceCursorId, SystemCursorId targetCursorId)
        {
            var result = false;

            try
            {
                Copy(sourceCursorId, targetCursorId);
            }
            catch (PInvokeException)
            { }

            return result;
        }

        public bool TryRestoreOne(SystemCursorId cursorId)
        {
            var result = false;

            IntPtr cursorHandle = originalCursors[cursorId];
            if (cursorHandle != IntPtr.Zero)
            {
                result = PInvoke.Cursor.Functions.SetSystemCursor(cursorHandle, cursorId);
                PInvoke.Cursor.Functions.DestroyCursor(cursorHandle);
                originalCursors[cursorId] = IntPtr.Zero;
            }

            return result;
        }

        public void RestoreAll()
        {
            foreach (object cursorId in Enum.GetValues(typeof(SystemCursorId)))
            {
                TryRestoreOne((SystemCursorId)cursorId);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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
                RestoreAll();

                disposed = true;
            }
        }
    }
}
