// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        internal class DataGridViewEditingPanelAccessibleObject : ControlAccessibleObject
        {
            private readonly DataGridView _dataGridView;
            private readonly Panel _panel;

            public DataGridViewEditingPanelAccessibleObject(DataGridView dataGridView, Panel panel) : base(panel)
            {
                _dataGridView = dataGridView;
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
                    return _dataGridView.AccessibilityObject;
                }
            }

            internal override int[] RuntimeId
            {
                get
                {
                    return _panel.AccessibilityObject.RuntimeId;
                }
            }

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        DataGridViewCell currentCell = _dataGridView.CurrentCell;
                        if (currentCell != null && _dataGridView.IsCurrentCellInEditMode)
                        {
                            return currentCell.AccessibilityObject;
                        }
                        break;
                    case UiaCore.NavigateDirection.FirstChild:
                    case UiaCore.NavigateDirection.LastChild:
                        return _dataGridView.EditingControlAccessibleObject;
                }

                return null;
            }

            internal override void SetFocus()
            {
                if (_panel.IsHandleCreated && _panel.CanFocus)
                {
                    _panel.Focus();
                }
            }

            #endregion

            #region IRawElementProviderSimple Implementation

            internal override object GetPropertyValue(UiaCore.UIA propertyId)
            {
                switch (propertyId)
                {
                    case UiaCore.UIA.NamePropertyId:
                        return SR.DataGridView_AccEditingPanelAccName;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.PaneControlTypeId;
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        return true;
                    case UiaCore.UIA.HasKeyboardFocusPropertyId:
                        return _dataGridView.CurrentCell != null;
                    case UiaCore.UIA.IsEnabledPropertyId:
                        return _dataGridView.Enabled;
                    case UiaCore.UIA.IsOffscreenPropertyId:
                        return false;
                    case UiaCore.UIA.IsControlElementPropertyId:
                    case UiaCore.UIA.IsContentElementPropertyId:
                        return true;
                    case UiaCore.UIA.IsPasswordPropertyId:
                        return false;
                    case UiaCore.UIA.AccessKeyPropertyId:
                        return _panel.AccessibilityObject.KeyboardShortcut;
                    case UiaCore.UIA.HelpTextPropertyId:
                        return string.Empty;
                    case UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId:
                        return true;
                    case UiaCore.UIA.ProviderDescriptionPropertyId:
                        return SR.DataGridViewEditingPanelUiaProviderDescription;
                }

                return base.GetPropertyValue(propertyId);
            }

            #endregion
        }
    }
}
