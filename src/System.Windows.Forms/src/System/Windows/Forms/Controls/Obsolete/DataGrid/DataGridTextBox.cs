// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#pragma warning disable RS0016
#nullable disable
[Obsolete("DataGridTextBox has been deprecated.")]
public class DataGridTextBox : TextBox
{
    public DataGridTextBox() : base()
    {
        throw new PlatformNotSupportedException();
    }

    public void SetDataGrid(DataGrid parentGrid)
    {
        throw new PlatformNotSupportedException();
    }

    protected override void WndProc(ref Message m)
    {
        // but what if we get a CtrlV?
        // what about deleting from the menu?
        if (m.Msg == PInvoke.WM_PASTE || m.Msg == PInvoke.WM_CUT || m.Msg == PInvoke.WM_CLEAR)
        {
            IsInEditOrNavigateMode = false;
        }

        base.WndProc(ref m);
    }

    protected override void OnMouseWheel(MouseEventArgs e)
    {
    }

    protected override void OnKeyPress(KeyPressEventArgs e)
    {
        base.OnKeyPress(e);

        if (e.KeyChar == ' ' && (ModifierKeys & Keys.Shift) == Keys.Shift)
            return;

        if (ReadOnly)
            return;

        if ((ModifierKeys & Keys.Control) == Keys.Control && ((ModifierKeys & Keys.Alt) == 0))
            return;
        IsInEditOrNavigateMode = false;
    }

    protected internal override bool ProcessKeyMessage(ref Message m)
    {
        Keys key = (Keys)unchecked((int)(long)m.WParam);
        Keys modifierKeys = ModifierKeys;

        if ((key | modifierKeys) == Keys.Enter ||
            (key | modifierKeys) == Keys.Escape ||
            ((key | modifierKeys) == (Keys.Enter | Keys.Control)))
        {
            return ProcessKeyPreview(ref m);
        }

        Keys keyData = key & Keys.KeyCode;

        switch (keyData)
        {
            case Keys.Right:
                if (SelectionStart + SelectionLength == Text.Length)
                    return ProcessKeyPreview(ref m);
                return ProcessKeyEventArgs(ref m);
            case Keys.Left:
                if (SelectionStart + SelectionLength == 0 ||
                    (IsInEditOrNavigateMode && SelectionLength == Text.Length))
                    return ProcessKeyPreview(ref m);
                return ProcessKeyEventArgs(ref m);
            case Keys.Down:
                int end = SelectionStart + SelectionLength;
                if (Text.IndexOf("\r\n", end) == -1)
                    return ProcessKeyPreview(ref m);
                return ProcessKeyEventArgs(ref m);
            case Keys.Up:
                if (Text.IndexOf("\r\n") < 0 || SelectionStart + SelectionLength < Text.IndexOf("\r\n"))
                    return ProcessKeyPreview(ref m);
                return ProcessKeyEventArgs(ref m);
            case Keys.Home:
            case Keys.End:
                if (SelectionLength == Text.Length)
                    return ProcessKeyPreview(ref m);
                else
                    return ProcessKeyEventArgs(ref m);
            case Keys.Prior:
            case Keys.Next:
            case Keys.Oemplus:
            case Keys.Add:
            case Keys.OemMinus:
            case Keys.Subtract:
                if (IsInEditOrNavigateMode)
                {
                    return ProcessKeyPreview(ref m);
                }
                else
                {
                    return ProcessKeyEventArgs(ref m);
                }

            case Keys.Space:
                if (IsInEditOrNavigateMode && (ModifierKeys & Keys.Shift) == Keys.Shift)
                {
                    return ProcessKeyPreview(ref m);
                }

                return ProcessKeyEventArgs(ref m);
            case Keys.A:
                if (IsInEditOrNavigateMode && (ModifierKeys & Keys.Control) == Keys.Control)
                {
                    return ProcessKeyPreview(ref m);
                }

                return ProcessKeyEventArgs(ref m);
            case Keys.F2:
                IsInEditOrNavigateMode = false;
                SelectionStart = Text.Length;
                return true;
            case Keys.Delete:
                if (IsInEditOrNavigateMode)
                {
                    if (ProcessKeyPreview(ref m))
                        return true;
                    else
                    {
                        IsInEditOrNavigateMode = false;
                        return ProcessKeyEventArgs(ref m);
                    }
                }
                else
                    return ProcessKeyEventArgs(ref m);
            case Keys.Tab:
                if ((ModifierKeys & Keys.Control) == Keys.Control)
                    return ProcessKeyPreview(ref m);
                else
                    return ProcessKeyEventArgs(ref m);
            default:
                return ProcessKeyEventArgs(ref m);
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool IsInEditOrNavigateMode
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }
}
