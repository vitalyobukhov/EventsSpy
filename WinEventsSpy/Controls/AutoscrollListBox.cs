using System.Windows.Forms;
using WinEventsSpy.PInvoke.Windows.Structures;
using WinEventsSpy.Wrappers;
using System;

namespace WinEventsSpy.Controls
{
    sealed class AutoscrollListBox : ListBox
    {
        protected override void WndProc(ref Message m)
        {
            switch ((Messages)m.Msg)
            {
                case Messages.WM_NCCALCSIZE:
                    while (WindowStyleManager.HasStyle(this.Handle, WindowStyle.WS_VSCROLL) &&
                        Items.Count > 0)
                    {
                        Items.RemoveAt(0);
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
