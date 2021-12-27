// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        internal class DataGridViewEditingPanelAccessibleObject : ControlAccessibleObject
        {
            private readonly DataGridView _ownerDataGridView;
            private readonly Panel _panel;

            public DataGridViewEditingPanelAccessibleObject(DataGridView dataGridView, Panel panel) : base(panel)
            {
                _ownerDataGridView = dataGridView;
                _panel = panel;
            }

            #region IRawElementProviderFragment Implementation

            internal override Rectangle BoundingRectangle
            {
                get
                {
                    return _panel.AccessibilityObject.Bounds;
                }
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return _ownerDataGridView.AccessibilityObject;
                }
            }

            internal override int[] RuntimeId => _panel.AccessibilityObject.RuntimeId;

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        DataGridViewCell currentCell = _ownerDataGridView.CurrentCell;
                        if (currentCell is not null && _ownerDataGridView.IsCurrentCellInEditMode)
                        {
                            return currentCell.AccessibilityObject;
                        }

                        break;
                    case UiaCore.NavigateDirection.FirstChild:
                    case UiaCore.NavigateDirection.LastChild:
                        return _ownerDataGridView.EditingControlAccessibleObject;
                }

                return base.FragmentNavigate(direction);
            }

            public override string? Name => SR.DataGridView_AccEditingPanelAccName;

            internal override void SetFocus()
            {
                if (_panel.IsHandleCreated && _panel.CanFocus)
                {
                    _panel.Focus();
                }
            }

            #endregion

            #region IRawElementProviderSimple Implementation

            internal override object? GetPropertyValue(UiaCore.UIA propertyId)
            {
                switch (propertyId)
                {
                    case UiaCore.UIA.ControlTypePropertyId:
                        // If we don't set a default role for the accessible object
                        // it will be retrieved from Windows.
                        // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                        return Owner.AccessibleRole == AccessibleRole.Default
                               ? UiaCore.UIA.PaneControlTypeId
                               : base.GetPropertyValue(propertyId);
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        return true;
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                        return _ownerDataGridView.CurrentCell is not null;
                    case UiaCore.UIA.IsEnabledPropertyId:
                        return _ownerDataGridView.Enabled;
                    case UiaCore.UIA.IsControlElementPropertyId:
                    case UiaCore.UIA.IsContentElementPropertyId:
                        return true;
                    case UiaCore.UIA.IsPasswordPropertyId:
                        return false;
                    case UiaCore.UIA.AccessKeyPropertyId:
                        return _panel.AccessibilityObject.KeyboardShortcut;
                    case UiaCore.UIA.ProviderDescriptionPropertyId:
                        return SR.DataGridViewEditingPanelUiaProviderDescription;
                }

                return base.GetPropertyValue(propertyId);
            }

            #endregion
        }
    }
}
