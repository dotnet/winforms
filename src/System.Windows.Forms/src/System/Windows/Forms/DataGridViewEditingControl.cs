// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;


    public interface IDataGridViewEditingControl
    {

        DataGridView EditingControlDataGridView
        {
            get;
            set;
        }


        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        object EditingControlFormattedValue
        {
            get;
            set;
        }


        int EditingControlRowIndex
        {
            get;
            set;
        }


        bool EditingControlValueChanged
        {
            get;
            set;
        }


        Cursor EditingPanelCursor
        {
            get;
        }


        bool RepositionEditingControlOnValueChange
        {
            get;
        }


        void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle);


        bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey);


        [SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
        object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context);


        void PrepareEditingControlForEdit(bool selectAll);
    }


    /// <devdoc>
    ///    Implements a custom AccessibleObject that fixes editing control's accessibility ancestor chain.
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    internal class DataGridViewEditingControlAccessibleObject : Control.ControlAccessibleObject
    {
        public DataGridViewEditingControlAccessibleObject(Control ownerControl) : base(ownerControl)
        {
            Debug.Assert(ownerControl is IDataGridViewEditingControl, "ownerControl must implement IDataGridViewEditingControl");
        }

        internal override bool IsIAccessibleExSupported()
        {
            if (AccessibilityImprovements.Level3)
            {
                return true;
            }

            return base.IsIAccessibleExSupported();
        }

        public override AccessibleObject Parent
        {
            get
            {
                return (Owner as IDataGridViewEditingControl)?.EditingControlDataGridView?.CurrentCell?.AccessibilityObject;
            }
        }

        internal override bool IsPatternSupported(int patternId)
        {
            if (AccessibilityImprovements.Level3 && patternId == NativeMethods.UIA_ExpandCollapsePatternId)
            {
                ComboBox ownerComboBoxControl = Owner as ComboBox;
                if (ownerComboBoxControl != null)
                {
                    return ownerComboBoxControl.DropDownStyle != ComboBoxStyle.Simple;
                }
            }

            return base.IsPatternSupported(patternId);
        }

        internal override object GetPropertyValue(int propertyID)
        {
            if (AccessibilityImprovements.Level3 && propertyID == NativeMethods.UIA_IsExpandCollapsePatternAvailablePropertyId)
            {
                return IsPatternSupported(NativeMethods.UIA_ExpandCollapsePatternId);
            }

            return base.GetPropertyValue(propertyID);
        }

        internal override UnsafeNativeMethods.ExpandCollapseState ExpandCollapseState
        {
            get
            {
                ComboBox ownerComboBoxControl = Owner as ComboBox;
                if (ownerComboBoxControl != null)
                {
                    return ownerComboBoxControl.DroppedDown == true ? UnsafeNativeMethods.ExpandCollapseState.Expanded : UnsafeNativeMethods.ExpandCollapseState.Collapsed;
                }

                return base.ExpandCollapseState;
            }
        }
    }
}
