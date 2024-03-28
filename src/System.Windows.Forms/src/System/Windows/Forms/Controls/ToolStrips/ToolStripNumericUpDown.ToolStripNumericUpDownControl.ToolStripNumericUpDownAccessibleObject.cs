// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

internal partial class ToolStripNumericUpDown
{
    internal partial class ToolStripNumericUpDownControl
    {
        private class ToolStripNumericUpDownAccessibleObject : ToolStripHostedControlAccessibleObject
        {
            public ToolStripNumericUpDownAccessibleObject(Control toolStripHostedControl, ToolStripControlHost? toolStripControlHost)
                : base(toolStripHostedControl, toolStripControlHost)
            {
            }

            internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
                propertyID switch
                {
                    // If we don't set a default role for the accessible object
                    // it will be retrieved from Windows.
                    // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                    UIA_PROPERTY_ID.UIA_ControlTypePropertyId when this.GetOwnerAccessibleRole() == AccessibleRole.Default
                        => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_SpinnerControlTypeId,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
                => patternId == UIA_PATTERN_ID.UIA_ValuePatternId || base.IsPatternSupported(patternId);
        }
    }
}
