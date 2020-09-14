// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        private partial class GridViewEdit : TextBox, IMouseHookClient
        {
            internal bool fInSetText;
            internal bool filter;
            internal PropertyGridView psheet;
            private bool dontFocusMe;
            private int lastMove;

            private readonly MouseHook mouseHook;

            // We do this because the Focus call above doesn't always stick, so
            // we make the Edit think that it doesn't have focus.  this prevents
            // ActiveControl code on the containercontrol from moving focus elsewhere
            // when the dropdown closes.
            public bool DontFocus
            {
                set
                {
                    dontFocusMe = value;
                }
            }

            public virtual bool Filter
            {
                get { return filter; }

                set
                {
                    filter = value;
                }
            }

            /// <summary>
            ///  Indicates whether or not the control supports UIA Providers via
            ///  IRawElementProviderFragment/IRawElementProviderFragmentRoot interfaces
            /// </summary>
            internal override bool SupportsUiaProviders => true;

            public override bool Focused
            {
                get
                {
                    if (dontFocusMe)
                    {
                        return false;
                    }
                    return base.Focused;
                }
            }

            public override string Text
            {
                get => base.Text;
                set
                {
                    fInSetText = true;
                    base.Text = value;
                    fInSetText = false;
                }
            }

            public bool DisableMouseHook
            {
                set
                {
                    mouseHook.DisableMouseHook = value;
                }
            }

            public virtual bool HookMouseDown
            {
                get
                {
                    return mouseHook.HookMouseDown;
                }
                set
                {
                    mouseHook.HookMouseDown = value;
                    if (value)
                    {
                        Focus();
                    }
                }
            }

            public GridViewEdit(PropertyGridView psheet)
            {
                this.psheet = psheet;
                mouseHook = new MouseHook(this, this, psheet);
            }

            /// <summary>
            ///  Creates a new AccessibleObject for this GridViewEdit instance.
            ///  The AccessibleObject instance returned by this method overrides several UIA properties.
            /// </summary>
            /// <returns>
            ///  AccessibleObject for this GridViewEdit instance.
            /// </returns>
            protected override AccessibleObject CreateAccessibilityInstance()
            {
                return new GridViewEditAccessibleObject(this);
            }

            protected override void DestroyHandle()
            {
                mouseHook.HookMouseDown = false;
                base.DestroyHandle();
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    mouseHook.Dispose();
                }
                base.Dispose(disposing);
            }

            public void FilterKeyPress(char keyChar)
            {
                if (IsInputChar(keyChar))
                {
                    Focus();
                    SelectAll();
                    User32.PostMessageW(this, User32.WM.CHAR, (IntPtr)keyChar);
                }
            }

            /// <summary>
            ///  Overridden to handle TAB key.
            /// </summary>
            protected override bool IsInputKey(Keys keyData)
            {
                switch (keyData & Keys.KeyCode)
                {
                    case Keys.Escape:
                    case Keys.Tab:
                    case Keys.F4:
                    case Keys.F1:
                    case Keys.Return:
                        return false;
                }
                if (psheet.NeedsCommit)
                {
                    return false;
                }
                return base.IsInputKey(keyData);
            }

            /// <summary>
            ///  Overridden to handle TAB key.
            /// </summary>
            protected override bool IsInputChar(char keyChar)
            {
                switch ((Keys)(int)keyChar)
                {
                    case Keys.Tab:
                    case Keys.Return:
                        return false;
                }
                return base.IsInputChar(keyChar);
            }

            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);

                this.AccessibilityObject.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
            }

            protected override void OnKeyDown(KeyEventArgs ke)
            {
                // this is because on a dialog we may
                // not get a chance to pre-process
                //
                if (ProcessDialogKey(ke.KeyData))
                {
                    ke.Handled = true;
                    return;
                }

                base.OnKeyDown(ke);
            }

            protected override void OnKeyPress(KeyPressEventArgs ke)
            {
                if (!IsInputChar(ke.KeyChar))
                {
                    ke.Handled = true;
                    return;
                }
                base.OnKeyPress(ke);
            }

            public bool OnClickHooked()
            {
                // can we commit this value?
                // eat the value if we failed to commit.
                return !psheet._Commit();
            }

            protected override void OnMouseEnter(EventArgs e)
            {
                base.OnMouseEnter(e);

                if (!Focused)
                {
                    Graphics g = CreateGraphics();
                    if (psheet.SelectedGridEntry != null &&
                        ClientRectangle.Width <= psheet.SelectedGridEntry.GetValueTextWidth(Text, g, Font))
                    {
                        psheet.ToolTip.ToolTip = PasswordProtect ? "" : Text;
                    }
                    g.Dispose();
                }
            }

            protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
            {
                // make sure we allow the Edit to handle ctrl-z
                switch (keyData & Keys.KeyCode)
                {
                    case Keys.Z:
                    case Keys.C:
                    case Keys.X:
                    case Keys.V:
                        if (
                           ((keyData & Keys.Control) != 0) &&
                           ((keyData & Keys.Shift) == 0) &&
                           ((keyData & Keys.Alt) == 0))
                        {
                            return false;
                        }
                        break;

                    case Keys.A:
                        if (
                           ((keyData & Keys.Control) != 0) &&
                           ((keyData & Keys.Shift) == 0) &&
                           ((keyData & Keys.Alt) == 0))
                        {
                            SelectAll();
                            return true;
                        }

                        break;

                    case Keys.Insert:
                        if (((keyData & Keys.Alt) == 0))
                        {
                            if (((keyData & Keys.Control) != 0) ^ ((keyData & Keys.Shift) == 0))
                            {
                                return false;
                            }
                        }
                        break;

                    case Keys.Delete:
                        if (
                          ((keyData & Keys.Control) == 0) &&
                          ((keyData & Keys.Shift) != 0) &&
                          ((keyData & Keys.Alt) == 0))
                        {
                            return false;
                        }
                        else if (
                              ((keyData & Keys.Control) == 0) &&
                              ((keyData & Keys.Shift) == 0) &&
                              ((keyData & Keys.Alt) == 0)
                                )
                        {
                            // if this is just the delete key and we're on a non-text editable property that is resettable,
                            // reset it now.
                            //
                            if (psheet.SelectedGridEntry != null && !psheet.SelectedGridEntry.Enumerable && !psheet.SelectedGridEntry.IsTextEditable && psheet.SelectedGridEntry.CanResetPropertyValue())
                            {
                                object oldValue = psheet.SelectedGridEntry.PropertyValue;
                                psheet.SelectedGridEntry.ResetPropertyValue();
                                psheet.UnfocusSelection();
                                psheet.OwnerGrid.OnPropertyValueSet(psheet.SelectedGridEntry, oldValue);
                            }
                        }
                        break;
                }
                return base.ProcessCmdKey(ref msg, keyData);
            }

            /// <summary>
            ///  Overrides Control.ProcessDialogKey to handle the Escape and Return
            ///  keys.
            /// </summary>
            protected override bool ProcessDialogKey(Keys keyData)
            {
                // We don't do anything with modified keys here.
                //
                if ((keyData & (Keys.Shift | Keys.Control | Keys.Alt)) == 0)
                {
                    switch (keyData & Keys.KeyCode)
                    {
                        case Keys.Return:
                            bool fwdReturn = !psheet.NeedsCommit;
                            if (psheet.UnfocusSelection() && fwdReturn && psheet.SelectedGridEntry != null)
                            {
                                psheet.SelectedGridEntry.OnValueReturnKey();
                            }
                            return true;
                        case Keys.Escape:
                            psheet.OnEscape(this);
                            return true;
                        case Keys.F4:
                            psheet.F4Selection(true);
                            return true;
                    }
                }

                // for the tab key, we want to commit before we allow it to be processed.
                if ((keyData & Keys.KeyCode) == Keys.Tab && ((keyData & (Keys.Control | Keys.Alt)) == 0))
                {
                    return !psheet._Commit();
                }

                return base.ProcessDialogKey(keyData);
            }

            protected override void SetVisibleCore(bool value)
            {
                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "DropDownHolder:Visible(" + (value.ToString()) + ")");
                // make sure we dont' have the mouse captured if
                // we're going invisible
                if (value == false && HookMouseDown)
                {
                    mouseHook.HookMouseDown = false;
                }
                base.SetVisibleCore(value);
            }

            // a mini version of process dialog key
            // for responding to WM_GETDLGCODE
            internal bool WantsTab(bool forward)
            {
                return psheet.WantsTab(forward);
            }

            private unsafe bool WmNotify(ref Message m)
            {
                if (m.LParam != IntPtr.Zero)
                {
                    User32.NMHDR* nmhdr = (User32.NMHDR*)m.LParam;

                    if (nmhdr->hwndFrom == psheet.ToolTip.Handle)
                    {
                        switch ((ComCtl32.TTN)nmhdr->code)
                        {
                            case ComCtl32.TTN.SHOW:
                                PropertyGridView.PositionTooltip(this, psheet.ToolTip, ClientRectangle);
                                m.Result = (IntPtr)1;
                                return true;
                            default:
                                psheet.WndProc(ref m);
                                break;
                        }
                    }
                }
                return false;
            }

            protected override void WndProc(ref Message m)
            {
                if (filter)
                {
                    if (psheet.FilterEditWndProc(ref m))
                    {
                        return;
                    }
                }

                switch ((User32.WM)m.Msg)
                {
                    case User32.WM.STYLECHANGED:
                        if ((unchecked((int)(long)m.WParam) & (int)User32.GWL.EXSTYLE) != 0)
                        {
                            psheet.Invalidate();
                        }
                        break;
                    case User32.WM.MOUSEMOVE:
                        if (unchecked((int)(long)m.LParam) == lastMove)
                        {
                            return;
                        }
                        lastMove = unchecked((int)(long)m.LParam);
                        break;
                    case User32.WM.DESTROY:
                        mouseHook.HookMouseDown = false;
                        break;
                    case User32.WM.SHOWWINDOW:
                        if (IntPtr.Zero == m.WParam)
                        {
                            mouseHook.HookMouseDown = false;
                        }
                        break;
                    case User32.WM.PASTE:
                        if (ReadOnly)
                        {
                            return;
                        }
                        break;

                    case User32.WM.GETDLGCODE:

                        m.Result = (IntPtr)((long)m.Result | (int)User32.DLGC.WANTARROWS | (int)User32.DLGC.WANTCHARS);
                        if (psheet.NeedsCommit || WantsTab((ModifierKeys & Keys.Shift) == 0))
                        {
                            m.Result = (IntPtr)((long)m.Result | (int)User32.DLGC.WANTALLKEYS | (int)User32.DLGC.WANTTAB);
                        }
                        return;

                    case User32.WM.NOTIFY:
                        if (WmNotify(ref m))
                        {
                            return;
                        }

                        break;
                }
                base.WndProc(ref m);
            }

            public virtual bool InSetText()
            {
                return fInSetText;
            }
        }
    }
}
