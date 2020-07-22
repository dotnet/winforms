// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Globalization;
using static Interop;

namespace System.Windows.Forms
{
    public class DataGridViewComboBoxEditingControl : ComboBox, IDataGridViewEditingControl
    {
        private DataGridView _dataGridView;
        private bool _valueChanged;
        private int _rowIndex;

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
                return _dataGridView;
            }
            set
            {
                _dataGridView = value;
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
                return _rowIndex;
            }
            set
            {
                _rowIndex = value;
            }
        }

        public virtual bool EditingControlValueChanged
        {
            get
            {
                return _valueChanged;
            }
            set
            {
                _valueChanged = value;
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
                _dataGridView.EditingPanel.BackColor = opaqueBackColor;
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
#pragma warning disable SA1408 // Conditional expressions should declare precedence
                (DroppedDown && ((keyData & Keys.KeyCode) == Keys.Escape) || (keyData & Keys.KeyCode) == Keys.Enter))
#pragma warning restore SA1408 // Conditional expressions should declare precedence
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
            _valueChanged = true;
            _dataGridView.NotifyCurrentCellDirty(true);
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

            _dataGridView?.SetAccessibleObjectParent(this.AccessibilityObject);
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
        private AccessibleObject _parentAccessibleObject;

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

        internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
        {
            switch (direction)
            {
                case UiaCore.NavigateDirection.Parent:
                    if (Owner is IDataGridViewEditingControl owner && owner.EditingControlDataGridView.EditingControl == owner)
                    {
                        return _parentAccessibleObject;
                    }

                    return null;
            }

            return base.FragmentNavigate(direction);
        }

        internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
        {
            get
            {
                return (Owner as IDataGridViewEditingControl)?.EditingControlDataGridView?.AccessibilityObject;
            }
        }

        internal override bool IsPatternSupported(UiaCore.UIA patternId)
        {
            if (patternId == UiaCore.UIA.ExpandCollapsePatternId)
            {
                return ownerControl.DropDownStyle != ComboBoxStyle.Simple;
            }

            return base.IsPatternSupported(patternId);
        }

        internal override object GetPropertyValue(UiaCore.UIA propertyID)
        {
            if (propertyID == UiaCore.UIA.IsExpandCollapsePatternAvailablePropertyId)
            {
                return IsPatternSupported(UiaCore.UIA.ExpandCollapsePatternId);
            }

            return base.GetPropertyValue(propertyID);
        }

        internal override UiaCore.ExpandCollapseState ExpandCollapseState
        {
            get
            {
                return ownerControl.DroppedDown ? UiaCore.ExpandCollapseState.Expanded : UiaCore.ExpandCollapseState.Collapsed;
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
