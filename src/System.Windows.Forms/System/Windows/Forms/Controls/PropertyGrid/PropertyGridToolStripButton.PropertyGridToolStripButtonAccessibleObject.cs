// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

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

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_SelectionItemPatternId => _owningPropertyGridToolStripButton._selectItemEnabled,
                UIA_PATTERN_ID.UIA_TogglePatternId => false,
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

            focusedAccessibleObject?.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
        }
    }
}
