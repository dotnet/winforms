﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

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

            internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    // If we don't set a default role for the accessible object
                    // it will be retrieved from Windows.
                    // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                    UiaCore.UIA.ControlTypePropertyId when this.GetOwnerAccessibleRole() == AccessibleRole.Default
                        => UiaCore.UIA.SpinnerControlTypeId,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId == UiaCore.UIA.ValuePatternId || base.IsPatternSupported(patternId);
        }
    }
}
