// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    [
        ComVisible(true),
        ClassInterface(ClassInterfaceType.AutoDispatch)
    ]
    public class DataGridViewComboBoxEditingControl : ComboBox, IDataGridViewEditingControl
    {
        private DataGridView dataGridView;
        private bool valueChanged;
        private int rowIndex;

        public DataGridViewComboBoxEditingControl() : base()
        {
            TabStop = false;
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewComboBoxEditingControlAccessibleObject(this);
        }

        // IDataGridViewEditingControl interface implementation

        public virtual DataGridView EditingControlDataGridView
        {
            get
            {
                return dataGridView;
            }
            set
            {
                dataGridView = value;
            }
        }

        public virtual object EditingControlFormattedValue
        {
            get
            {
                return GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Formatting);
            }
            set
            {
                if (value is string valueStr)
                {
                    Text = valueStr;
                    if (string.Compare(valueStr, Text, true, CultureInfo.CurrentCulture) != 0)
                    {
                        SelectedIndex = -1;
                    }
                }
            }
        }

        public virtual int EditingControlRowIndex
        {
            get
            {
                return rowIndex;
            }
            set
            {
                rowIndex = value;
            }
        }

        public virtual bool EditingControlValueChanged
        {
            get
            {
                return valueChanged;
            }
            set
            {
                valueChanged = value;
            }
        }

        public virtual Cursor EditingPanelCursor
        {
            get
            {
                return Cursors.Default;
            }
        }

        public virtual bool RepositionEditingControlOnValueChange
        {
            get
            {
                return false;
            }
        }

        public virtual void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            Font = dataGridViewCellStyle.Font;
            if (dataGridViewCellStyle.BackColor.A < 255)
            {
                // Our ComboBox does not support transparent back colors
                Color opaqueBackColor = Color.FromArgb(255, dataGridViewCellStyle.BackColor);
                BackColor = opaqueBackColor;
                dataGridView.EditingPanel.BackColor = opaqueBackColor;
            }
            else
            {
                BackColor = dataGridViewCellStyle.BackColor;
            }
            ForeColor = dataGridViewCellStyle.ForeColor;
        }

        public virtual bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            if ((keyData & Keys.KeyCode) == Keys.Down ||
                (keyData & Keys.KeyCode) == Keys.Up ||
                (DroppedDown && ((keyData & Keys.KeyCode) == Keys.Escape) || (keyData & Keys.KeyCode) == Keys.Enter))
            {
                return true;
            }
            return !dataGridViewWantsInputKey;
        }

        public virtual object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return Text;
        }

        public virtual void PrepareEditingControlForEdit(bool selectAll)
        {
            if (selectAll)
            {
                SelectAll();
            }
        }

        private void NotifyDataGridViewOfValueChange()
        {
            valueChanged = true;
            dataGridView.NotifyCurrentCellDirty(true);
        }

        protected override void OnSelectedIndexChanged(EventArgs e)
        {
            base.OnSelectedIndexChanged(e);
            if (SelectedIndex != -1)
            {
                NotifyDataGridViewOfValueChange();
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            dataGridView.SetAccessibleObjectParent(this.AccessibilityObject);
        }
    }

    /// <summary>
    ///  Defines the DataGridView ComboBox EditingControl accessible object.
    /// </summary>
    internal class DataGridViewComboBoxEditingControlAccessibleObject : ComboBox.ComboBoxAccessibleObject
    {
        private readonly DataGridViewComboBoxEditingControl ownerControl;

        /// <summary>
        ///  The parent is changed when the editing control is attached to another editing cell.
        /// </summary>
        private AccessibleObject _parentAccessibleObject = null;

        public DataGridViewComboBoxEditingControlAccessibleObject(DataGridViewComboBoxEditingControl ownerControl) : base(ownerControl)
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

        internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
        {
            switch (direction)
            {
                case UnsafeNativeMethods.NavigateDirection.Parent:
                    if (Owner is IDataGridViewEditingControl owner && owner.EditingControlDataGridView.EditingControl == owner)
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

        /// <summary>
        ///  Sets the parent accessible object for the node which can be added or removed to/from hierachy nodes.
        /// </summary>
        /// <param name="parent">The parent accessible object.</param>
        internal override void SetParent(AccessibleObject parent)
        {
            _parentAccessibleObject = parent;
        }
    }
}
