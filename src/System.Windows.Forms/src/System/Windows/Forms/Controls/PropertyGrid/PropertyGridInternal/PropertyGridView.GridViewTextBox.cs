// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyGridView
{
    private sealed partial class GridViewTextBox : TextBox, IMouseHookClient
    {
        private bool _filter;
        private int _lastMove;

        private readonly MouseHook _mouseHook;

        public GridViewTextBox(PropertyGridView gridView)
        {
            PropertyGridView = gridView;
            _mouseHook = new MouseHook(this, this, gridView);
        }

        internal PropertyGridView PropertyGridView { get; }

        public bool InSetText { get; private set; }

        /// <summary>
        ///  Setting this to true will cause this <see cref="GridViewTextBox"/> to always
        ///  report that it is not focused.
        /// </summary>
        public bool HideFocusState { private get; set; }

        public bool Filter
        {
            get => _filter;
            set => _filter = value;
        }

        public override bool Focused => !HideFocusState && base.Focused;

        [AllowNull]
        public override string Text
        {
            get => base.Text;
            set
            {
                InSetText = true;
                base.Text = value;
                InSetText = false;
            }
        }

        public bool DisableMouseHook
        {
            set => _mouseHook.DisableMouseHook = value;
        }

        public bool HookMouseDown
        {
            get => _mouseHook.HookMouseDown;
            set
            {
                _mouseHook.HookMouseDown = value;
                if (value)
                {
                    Focus();
                }
            }
        }

        protected override AccessibleObject CreateAccessibilityInstance() => new GridViewTextBoxAccessibleObject(this);

        protected override void DestroyHandle()
        {
            _mouseHook.HookMouseDown = false;
            base.DestroyHandle();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _mouseHook.Dispose();
            }

            base.Dispose(disposing);
        }

        public void FilterKeyPress(char keyChar)
        {
            if (IsInputChar(keyChar))
            {
                Focus();
                SelectAll();
                PInvokeCore.PostMessage(this, PInvokeCore.WM_CHAR, (WPARAM)keyChar);
            }
        }

        protected override bool IsInputKey(Keys keyData)
        {
            // Overridden to handle TAB key.
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Escape:
                case Keys.Tab:
                case Keys.F4:
                case Keys.F1:
                case Keys.Return:
                    return false;
            }

            if (PropertyGridView.EditTextBoxNeedsCommit)
            {
                return false;
            }

            return base.IsInputKey(keyData);
        }

        protected override bool IsInputChar(char keyChar) => (Keys)keyChar switch
        {
            // Overridden to handle TAB key.
            Keys.Tab or Keys.Return => false,
            _ => base.IsInputChar(keyChar),
        };

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (IsAccessibilityObjectCreated)
            {
                AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
            }

            // MSAA Event:
            AccessibilityNotifyClients(AccessibleEvents.Focus, childID: -1);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            // This is because on a dialog we may not get a chance to pre-process.
            if (ProcessDialogKey(e.KeyData))
            {
                e.Handled = true;
                return;
            }

            base.OnKeyDown(e);
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (!IsInputChar(e.KeyChar))
            {
                e.Handled = true;
                return;
            }

            base.OnKeyPress(e);
        }

        public bool OnClickHooked() => !PropertyGridView.CommitEditTextBox();

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            if (!Focused)
            {
                using Graphics graphics = CreateGraphics();
                if (PropertyGridView.SelectedGridEntry is not null &&
                    ClientRectangle.Width <= PropertyGridView.SelectedGridEntry.GetValueTextWidth(Text, graphics, Font))
                {
                    PropertyGridView.ToolTip.ToolTip = PasswordProtect ? "" : Text;
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            // Make sure we allow the Edit to handle ctrl-z.
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Z:
                case Keys.C:
                case Keys.X:
                case Keys.V:
                    if (((keyData & Keys.Control) != 0) && ((keyData & Keys.Shift) == 0) && ((keyData & Keys.Alt) == 0))
                    {
                        return false;
                    }

                    break;

                case Keys.A:
                    if (((keyData & Keys.Control) != 0) && ((keyData & Keys.Shift) == 0) && ((keyData & Keys.Alt) == 0))
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
                    if (((keyData & Keys.Control) == 0) && ((keyData & Keys.Shift) != 0) && ((keyData & Keys.Alt) == 0))
                    {
                        return false;
                    }
                    else if (((keyData & Keys.Control) == 0) && ((keyData & Keys.Shift) == 0) && ((keyData & Keys.Alt) == 0))
                    {
                        // If this is just the delete key and we're on a non-text editable property that
                        // is resettable, reset it now.
                        if (PropertyGridView.SelectedGridEntry is not null
                            && !PropertyGridView.SelectedGridEntry.Enumerable
                            && !PropertyGridView.SelectedGridEntry.IsTextEditable
                            && PropertyGridView.SelectedGridEntry.CanResetPropertyValue())
                        {
                            object? oldValue = PropertyGridView.SelectedGridEntry.PropertyValue;
                            PropertyGridView.SelectedGridEntry.ResetPropertyValue();
                            PropertyGridView.UnfocusSelection();
                            PropertyGridView.OwnerGrid.OnPropertyValueSet(PropertyGridView.SelectedGridEntry, oldValue);
                        }
                    }

                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // We don't do anything with modified keys here.
            if ((keyData & (Keys.Shift | Keys.Control | Keys.Alt)) == 0)
            {
                switch (keyData & Keys.KeyCode)
                {
                    case Keys.Return:
                        bool fwdReturn = !PropertyGridView.EditTextBoxNeedsCommit;
                        if (PropertyGridView.UnfocusSelection() && fwdReturn && PropertyGridView.SelectedGridEntry is not null)
                        {
                            PropertyGridView.SelectedGridEntry.OnValueReturnKey();
                        }

                        return true;
                    case Keys.Escape:
                        PropertyGridView.OnEscape(this);
                        return true;
                    case Keys.F4:
                        PropertyGridView.F4Selection(true);
                        return true;
                }
            }

            // For the tab key we want to commit before we allow it to be processed.
            if ((keyData & Keys.KeyCode) == Keys.Tab && ((keyData & (Keys.Control | Keys.Alt)) == 0))
            {
                return !PropertyGridView.CommitEditTextBox();
            }

            return base.ProcessDialogKey(keyData);
        }

        protected override void SetVisibleCore(bool value)
        {
            // Make sure we don't have the mouse captured if we're going invisible.
            if (!value && HookMouseDown)
            {
                _mouseHook.HookMouseDown = false;
            }

            base.SetVisibleCore(value);
        }

        private unsafe bool WmNotify(ref Message m)
        {
            if (m.LParamInternal != 0)
            {
                NMHDR* nmhdr = (NMHDR*)(nint)m.LParamInternal;

                if (nmhdr->hwndFrom == PropertyGridView.ToolTip.Handle)
                {
                    switch (nmhdr->code)
                    {
                        case PInvoke.TTN_SHOW:
                            PositionTooltip(this, PropertyGridView.ToolTip, ClientRectangle);
                            m.ResultInternal = (LRESULT)1;
                            return true;
                        default:
                            PropertyGridView.WndProc(ref m);
                            break;
                    }
                }
            }

            return false;
        }

        protected override void WndProc(ref Message m)
        {
            if (_filter && PropertyGridView.FilterEditWndProc(ref m))
            {
                return;
            }

            switch (m.MsgInternal)
            {
                case PInvokeCore.WM_STYLECHANGED:
                    if ((WINDOW_LONG_PTR_INDEX)(int)m.WParamInternal == WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE)
                    {
                        PropertyGridView.Invalidate();
                    }

                    break;
                case PInvokeCore.WM_MOUSEMOVE:
                    if (m.LParamInternal == _lastMove)
                    {
                        return;
                    }

                    _lastMove = (int)m.LParamInternal;
                    break;
                case PInvokeCore.WM_DESTROY:
                    _mouseHook.HookMouseDown = false;
                    break;
                case PInvokeCore.WM_SHOWWINDOW:
                    if (m.WParamInternal == 0u)
                    {
                        _mouseHook.HookMouseDown = false;
                    }

                    break;
                case PInvokeCore.WM_PASTE:
                    if (ReadOnly)
                    {
                        return;
                    }

                    break;

                case PInvokeCore.WM_GETDLGCODE:

                    m.ResultInternal = (LRESULT)(m.ResultInternal | (nint)(PInvoke.DLGC_WANTARROWS | PInvoke.DLGC_WANTCHARS));
                    if (PropertyGridView.EditTextBoxNeedsCommit || PropertyGridView.WantsTab(forward: (ModifierKeys & Keys.Shift) == 0))
                    {
                        m.ResultInternal = (LRESULT)(m.ResultInternal | (nint)(PInvoke.DLGC_WANTALLKEYS | PInvoke.DLGC_WANTTAB));
                    }

                    return;

                case PInvokeCore.WM_NOTIFY:
                    if (WmNotify(ref m))
                    {
                        return;
                    }

                    break;
            }

            base.WndProc(ref m);
        }
    }
}
