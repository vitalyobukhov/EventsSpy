using System;
using System.Windows.Forms;
using WinEventsSpy.Wrappers;
using WinEventsSpy.PInvoke;
using WinEventsSpy.PInvoke.Cursor.Structures;
using WinEventsSpy.PInvoke.WinEvents.Structures;
using WinEventsSpy.PInvoke.Windows.Structures;
using System.Reflection;
using System.Drawing;

namespace WinEventsSpy.Forms
{
    partial class Main : Form
    {
        private enum FormState
        {
            // no hook and activity
            Idle,
            // waiting for mouse click on target window
            MouseActive,
            // target window was captured
            TargetActive
        }


        private FormState state;

        // monitors mouse input to specify target window
        private EventGrabber mouseGrabber;
        // monitors target window events
        private EventGrabber targetGrabber;
        // monitors target process availability
        private ProcessMonitor targetMonitor;
        // changes global cursor
        private SystemCursor systemCursor;
        // to prevent issues with cursor
        private TerminationGuard terminationGuard;

        
        public Main()
        {
            InitializeComponent();
            this.Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

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
            targetMonitor.Stopped += new EventHandler(targetMonitor_Ended);
            targetMonitor.Exited += new EventHandler<ProcessMonitor.ExitedEventArgs>(targetMonitor_Ended);

            systemCursor = new SystemCursor();

            terminationGuard = new TerminationGuard();
            try
            {
                if (!terminationGuard.TrySet())
                {
                    systemCursor.TryReloadAll();
                }
            }
            catch
            { }
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                mouseGrabber.Dispose();
                targetGrabber.Dispose();
                targetMonitor.Dispose();
                systemCursor.Dispose();
                terminationGuard.Dispose();
            }

            base.Dispose(disposing);
        }


        // mouse monitoring was started
        private void mouseGrabber_Started(object sender, EventArgs e)
        {
            state = FormState.MouseActive;

            try
            {
                // change global cursor
                systemCursor.Copy(SystemCursorId.CROSSHAIR, SystemCursorId.ARROW);
            }
            catch (PInvokeException)
            { }

            aslbxMessages.Items.Clear();
            btnToggleHook.Text = "Cancel";
        }

        // mouse monitoring stopped
        // canceled by user or target window was specified
        private void mouseGrabber_Stopped(object sender, EventArgs e)
        {
            // if canceled by user
            if (!targetGrabber.IsStarted)
            {
                state = FormState.Idle;

                // restore global cursor
                systemCursor.TryRestoreAll();

                btnToggleHook.Text = "Hook";
            }
        }

        // mouse click on target window
        private void mouseGrabber_Grabbed(object sender, EventGrabber.GrabbedEventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler<EventGrabber.GrabbedEventArgs>(mouseGrabber_Grabbed),
                    new object[] { sender, e });
            }
            else
            {
                // set up target events monitoring
                targetGrabber.ThreadId = e.ThreadId;
                targetGrabber.ProcessId = e.ProcessId;
                targetGrabber.Start();

                // stop mouse monitoring
                mouseGrabber.Stop();

                // set up target availability monitoring
                targetMonitor.ProcessId = e.ProcessId;
                targetMonitor.Start();

                state = FormState.TargetActive;

                tbxWindowId.Text = e.Window.ToInt64().ToString();
                tbxProcessId.Text = e.ProcessId.ToString();
                tbxThreadId.Text = e.ThreadId.ToString();
                btnToggleHook.Text = "Unhook";

                // restore global cursor
                systemCursor.TryRestoreAll();
            }
        }

        // target event monitoring was stopped
        private void targetGrabber_Stopped(object sender, EventArgs e)
        {
            state = FormState.Idle;

            tbxWindowId.Clear();
            tbxProcessId.Clear();
            tbxThreadId.Clear();
            btnToggleHook.Text = "Hook";
        }

        // target raised event
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

        // target availability monitoring was stopped
        private void targetMonitor_Ended(object sender, EventArgs e)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EventHandler(targetMonitor_Ended),
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
                //initiate mouse monitoring
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

                //cancel by user
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

                //cancel target monitoring
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
