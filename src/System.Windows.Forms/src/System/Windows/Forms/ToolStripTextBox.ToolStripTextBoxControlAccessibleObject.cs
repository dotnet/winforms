// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static System.Windows.Forms.TextBoxBase;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripTextBox
    {
        private class ToolStripTextBoxControlAccessibleObject : TextBoxBaseAccessibleObject
        {
            public ToolStripTextBoxControlAccessibleObject(TextBox toolStripHostedControl) : base(toolStripHostedControl)
            { }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    // If we don't set a default role for the accessible object
                    // it will be retrieved from Windows.
                    // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                    UiaCore.UIA.ControlTypePropertyId => Owner.AccessibleRole == AccessibleRole.Default
                                                         ? UiaCore.UIA.EditControlTypeId
                                                         : base.GetPropertyValue(propertyID),
                    UiaCore.UIA.HasKeyboardFocusPropertyId
                        => (State & AccessibleStates.Focused) == AccessibleStates.Focused,
                    UiaCore.UIA.IsOffscreenPropertyId
                        => GetIsOffscreenPropertyValue(Owner.ToolStripControlHost?.Placement, Bounds),
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId) =>
                patternId switch
                {
                    UiaCore.UIA.ValuePatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };
        }
    }
}
