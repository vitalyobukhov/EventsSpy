using System.Windows.Forms;
using WinEventsSpy.Wrappers;
using System;

using WindowMessage = WinEventsSpy.PInvoke.Messages.Structures.Message;
using WinEventsSpy.PInvoke.Windows.Structures;

namespace WinEventsSpy.Controls
{
    // listbox with vertical autoscroll functionality
    // maximum simultaneous item count = maximum visible item count without vertical scroll
    sealed class AutoscrollListBox : ListBox
    {
        private WindowStyleManager styleManager;


        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            styleManager = new WindowStyleManager(Handle);
        }

        protected override void WndProc(ref Message m)
        {
            switch ((WindowMessage)m.Msg)
            {
                case WindowMessage.LB_ADDSTRING:
                case WindowMessage.CB_INSERTSTRING:
                case WindowMessage.LB_DELETESTRING:
                case WindowMessage.LB_RESETCONTENT:
                case WindowMessage.WM_SIZE:
                    base.WndProc(ref m);

                    if (styleManager != null)
                    {
                        while (styleManager.HasStyle(WindowStyle.WS_VSCROLL) &&
                            Items.Count > 0)
                        {
                            Items.RemoveAt(0);
                        }
                    }
                    break;

                case WindowMessage.WM_NCCALCSIZE:
                    if (styleManager != null)
                    {
                        if (styleManager.HasStyle(WindowStyle.WS_VSCROLL))
                        {
                            styleManager.RemoveStyle(WindowStyle.WS_VSCROLL);
                        }
                    }

                    base.WndProc(ref m);

                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
