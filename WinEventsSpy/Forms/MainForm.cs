using System;
using System.Windows.Forms;
using WinEventsSpy.Wrappers;
using WinEventsSpy.PInvoke;
using WinEventsSpy.PInvoke.Cursor.Structures;
using WinEventsSpy.PInvoke.WinEvents.Structures;
using WinEventsSpy.PInvoke.Windows.Structures;

namespace WinEventsSpy.Forms
{
    partial class Main : Form
    {
        private enum FormState
        {
            Idle,
            MouseActive,
            TargetActive
        }


        private FormState state;

        private EventGrabber mouseGrabber;
        private EventGrabber targetGrabber;
        private ProcessMonitor targetMonitor;
        private SystemCursor systemCursor;

        private bool disposed;

        
        public Main()
        {
            InitializeComponent();

            state = FormState.Idle;

            mouseGrabber = new EventGrabber
            {
                MinEventType = SetWinEventHookEventType.EVENT_SYSTEM_CAPTURESTART,
                MaxEventType = SetWinEventHookEventType.EVENT_SYSTEM_CAPTURESTART
            };
            mouseGrabber.Started += new EventHandler(mouseGrabber_Started);
            mouseGrabber.Stopped += new EventHandler(mouseGrabber_Stopped);
            mouseGrabber.Grabbed += new EventHandler<EventGrabber.GrabbedEventArgs>(mouseGrabber_Grabbed);

            targetGrabber = new EventGrabber();
            targetGrabber.Stopped += new EventHandler(targetGrabber_Stopped);
            targetGrabber.Grabbed += new EventHandler<EventGrabber.GrabbedEventArgs>(targetGrabber_Grabbed);

            targetMonitor = new ProcessMonitor();
            targetMonitor.Stopped += new EventHandler(targetMonitor_Stopped);

            systemCursor = new SystemCursor();

            disposed = false;
        }

        ~Main()
        {
            Dispose(false);
        }


        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    if (components != null)
                    {
                        components.Dispose();
                    }
                }

                //dispose unmanaged resources
                mouseGrabber.Dispose();
                targetGrabber.Dispose();
                targetMonitor.Dispose();
                systemCursor.Dispose();

                disposed = true;

                base.Dispose(disposing);
            }
        }


        private void mouseGrabber_Started(object sender, EventArgs e)
        {
            state = FormState.MouseActive;

            try
            {
                systemCursor.Copy(SystemCursorId.CROSS, SystemCursorId.ARROW);
            }
            catch (PInvokeException)
            { }

            aslbxMessages.Items.Clear();
            btnToggleHook.Text = "Cancel";
        }

        private void mouseGrabber_Stopped(object sender, EventArgs e)
        {
            if (!targetGrabber.IsStarted)
            {
                state = FormState.Idle;

                systemCursor.RestoreAll();

                btnToggleHook.Text = "Hook";
            }
        }

        private void mouseGrabber_Grabbed(object sender, EventGrabber.GrabbedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<EventGrabber.GrabbedEventArgs>(mouseGrabber_Grabbed),
                    new object[] { sender, e });
            }
            else
            {
                targetGrabber.ThreadId = e.ThreadId;
                targetGrabber.ProcessId = e.ProcessId;
                targetGrabber.Start();

                mouseGrabber.Stop();

                targetMonitor.ProcessId = e.ProcessId;
                targetMonitor.Start();

                state = FormState.TargetActive;

                tbxWindowId.Text = e.Window.ToInt64().ToString();
                tbxProcessId.Text = e.ProcessId.ToString();
                tbxThreadId.Text = e.ThreadId.ToString();
                btnToggleHook.Text = "Unhook";

                systemCursor.RestoreAll();
            }
        }

        private void targetGrabber_Stopped(object sender, EventArgs e)
        {
            state = FormState.Idle;

            tbxWindowId.Clear();
            tbxProcessId.Clear();
            tbxThreadId.Clear();
            btnToggleHook.Text = "Hook";
        }

        private void targetGrabber_Grabbed(object sender, EventGrabber.GrabbedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<EventGrabber.GrabbedEventArgs>(targetGrabber_Grabbed),
                    new object[] { sender, e });
            }
            else
            {
                var itemDate = DateTime.Now.ToString(@"HH\:mm\:ss.fffffff");
                var itemEventType = e.EventType.ToString("F").PadRight(31);
                var itemObject = (e.StandardObject.HasValue ?
                    e.StandardObject.Value.ToString("F") :
                    e.ObjectId.ToString("X8")).
                    PadRight(23);

                var item = string.Format(@"{0}    {1}    {2}", itemDate, itemEventType, itemObject);
                aslbxMessages.Items.Add(item);
            }
        }

        private void targetMonitor_Stopped(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(targetMonitor_Stopped),
                    new object[] { sender, e });
            }
            else
            {
                try
                {
                    targetGrabber.Stop();
                }
                catch (PInvokeException ex)
                {
                    MessageBox.Show(this, ex.Message, "Error", 
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btnToggleHook_Click(object sender, EventArgs e)
        {
            switch (state)
            {
                case FormState.Idle:
                    try
                    {
                        mouseGrabber.Start();
                    }
                    catch (PInvokeException ex)
                    {
                        MessageBox.Show(this, ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

                case FormState.MouseActive:
                    try
                    {
                        mouseGrabber.Stop();
                    }
                    catch (PInvokeException ex)
                    {
                        MessageBox.Show(this, ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

                case FormState.TargetActive:
                    try
                    {
                        targetMonitor.Stop();
                    }
                    catch (PInvokeException ex)
                    {
                        MessageBox.Show(this, ex.Message, "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    break;

                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
