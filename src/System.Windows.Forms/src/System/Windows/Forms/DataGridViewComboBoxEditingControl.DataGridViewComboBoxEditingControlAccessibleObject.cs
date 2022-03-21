﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridViewComboBoxEditingControl
    {
        /// <summary>
        ///  Defines the DataGridView ComboBox EditingControl accessible object.
        /// </summary>
        internal class DataGridViewComboBoxEditingControlAccessibleObject : ComboBoxAccessibleObject
        {
            private readonly DataGridViewComboBoxEditingControl _ownerControl;

            /// <summary>
            ///  The parent is changed when the editing control is attached to another editing cell.
            /// </summary>
            private AccessibleObject? _parentAccessibleObject;

            public DataGridViewComboBoxEditingControlAccessibleObject(DataGridViewComboBoxEditingControl ownerControl)
                : base(ownerControl)
            {
                _ownerControl = ownerControl;
            }

            public override AccessibleObject? Parent
            {
                get
                {
                    return _parentAccessibleObject;
                }
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        if (Owner is IDataGridViewEditingControl owner
                            && owner.EditingControlDataGridView?.EditingControl == owner
                            && Owner.ToolStripControlHost is null)
                        {
                            return _parentAccessibleObject;
                        }

                        break;
                }

                return base.FragmentNavigate(direction);
            }

            internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot
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
                    return _ownerControl.DropDownStyle != ComboBoxStyle.Simple;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override UiaCore.ExpandCollapseState ExpandCollapseState
            {
                get
                {
                    return _ownerControl.DroppedDown ? UiaCore.ExpandCollapseState.Expanded : UiaCore.ExpandCollapseState.Collapsed;
                }
            }

            /// <summary>
            ///  Sets the parent accessible object for the node which can be added or removed to/from hierarchy nodes.
            /// </summary>
            /// <param name="parent">The parent accessible object.</param>
            internal override void SetParent(AccessibleObject? parent)
            {
                _parentAccessibleObject = parent;
            }
        }
    }
}
