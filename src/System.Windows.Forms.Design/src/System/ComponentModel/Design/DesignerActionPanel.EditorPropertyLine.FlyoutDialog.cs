// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms;

namespace System.ComponentModel.Design;

internal sealed partial class DesignerActionPanel
{
    private sealed partial class EditorPropertyLine
    {
        internal class FlyoutDialog : Form
        {
            private readonly Control _hostedControl;
            private readonly Control _parentControl;

            public FlyoutDialog(Control hostedControl, Control parentControl, Color borderColor, Font font)
            {
                _hostedControl = hostedControl;
                _parentControl = parentControl;
                BackColor = SystemColors.Window;
                ControlBox = false;
                Font = font;
                FormBorderStyle = FormBorderStyle.None;
                MinimizeBox = false;
                MaximizeBox = false;
                ShowInTaskbar = false;
                StartPosition = FormStartPosition.Manual;
                Text = string.Empty;
                SuspendLayout();
                try
                {
                    Controls.Add(hostedControl);

                    int width = Math.Max(_hostedControl.Width, SystemInformation.MinimumWindowSize.Width);
                    int height = Math.Max(_hostedControl.Height, SystemInformation.MinimizedWindowSize.Height);
                    if (!borderColor.IsEmpty)
                    {
                        DockPadding.All = 1;
                        BackColor = borderColor;
                        width += 2;
                        height += 4;
                    }

                    _hostedControl.Dock = DockStyle.Fill;

                    Width = width;
                    Height = height;
                }
                finally
                {
                    ResumeLayout();
                }
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_TOOLWINDOW;
                    cp.Style |= unchecked((int)(WINDOW_STYLE.WS_POPUP | WINDOW_STYLE.WS_BORDER));
                    cp.ClassStyle |= (int)WNDCLASS_STYLES.CS_SAVEBITS;
                    if (_parentControl is not null)
                    {
                        if (!_parentControl.IsDisposed)
                        {
                            cp.Parent = _parentControl.Handle;
                        }
                    }

                    return cp;
                }
            }

            public virtual void FocusComponent()
            {
                if (_hostedControl is not null && Visible)
                {
                    _hostedControl.Focus();
                }
            }

            // Lifted directly from PropertyGridView.DropDownHolder. Less destructive than using ShowDialog().
            public unsafe void DoModalLoop()
            {
                while (Visible)
                {
                    Application.DoEvents();
                    PInvoke.MsgWaitForMultipleObjectsEx(
                        0,
                        null,
                        250,
                        QUEUE_STATUS_FLAGS.QS_ALLINPUT,
                        MSG_WAIT_FOR_MULTIPLE_OBJECTS_EX_FLAGS.MWMO_INPUTAVAILABLE);
                }
            }

            /// <summary>
            ///  General purpose method, based on Control.Contains()...
            ///  Determines whether a given window (specified using native window handle) is a descendant of this control.
            ///  This catches both contained descendants and 'owned' windows such as modal dialogs.
            ///  Using window handles rather than Control objects allows it to catch un-managed windows as well.
            /// </summary>
            private bool OwnsWindow(HWND hWnd)
            {
                while (!hWnd.IsNull)
                {
                    hWnd = (HWND)PInvokeCore.GetWindowLong(hWnd, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT);
                    if (hWnd.IsNull)
                    {
                        return false;
                    }

                    if (hWnd == (HWND)Handle)
                    {
                        return true;
                    }
                }

                return false;
            }

            protected override bool ProcessDialogKey(Keys keyData)
            {
                if (keyData is (Keys.Alt | Keys.Down) or (Keys.Alt | Keys.Up) or Keys.F4)
                {
                    // Any of these keys indicates the selection is accepted
                    Visible = false;
                    return true;
                }

                return base.ProcessDialogKey(keyData);
            }

            public void ShowDropDown(Control parent)
            {
                try
                {
                    PInvokeCore.SetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT, parent.Handle);

                    // Lifted directly from Form.ShowDialog().
                    HWND hWndCapture = PInvoke.GetCapture();
                    if (!hWndCapture.IsNull)
                    {
                        PInvokeCore.SendMessage(hWndCapture, PInvokeCore.WM_CANCELMODE);
                        PInvoke.ReleaseCapture();
                    }

                    Visible = true; // NOTE: Do this AFTER creating handle and setting parent
                    FocusComponent();
                    DoModalLoop();
                }
                finally
                {
                    PInvokeCore.SetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT, IntPtr.Zero);

                    // sometimes activation goes to LALA land - if our parent control is still around,
                    // remind it to take focus.
                    if (parent is not null && parent.Visible)
                    {
                        parent.Focus();
                    }
                }
            }

            protected override void WndProc(ref Message m)
            {
                if (m.MsgInternal == PInvokeCore.WM_ACTIVATE
                    && Visible
                    && m.WParamInternal.LOWORD == PInvoke.WA_INACTIVE
                    && !OwnsWindow((HWND)m.LParamInternal))
                {
                    Visible = false;
                    if (m.LParamInternal == 0)
                    {
                        // We 're switching process, also dismiss the parent.
                        Control? toplevel = _parentControl.TopLevelControl;
                        if (toplevel is ToolStripDropDown dropDown)
                        {
                            // If it's a toolstrip dropdown let it know that we have a specific close reason.
                            dropDown.Close();
                        }
                        else if (toplevel is not null)
                        {
                            toplevel.Visible = false;
                        }
                    }

                    return;
                }

                base.WndProc(ref m);
            }
        }
    }
}
