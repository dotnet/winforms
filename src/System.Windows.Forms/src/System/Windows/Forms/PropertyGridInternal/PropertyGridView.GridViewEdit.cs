// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        private class GridViewEdit : TextBox, IMouseHookClient
        {
            internal bool fInSetText = false;
            internal bool filter = false;
            internal PropertyGridView psheet = null;
            private bool dontFocusMe = false;
            private int lastMove;

            private readonly MouseHook mouseHook;

            // We do this becuase the Focus call above doesn't always stick, so
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
                get
                {
                    return base.Text;
                }
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
                    UnsafeNativeMethods.PostMessage(new HandleRef(this, Handle), WindowMessages.WM_CHAR, (IntPtr)keyChar, IntPtr.Zero);
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
                                psheet._ownerGrid.OnPropertyValueSet(psheet.SelectedGridEntry, oldValue);
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
                            if (psheet.UnfocusSelection() && fwdReturn)
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
                    NativeMethods.NMHDR* nmhdr = (NativeMethods.NMHDR*)m.LParam;

                    if (nmhdr->hwndFrom == psheet.ToolTip.Handle)
                    {
                        switch (nmhdr->code)
                        {
                            case NativeMethods.TTN_SHOW:
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

                switch (m.Msg)
                {
                    case WindowMessages.WM_STYLECHANGED:
                        if ((unchecked((int)(long)m.WParam) & NativeMethods.GWL_EXSTYLE) != 0)
                        {
                            psheet.Invalidate();
                        }
                        break;
                    case WindowMessages.WM_MOUSEMOVE:
                        if (unchecked((int)(long)m.LParam) == lastMove)
                        {
                            return;
                        }
                        lastMove = unchecked((int)(long)m.LParam);
                        break;
                    case WindowMessages.WM_DESTROY:
                        mouseHook.HookMouseDown = false;
                        break;
                    case WindowMessages.WM_SHOWWINDOW:
                        if (IntPtr.Zero == m.WParam)
                        {
                            mouseHook.HookMouseDown = false;
                        }
                        break;
                    case WindowMessages.WM_PASTE:
                        /*if (!this.ReadOnly) {
                            IDataObject dataObject = Clipboard.GetDataObject();
                            Debug.Assert(dataObject != null, "Failed to get dataObject from clipboard");
                            if (dataObject != null) {
                                object data = dataObject.GetData(typeof(string));
                                if (data != null) {
                                    string clipboardText = data.ToString();
                                    SelectedText = clipboardText;
                                    m.result = 1;
                                    return;
                                }
                            }
                        }*/
                        if (ReadOnly)
                        {
                            return;
                        }
                        break;

                    case WindowMessages.WM_GETDLGCODE:

                        m.Result = (IntPtr)((long)m.Result | NativeMethods.DLGC_WANTARROWS | NativeMethods.DLGC_WANTCHARS);
                        if (psheet.NeedsCommit || WantsTab((ModifierKeys & Keys.Shift) == 0))
                        {
                            m.Result = (IntPtr)((long)m.Result | NativeMethods.DLGC_WANTALLKEYS | NativeMethods.DLGC_WANTTAB);
                        }
                        return;

                    case WindowMessages.WM_NOTIFY:
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

            [ComVisible(true)]
            protected class GridViewEditAccessibleObject : ControlAccessibleObject
            {

                private readonly PropertyGridView propertyGridView;

                public GridViewEditAccessibleObject(GridViewEdit owner) : base(owner)
                {
                    propertyGridView = owner.psheet;
                }

                public override AccessibleStates State
                {
                    get
                    {
                        AccessibleStates states = base.State;
                        if (IsReadOnly)
                        {
                            states |= AccessibleStates.ReadOnly;
                        }
                        else
                        {
                            states &= ~AccessibleStates.ReadOnly;
                        }
                        return states;
                    }
                }

                internal override bool IsIAccessibleExSupported() => true;

                /// <summary>
                ///  Returns the element in the specified direction.
                /// </summary>
                /// <param name="direction">Indicates the direction in which to navigate.</param>
                /// <returns>Returns the element in the specified direction.</returns>
                internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
                {
                    if (direction == UnsafeNativeMethods.NavigateDirection.Parent)
                    {
                        return propertyGridView.SelectedGridEntry.AccessibilityObject;
                    }
                    else if (direction == UnsafeNativeMethods.NavigateDirection.NextSibling)
                    {
                        if (propertyGridView.DropDownButton.Visible)
                        {
                            return propertyGridView.DropDownButton.AccessibilityObject;
                        }
                        else if (propertyGridView.DialogButton.Visible)
                        {
                            return propertyGridView.DialogButton.AccessibilityObject;
                        }
                    }

                    return base.FragmentNavigate(direction);
                }

                /// <summary>
                ///  Gets the top level element.
                /// </summary>
                internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot
                {
                    get
                    {
                        return propertyGridView.AccessibilityObject;
                    }
                }

                internal override object GetPropertyValue(int propertyID)
                {
                    if (propertyID == NativeMethods.UIA_IsEnabledPropertyId)
                    {
                        return !IsReadOnly;
                    }
                    else if (propertyID == NativeMethods.UIA_IsValuePatternAvailablePropertyId)
                    {
                        return IsPatternSupported(NativeMethods.UIA_ValuePatternId);
                    }
                    else if (propertyID == NativeMethods.UIA_ControlTypePropertyId)
                    {
                        return NativeMethods.UIA_EditControlTypeId;
                    }
                    else if (propertyID == NativeMethods.UIA_NamePropertyId)
                    {
                        return Name;
                    }

                    return base.GetPropertyValue(propertyID);
                }

                internal override bool IsPatternSupported(int patternId)
                {
                    if (patternId == NativeMethods.UIA_ValuePatternId)
                    {
                        return true;
                    }

                    return base.IsPatternSupported(patternId);
                }

                public override string Name
                {
                    get
                    {
                        string name = Owner.AccessibleName;
                        if (name != null)
                        {
                            return name;
                        }
                        else
                        {
                            GridEntry selectedGridEntry = propertyGridView.SelectedGridEntry;
                            if (selectedGridEntry != null)
                            {
                                return selectedGridEntry.AccessibilityObject.Name;
                            }
                        }

                        return base.Name;
                    }

                    set
                    {
                        base.Name = value;
                    }
                }

                #region IValueProvider

                internal override bool IsReadOnly
                {
                    get
                    {
                        return !(propertyGridView.SelectedGridEntry is PropertyDescriptorGridEntry propertyDescriptorGridEntry) || propertyDescriptorGridEntry.IsPropertyReadOnly;
                    }
                }

                #endregion

                internal override void SetFocus()
                {
                    RaiseAutomationEvent(NativeMethods.UIA_AutomationFocusChangedEventId);

                    base.SetFocus();
                }
            }
        }

    }
}
