// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.Design;

internal class DropDownHolder : Form
{
    private Control _parent;
    private Control _currentControl = null;
    private const int BORDER = 1;

    [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")]
    public DropDownHolder(Control parent)
    : base()
    {
        parent = parent;
        ShowInTaskbar = false;
        ControlBox = false;
        MinimizeBox = false;
        MaximizeBox = false;
        Text = "";
        FormBorderStyle = FormBorderStyle.FixedToolWindow;
        StartPosition = FormStartPosition.Manual;
        Font = parent.Font;
        Visible = false;
        BackColor = SystemColors.Window;
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ExStyle |= NativeMethods.WS_EX_TOOLWINDOW;
            cp.Style |= NativeMethods.WS_POPUP | NativeMethods.WS_BORDER;
            if (_parent is not null)
            {
                cp.Parent = _parent.Handle;
            }

            return cp;
        }
    }

    // Lifted directly from PropertyGridView.DropDownHolder. Less destructive than using ShowDialog().
    //  
    public void DoModalLoop()
    {

        while (Visible)
        {
            Application.DoEvents();
            UnsafeNativeMethods.MsgWaitForMultipleObjectsEx(0, IntPtr.Zero, 250, NativeMethods.QS_ALLINPUT, NativeMethods.MWMO_INPUTAVAILABLE);
        }
    }

    public virtual Control Component
    {
        get
        {
            return _currentControl;
        }
    }

    public virtual bool GetUsed()
    {
        return (_currentControl is not null);
    }

    protected override void OnMouseDown(MouseEventArgs me)
    {
        if (me.Button == MouseButtons.Left)
        {
            Visible = false;
        }

        base.OnMouseDown(me);
    }

    /// <devdoc>
    ///    General purpose method, based on Control.Contains()...
    ///
    ///    Determines whether a given window (specified using native window handle)
    ///    is a descendant of this control. This catches both contained descendants
    ///    and 'owned' windows such as modal dialogs. Using window handles rather
    ///    than Control objects allows it to catch un-managed windows as well.
    /// </devdoc>
    private bool OwnsWindow(IntPtr hWnd)
    {
        while (hWnd != IntPtr.Zero)
        {
            hWnd = UnsafeNativeMethods.GetWindowLong(new HandleRef(null, hWnd), NativeMethods.GWL_HWNDPARENT);
            if (hWnd == IntPtr.Zero)
            {
                return false;
            }

            if (hWnd == Handle)
            {
                return true;
            }
        }

        return false;
    }

    /*
    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
        if (currentControl is not null) {
            currentControl.SetBounds(0, 0, width - 2 * BORDER, height - 2 * BORDER);
            width = currentControl.Width;
            height = currentControl.Height;
            if (height == 0 && currentControl is ListBox) {
                height = ((ListBox)currentControl).ItemHeight;
                currentControl.Height = height;
            }
            width += 2 * BORDER;
            height += 2 * BORDER;
        }
        base.SetBoundsCore(x, y, width, height, specified);
    } */

    public virtual void FocusComponent()
    {
        if (_currentControl is not null && Visible)
        {
            //currentControl.FocusInternal();
            _currentControl.Focus();
        }
    }

    private void OnCurrentControlResize(object o, EventArgs e)
    {
        if (_currentControl is not null)
        {
            int oldWidth = Width;
            UpdateSize();
            _currentControl.Location = new Point(BORDER, BORDER);
            Left -= (Width - oldWidth);
        }
    }

    protected override bool ProcessDialogKey(Keys keyData)
    {
        if ((keyData & (Keys.Shift | Keys.Control | Keys.Alt)) == 0)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Escape:
                    Visible = false;
                    return true;
                case Keys.F4:
                    //dataGridView.F4Selection(true);
                    return true;
                case Keys.Return:
                    // make sure the return gets forwarded to the control that
                    // is being displayed
                    //if (dataGridView.UnfocusSelection()) {
                    //dataGridView.SelectedGridEntry.OnValueReturnKey();
                    //}
                    return true;
            }
        }

        return base.ProcessDialogKey(keyData);
    }

    public virtual void SetComponent(Control ctl)
    {

        if (_currentControl is not null)
        {
            Controls.Remove(_currentControl);
            _currentControl = null;
        }

        if (ctl is not null)
        {
            Controls.Add(ctl);
            ctl.Location = new Point(BORDER, BORDER);
            ctl.Visible = true;
            _currentControl = ctl;
            UpdateSize();
            _currentControl.Resize += new EventHandler(OnCurrentControlResize);
        }

        Enabled = _currentControl is not null;
    }

    private void UpdateSize()
    {
        Size = new Size(2 * BORDER + _currentControl.Width + 2, 2 * BORDER + _currentControl.Height + 2);
    }

    protected override void WndProc(ref Message m)
    {

        if (m.Msg == NativeMethods.WM_ACTIVATE &&
            Visible &&
            NativeMethods.Util.LOWORD(unchecked((int)(long)m.WParam)) == NativeMethods.WA_INACTIVE &&
            !OwnsWindow((IntPtr)m.LParam))
        {

            Visible = false;
            return;
        }

        /*
        else if (m.Msg == NativeMethods.WM_CLOSE) {
            // don't let an ALT-F4 get you down
            //
            if (Visible) {
                //dataGridView.CloseDropDown();
            }
            return;
        } */

        base.WndProc(ref m);
    }
}
