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
            private readonly DataGridView dataGridView;
            private readonly Panel panel;

            public DataGridViewEditingPanelAccessibleObject(DataGridView dataGridView, Panel panel) : base(panel)
            {
                this.dataGridView = dataGridView;
                this.panel = panel;
            }

            #region IRawElementProviderFragment Implementation

            internal override Rectangle BoundingRectangle
            {
                get
                {
                    return panel.AccessibilityObject.Bounds;
                }
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return dataGridView.AccessibilityObject;
                }
            }

            internal override int[] RuntimeId
            {
                get
                {
                    return panel.AccessibilityObject.RuntimeId;
                }
            }

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        DataGridViewCell currentCell = dataGridView.CurrentCell;
                        if (currentCell is not null && dataGridView.IsCurrentCellInEditMode)
                        {
                            return currentCell.AccessibilityObject;
                        }
                        break;
                    case UiaCore.NavigateDirection.FirstChild:
                    case UiaCore.NavigateDirection.LastChild:
                        return dataGridView.EditingControlAccessibleObject;
                }

                return null;
            }

            internal override void SetFocus()
            {
                if (panel.CanFocus)
                {
                    panel.Focus();
                }
            }

            #endregion

            #region IRawElementProviderSimple Implementation

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                return patternId.Equals(UiaCore.UIA.LegacyIAccessiblePatternId);
            }

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
                        return dataGridView.CurrentCell is not null;
                    case UiaCore.UIA.IsEnabledPropertyId:
                        return dataGridView.Enabled;
                    case UiaCore.UIA.IsOffscreenPropertyId:
                        return false;
                    case UiaCore.UIA.IsControlElementPropertyId:
                    case UiaCore.UIA.IsContentElementPropertyId:
                        return true;
                    case UiaCore.UIA.IsPasswordPropertyId:
                        return false;
                    case UiaCore.UIA.AccessKeyPropertyId:
                        return panel.AccessibilityObject.KeyboardShortcut;
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
