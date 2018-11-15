// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Drawing;
    using System.Diagnostics;
    using System.Globalization;
    using System.Runtime.InteropServices;
    using Security.Permissions;

    /// <include file='doc\DataGridViewComboBoxEditingControl.uex' path='docs/doc[@for="DataGridViewComboBoxEditingControl"]/*' />
    [
        ComVisible(true),
        ClassInterface(ClassInterfaceType.AutoDispatch)
    ]
    public class DataGridViewComboBoxEditingControl : ComboBox, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private bool valueChanged;
        private int rowIndex;

        /// <include file='doc\DataGridViewComboBoxEditingControl.uex' path='docs/doc[@for="DataGridViewComboBoxEditingControl.DataGridViewComboBoxEditingControl"]/*' />
        public DataGridViewComboBoxEditingControl() : base()
        {
            this.TabStop = false;
        }

        /// <include file='doc\DataGridViewComboBoxEditingControl.uex' path='docs/doc[@for="DataGridViewComboBoxEditingControl.CreateAccessibilityInstance"]/*' />
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            if (AccessibilityImprovements.Level3)
            {
                return new DataGridViewComboBoxEditingControlAccessibleObject(this);
            }
            else if (AccessibilityImprovements.Level2)
            {
                return new DataGridViewEditingControlAccessibleObject(this);
            }

            return base.CreateAccessibilityInstance();
        }

        // IDataGridViewEditingControl interface implementation

        /// <include file='doc\DataGridViewComboBoxEditingControl.uex' path='docs/doc[@for="DataGridViewComboBoxEditingControl.IDataGridViewEditingControl.EditingControlDataGridView"]/*' />
        public virtual DataGridView EditingControlDataGridView
        {
            get
            {
                return this.dataGridView;
            }
            set
            {
                this.dataGridView = value;
            }
        }

        /// <include file='doc\DataGridViewComboBoxEditingControl.uex' path='docs/doc[@for="DataGridViewComboBoxEditingControl.IDataGridViewEditingControl.EditingControlFormattedValue"]/*' />
        public virtual object EditingControlFormattedValue
        {
            get
            {
                return GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Formatting);
            }
            set
            {
                string valueStr = value as string;
                if (valueStr != null)
                {
                    this.Text = valueStr;
                    if (String.Compare(valueStr, this.Text, true, CultureInfo.CurrentCulture) != 0)
                    {
                        this.SelectedIndex = -1;
                    }
                }
            }
        }

        /// <include file='doc\DataGridViewComboBoxEditingControl.uex' path='docs/doc[@for="DataGridViewComboBoxEditingControl.IDataGridViewEditingControl.EditingControlRowIndex"]/*' />
        public virtual int EditingControlRowIndex
        {
            get
            {
                return this.rowIndex;
            }
            set
            {
                this.rowIndex = value;
            }
        }

        /// <include file='doc\DataGridViewComboBoxEditingControl.uex' path='docs/doc[@for="DataGridViewComboBoxEditingControl.IDataGridViewEditingControl.EditingControlValueChanged"]/*' />
        public virtual bool EditingControlValueChanged
        {
            get
            {
                return this.valueChanged;
            }
            set
            {
                this.valueChanged = value;
            }
        }

        /// <include file='doc\DataGridViewComboBoxEditingControl.uex' path='docs/doc[@for="DataGridViewComboBoxEditingControl.IDataGridViewEditingControl.EditingPanelCursor"]/*' />
        public virtual Cursor EditingPanelCursor
        {
            get
            {
                return Cursors.Default;
            }
        }

        /// <include file='doc\DataGridViewComboBoxEditingControl.uex' path='docs/doc[@for="DataGridViewComboBoxEditingControl.IDataGridViewEditingControl.RepositionOnValueChange"]/*' />
        public virtual bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        /// <include file='doc\DataGridViewComboBoxEditingControl.uex' path='docs/doc[@for="DataGridViewComboBoxEditingControl.IDataGridViewEditingControl.ApplyCellStyleToEditingControl"]/*' />
        public virtual void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            if (dataGridViewCellStyle.BackColor.A < 255)
            {
                // Our ComboBox does not support transparent back colors
                Color opaqueBackColor = Color.FromArgb(255, dataGridViewCellStyle.BackColor);
                this.BackColor = opaqueBackColor;
                this.dataGridView.EditingPanel.BackColor = opaqueBackColor;
            }
            else
            {
                this.BackColor = dataGridViewCellStyle.BackColor;
            }
            this.ForeColor = dataGridViewCellStyle.ForeColor;
        }

        /// <include file='doc\DataGridViewComboBoxEditingControl.uex' path='docs/doc[@for="DataGridViewComboBoxEditingControl.IDataGridViewEditingControl.EditingControlWantsInputKey"]/*' />
        public virtual bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            if ((keyData & Keys.KeyCode) == Keys.Down ||
                (keyData & Keys.KeyCode) == Keys.Up ||
                (this.DroppedDown && ((keyData & Keys.KeyCode) == Keys.Escape) || (keyData & Keys.KeyCode) == Keys.Enter))
            {
                return true;
            }
            return !dataGridViewWantsInputKey;
        }

        /// <include file='doc\DataGridViewComboBoxEditingControl.uex' path='docs/doc[@for="DataGridViewComboBoxEditingControl.IDataGridViewEditingControl.GetEditingControlFormattedValue"]/*' />
        public virtual object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return this.Text;
        }

        /// <include file='doc\DataGridViewComboBoxEditingControl.uex' path='docs/doc[@for="DataGridViewComboBoxEditingControl.IDataGridViewEditingControl.PrepareEditingControlForEdit"]/*' />
        public virtual void PrepareEditingControlForEdit(bool selectAll)
        {
            if (selectAll)
            {
                SelectAll();
            }
        }

        private void NotifyDataGridViewOfValueChange()
        {
            this.valueChanged = true;
            this.dataGridView.NotifyCurrentCellDirty(true);
        }

        /// <include file='doc\DataGridViewComboBoxEditingControl.uex' path='docs/doc[@for="DataGridViewComboBoxEditingControl.OnSelectedIndexChanged"]/*' />
        protected override void OnSelectedIndexChanged(EventArgs e) 
        {
            base.OnSelectedIndexChanged(e);
            if (this.SelectedIndex != -1)
            {
                NotifyDataGridViewOfValueChange();
            }
        }
    }

    /// <summary>
    /// Defines the DataGridView ComboBox EditingControl accessible object.
    /// </summary>
    /// <remarks>
    /// This accessible object is only available in AccessibilityImprovements of Level 3.
    /// </remarks>
    internal class DataGridViewComboBoxEditingControlAccessibleObject : ComboBox.ComboBoxUiaProvider
    {
        private DataGridViewComboBoxEditingControl ownerControl;

        public DataGridViewComboBoxEditingControlAccessibleObject(DataGridViewComboBoxEditingControl ownerControl) : base(ownerControl)
        {
            this.ownerControl = ownerControl;
        }

        public override AccessibleObject Parent
        {
            get
            {
                return (Owner as IDataGridViewEditingControl)?.EditingControlDataGridView?.EditingPanel.AccessibilityObject;
            }
        }

        internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
        {
            switch (direction)
            {
                case UnsafeNativeMethods.NavigateDirection.Parent:
                    return Parent;
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

        internal override bool IsPatternSupported(int patternId)
        {
            if (patternId == NativeMethods.UIA_ExpandCollapsePatternId)
            {
                return ownerControl.DropDownStyle != ComboBoxStyle.Simple;
            }

            return base.IsPatternSupported(patternId);
        }

        internal override object GetPropertyValue(int propertyID)
        {
            if (propertyID == NativeMethods.UIA_IsExpandCollapsePatternAvailablePropertyId)
            {
                return IsPatternSupported(NativeMethods.UIA_ExpandCollapsePatternId);
            }

            return base.GetPropertyValue(propertyID);
        }

        internal override UnsafeNativeMethods.ExpandCollapseState ExpandCollapseState
        {
            get
            {
                return ownerControl.DroppedDown ? UnsafeNativeMethods.ExpandCollapseState.Expanded : UnsafeNativeMethods.ExpandCollapseState.Collapsed;
            }
        }
    }
}
