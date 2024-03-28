// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public abstract partial class UpDownBase
{
    internal partial class UpDownEdit : TextBox
    {
        private readonly UpDownBase _parent;
        private bool _doubleClickFired;

        internal UpDownEdit(UpDownBase parent)
        {
            SetStyle(ControlStyles.FixedHeight | ControlStyles.FixedWidth, true);

            _parent = parent;
        }

        [AllowNull]
        public override string Text
        {
            get => base.Text;
            set
            {
                if (IsAccessibilityObjectCreated && value != base.Text)
                {
                    AccessibilityObject.RaiseAutomationNotification(Automation.AutomationNotificationKind.ActionCompleted,
                        Automation.AutomationNotificationProcessing.CurrentThenMostRecent, SR.UpDownEditLocalizedControlTypeName);
                    AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_Text_TextChangedEventId);
                }

                base.Text = value;
            }
        }

        protected override AccessibleObject CreateAccessibilityInstance()
            => new UpDownEditAccessibleObject(this, _parent);

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Clicks == 2 && e.Button == MouseButtons.Left)
            {
                _doubleClickFired = true;
            }

            _parent.OnMouseDown(_parent.TranslateMouseEvent(this, e));

            if (IsHandleCreated && IsAccessibilityObjectCreated)
            {
                // As there is no corresponding windows notification
                // about text selection changed for TextBox assuming
                // that any mouse down on textbox leads to change of
                // the caret position and thereby change the selection.
                AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_Text_TextSelectionChangedEventId);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            MouseEventArgs me = _parent.TranslateMouseEvent(this, e);
            if (e.Button == MouseButtons.Left)
            {
                if (!_parent.ValidationCancelled && PInvoke.WindowFromPoint(PointToScreen(e.Location)) == HWND)
                {
                    if (!_doubleClickFired)
                    {
                        _parent.OnClick(me);
                        _parent.OnMouseClick(me);
                    }
                    else
                    {
                        _doubleClickFired = false;
                        _parent.OnDoubleClick(me);
                        _parent.OnMouseDoubleClick(me);
                    }
                }

                _doubleClickFired = false;
            }

            _parent.OnMouseUp(me);
        }

        internal override void WmContextMenu(ref Message m)
        {
            // Want to make the SourceControl to be the UpDownBase, not the Edit.
            if (ContextMenuStrip is not null)
            {
                WmContextMenu(ref m, _parent);
            }
            else
            {
                WmContextMenu(ref m, this);
            }
        }

        /// <summary>
        ///  Raises the <see cref="Control.KeyUp"/> event.
        /// </summary>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            _parent.OnKeyUp(e);

            if (IsHandleCreated && IsAccessibilityObjectCreated && ContainsNavigationKeyCode(e.KeyCode))
            {
                AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_Text_TextSelectionChangedEventId);
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            _parent.SetActiveControl(this);
            _parent.InvokeGotFocus(_parent, e);

            if (IsAccessibilityObjectCreated)
            {
                AccessibilityObject.SetFocus();
            }
        }

        protected override void OnLostFocus(EventArgs e)
            => _parent.InvokeLostFocus(_parent, e);
    }
}
