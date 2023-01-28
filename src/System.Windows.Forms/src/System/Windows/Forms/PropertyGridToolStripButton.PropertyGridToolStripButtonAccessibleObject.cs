// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    internal partial class PropertyGridToolStripButton
    {
        internal class PropertyGridToolStripButtonAccessibleObject : ToolStripButtonAccessibleObject
        {
            private readonly PropertyGrid? _owningPropertyGrid;
            private readonly PropertyGridToolStripButton _owningPropertyGridToolStripButton;

            public PropertyGridToolStripButtonAccessibleObject(PropertyGridToolStripButton owningToolStripButton) : base(owningToolStripButton)
            {
                _owningPropertyGridToolStripButton = owningToolStripButton;
                _owningPropertyGrid = owningToolStripButton._owningPropertyGrid;
            }

            internal override bool IsItemSelected
                => _owningPropertyGridToolStripButton.Checked;

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.SelectionItemPatternId => _owningPropertyGridToolStripButton._selectItemEnabled,
                    _ => base.IsPatternSupported(patternId)
                };

            internal override void AddToSelection() => SelectItem();

            internal override void Invoke() => SelectItem();

            internal override void RemoveFromSelection()
            {
            }

            internal override unsafe void SelectItem()
            {
                if (_owningPropertyGrid is null || !_owningPropertyGrid.IsHandleCreated)
                {
                    return;
                }

                bool initialState = _owningPropertyGridToolStripButton.Checked;
                _owningPropertyGridToolStripButton.PerformClick();

                // This code is required to simulate the behavior in 4.7.1. When we call "Toggle" method on an already
                // checked button, the focus switches to the ToolStrip. If the button was not checked before the call,
                // then the focus is switched to the table with properties.
                AccessibleObject? focusedAccessibleObject = initialState
                    ? _owningPropertyGridToolStripButton.Parent?.AccessibilityObject
                    : _owningPropertyGridToolStripButton._owningPropertyGrid.GridViewAccessibleObject.GetFocused();

                focusedAccessibleObject?.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
            }
        }
    }
}
