// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Drawing;
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    /// <include file='doc\DataGridViewTextBoxEditingControl.uex' path='docs/doc[@for="DataGridViewTextBoxEditingControl"]/*' />
    [
        ComVisible(true),
        ClassInterface(ClassInterfaceType.AutoDispatch)
    ]
    public class DataGridViewTextBoxEditingControl : TextBox, IDataGridViewEditingControl
    {
        private static readonly DataGridViewContentAlignment anyTop = DataGridViewContentAlignment.TopLeft | DataGridViewContentAlignment.TopCenter | DataGridViewContentAlignment.TopRight;
        private static readonly DataGridViewContentAlignment anyRight = DataGridViewContentAlignment.TopRight | DataGridViewContentAlignment.MiddleRight | DataGridViewContentAlignment.BottomRight;
        private static readonly DataGridViewContentAlignment anyCenter = DataGridViewContentAlignment.TopCenter | DataGridViewContentAlignment.MiddleCenter | DataGridViewContentAlignment.BottomCenter;

        private DataGridView dataGridView;
        private bool valueChanged;
        private bool repositionOnValueChange;
        private int rowIndex;

        /// <include file='doc\DataGridViewTextBoxEditingControl.uex' path='docs/doc[@for="DataGridViewTextBoxEditingControl.DataGridViewTextBoxEditingControl"]/*' />
        public DataGridViewTextBoxEditingControl() : base()
        {
            this.TabStop = false;
        }

        /// <include file='doc\DataGridViewTextBoxEditingControl.uex' path='docs/doc[@for="DataGridViewTextBoxEditingControl.CreateAccessibilityInstance"]/*' />
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewTextBoxEditingControlAccessibleObject(this);
        }

        /// <include file='doc\DataGridViewTextBoxEditingControl.uex' path='docs/doc[@for="DataGridViewTextBoxEditingControl.IDataGridViewEditingControl.EditingControlDataGridView"]/*' />
        public virtual DataGridView EditingControlDataGridView
        {
            get => this.dataGridView;
            set
            {
                this.dataGridView = value;
            }
        }

        /// <include file='doc\DataGridViewTextBoxEditingControl.uex' path='docs/doc[@for="DataGridViewTextBoxEditingControl.IDataGridViewEditingControl.EditingControlFormattedValue"]/*' />
        public virtual object EditingControlFormattedValue
        {
            get => GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Formatting);
            set
            {
                this.Text = (string) value;
            }
        }

        /// <include file='doc\DataGridViewTextBoxEditingControl.uex' path='docs/doc[@for="DataGridViewTextBoxEditingControl.IDataGridViewEditingControl.EditingControlRowIndex"]/*' />
        public virtual int EditingControlRowIndex
        {
            get => this.rowIndex;
            set
            {
                this.rowIndex = value;
            }
        }

        /// <include file='doc\DataGridViewTextBoxEditingControl.uex' path='docs/doc[@for="DataGridViewTextBoxEditingControl.IDataGridViewEditingControl.EditingControlValueChanged"]/*' />
        public virtual bool EditingControlValueChanged
        {
            get => this.valueChanged;
            set
            {
                this.valueChanged = value;
            }
        }

        /// <include file='doc\DataGridViewTextBoxEditingControl.uex' path='docs/doc[@for="DataGridViewTextBoxEditingControl.IDataGridViewEditingControl.EditingPanelCursor"]/*' />
        public virtual Cursor EditingPanelCursor
        {
            get
            {
                return Cursors.Default;
            }
        }

        /// <include file='doc\DataGridViewTextBoxEditingControl.uex' path='docs/doc[@for="DataGridViewTextBoxEditingControl.IDataGridViewEditingControl.RepositionEditingControlOnValueChange"]/*' />
        public virtual bool RepositionEditingControlOnValueChange
        {
            get
            {
                return this.repositionOnValueChange;
            }
        }

        internal override bool SupportsUiaProviders => true;

        /// <include file='doc\DataGridViewTextBoxEditingControl.uex' path='docs/doc[@for="DataGridViewTextBoxEditingControl.IDataGridViewEditingControl.ApplyCellStyleToEditingControl"]/*' />
        public virtual void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            if (dataGridViewCellStyle.BackColor.A < 255)
            {
                // Our TextBox does not support transparent back colors
                Color opaqueBackColor = Color.FromArgb(255, dataGridViewCellStyle.BackColor);
                this.BackColor = opaqueBackColor;
                this.dataGridView.EditingPanel.BackColor = opaqueBackColor;
            }
            else
            {
                this.BackColor = dataGridViewCellStyle.BackColor;
            }
            this.ForeColor = dataGridViewCellStyle.ForeColor;
            if (dataGridViewCellStyle.WrapMode == DataGridViewTriState.True)
            {
                this.WordWrap = true;
            }
            this.TextAlign = TranslateAlignment(dataGridViewCellStyle.Alignment);
            this.repositionOnValueChange = (dataGridViewCellStyle.WrapMode == DataGridViewTriState.True && (dataGridViewCellStyle.Alignment & anyTop) == 0);
        }

        /// <include file='doc\DataGridViewTextBoxEditingControl.uex' path='docs/doc[@for="DataGridViewTextBoxEditingControl.IDataGridViewEditingControl.EditingControlWantsInputKey"]/*' />
        public virtual bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Right:
                    // If the end of the selection is at the end of the string
                    // let the DataGridView treat the key message
                    if ((this.RightToLeft == RightToLeft.No && !(this.SelectionLength == 0 && this.SelectionStart == this.Text.Length)) ||
                        (this.RightToLeft == RightToLeft.Yes && !(this.SelectionLength == 0 && this.SelectionStart == 0)))
                    {
                        return true;
                    }
                    break;

                case Keys.Left:
                    // If the end of the selection is at the begining of the string
                    // or if the entire text is selected and we did not start editing
                    // send this character to the dataGridView, else process the key event
                    if ((this.RightToLeft == RightToLeft.No && !(this.SelectionLength == 0 && this.SelectionStart == 0)) ||
                        (this.RightToLeft == RightToLeft.Yes && !(this.SelectionLength == 0 && this.SelectionStart == this.Text.Length)))
                    {
                        return true;
                    }
                    break;

                case Keys.Down:
                    // If the end of the selection is on the last line of the text then 
                    // send this character to the dataGridView, else process the key event
                    int end = this.SelectionStart + this.SelectionLength;
                    if (this.Text.IndexOf("\r\n", end) != -1)
                    {
                        return true;
                    }
                    break;

                case Keys.Up:
                    // If the end of the selection is on the first line of the text then 
                    // send this character to the dataGridView, else process the key event
                    if (!(this.Text.IndexOf("\r\n") < 0 || this.SelectionStart + this.SelectionLength < this.Text.IndexOf("\r\n")))
                    {
                        return true;
                    }
                    break;

                case Keys.Home:
                case Keys.End:
                    if (this.SelectionLength != this.Text.Length)
                    {
                        return true;
                    }
                    break;

                case Keys.Prior:
                case Keys.Next:
                    if (this.valueChanged)
                    {
                        return true;
                    }
                    break;

                case Keys.Delete:
                    if (this.SelectionLength > 0 ||
                        this.SelectionStart < this.Text.Length)
                    {
                        return true;
                    }
                    break;

                case Keys.Enter:
                    if ((keyData & (Keys.Control | Keys.Shift | Keys.Alt)) == Keys.Shift && this.Multiline && this.AcceptsReturn)
                    {
                        return true;
                    }
                    break;
            }
            return !dataGridViewWantsInputKey;
        }

        /// <include file='doc\DataGridViewTextBoxEditingControl.uex' path='docs/doc[@for="DataGridViewTextBoxEditingControl.IDataGridViewEditingControl.GetEditingControlFormattedValue"]/*' />
        public virtual object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return this.Text;
        }

        /// <include file='doc\DataGridViewTextBoxEditingControl.uex' path='docs/doc[@for="DataGridViewTextBoxEditingControl.IDataGridViewEditingControl.PrepareEditingControlForEdit"]/*' />
        public virtual void PrepareEditingControlForEdit(bool selectAll)
        {
            if (selectAll)
            {
                SelectAll();
            }
            else
            {
                // Do not select all the text, but
                // position the caret at the end of the text
                this.SelectionStart = this.Text.Length;
            }
        }

        private void NotifyDataGridViewOfValueChange()
        {
            this.valueChanged = true;
            this.dataGridView.NotifyCurrentCellDirty(true);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            AccessibilityObject.RaiseAutomationEvent(NativeMethods.UIA_AutomationFocusChangedEventId);
        }

        /// <include file='doc\DataGridViewTextBoxEditingControl.uex' path='docs/doc[@for="DataGridViewTextBoxEditingControl.OnMouseWheel"]/*' />
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            // Forwarding to grid control. Can't prevent the TextBox from handling the mouse wheel as expected.
            this.dataGridView.OnMouseWheelInternal(e);
        }

        /// <include file='doc\DataGridViewTextBoxEditingControl.uex' path='docs/doc[@for="DataGridViewTextBoxEditingControl.OnTextChanged"]/*' />
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            // Let the DataGridView know about the value change
            NotifyDataGridViewOfValueChange();
        }

        /// <include file='doc\DataGridViewTextBoxEditingControl.uex' path='docs/doc[@for="DataGridViewTextBoxEditingControl.ProcessKeyEventArgs"]/*' />
        protected override bool ProcessKeyEventArgs(ref Message m)
        {
            switch ((Keys)(int) m.WParam)
            {
                case Keys.Enter:
                    if (m.Msg == Interop.WindowMessages.WM_CHAR &&
                        !(ModifierKeys == Keys.Shift && this.Multiline && this.AcceptsReturn))
                    {
                        // Ignore the Enter key and don't add it to the textbox content. This happens when failing validation brings
                        // up a dialog box for example.
                        // Shift-Enter for multiline textboxes need to be accepted however.
                        return true;
                    }
                    break;

                case Keys.LineFeed:
                    if (m.Msg == Interop.WindowMessages.WM_CHAR &&
                        ModifierKeys == Keys.Control && this.Multiline && this.AcceptsReturn)
                    {
                        // Ignore linefeed character when user hits Ctrl-Enter to commit the cell.
                        return true;
                    }
                    break;

                case Keys.A:
                    if (m.Msg == Interop.WindowMessages.WM_KEYDOWN && ModifierKeys == Keys.Control)
                    {
                        SelectAll();
                        return true;
                    }
                    break;

            }
            return base.ProcessKeyEventArgs(ref m);
        }

        private static HorizontalAlignment TranslateAlignment(DataGridViewContentAlignment align) 
        {
            if ((align & anyRight) != 0)
            {
                return HorizontalAlignment.Right;
            }
            else if ((align & anyCenter) != 0)
            {
                return HorizontalAlignment.Center;
            }
            else
            {
                return HorizontalAlignment.Left;
            }
        }
    }

    /// <summary>
    /// Defines the DataGridView TextBox EditingControl accessible object.
    /// </summary>
    internal class DataGridViewTextBoxEditingControlAccessibleObject : Control.ControlAccessibleObject
    {
        private DataGridViewTextBoxEditingControl ownerControl;

        /// <summary>
        /// The parent is changed when the editing control is attached to another editing cell.
        /// </summary>
        private AccessibleObject _parentAccessibleObject = null;

        public DataGridViewTextBoxEditingControlAccessibleObject(DataGridViewTextBoxEditingControl ownerControl) : base(ownerControl)
        {
            this.ownerControl = ownerControl;
        }

        public override AccessibleObject Parent
        {
            get
            {
                return _parentAccessibleObject;
            }
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
                    return SR.DataGridView_AccEditingControlAccName;
                }
            }

            set
            {
                base.Name = value;
            }
        }

        internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
        {
            switch (direction)
            {
                case UnsafeNativeMethods.NavigateDirection.Parent:
                    var owner = Owner as IDataGridViewEditingControl;
                    if (owner != null && owner.EditingControlDataGridView.EditingControl == owner)
                    {
                        return _parentAccessibleObject;
                    }

                    return null;
            }

            return base.FragmentNavigate(direction);
        }

        internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot
        {
            get
            {
                return (Owner as IDataGridViewEditingControl)?.EditingControlDataGridView?.AccessibilityObject;
            }
        }

        internal override object GetPropertyValue(int propertyID)
        {
            switch (propertyID)
            {
                case NativeMethods.UIA_ControlTypePropertyId:
                    return NativeMethods.UIA_EditControlTypeId;
                case NativeMethods.UIA_NamePropertyId:
                    return Name;
                case NativeMethods.UIA_IsValuePatternAvailablePropertyId:
                    return true;
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

        /// <summary>
        /// Sets the parent accessible object for the node which can be added or removed to/from hierachy nodes.
        /// </summary>
        /// <param name="parent">The parent accessible object.</param>
        internal override void SetParent(AccessibleObject parent)
        {
            _parentAccessibleObject = parent;
        }
    }
}
