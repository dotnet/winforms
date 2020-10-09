// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms
{
    internal partial class ToolStripNumericUpDown
    {
        internal partial class ToolStripNumericUpDownControl
        {
            private class ToolStripNumericUpDownAccessibleObject : ToolStripHostedControlAccessibleObject
            {
                public ToolStripNumericUpDownAccessibleObject(Control toolStripHostedControl, ToolStripControlHost toolStripControlHost) : base(toolStripHostedControl, toolStripControlHost)
                {
                }

                internal override object GetPropertyValue(UiaCore.UIA propertyID)
                {
                    if (propertyID == UiaCore.UIA.NamePropertyId)
                    {
                        return Name;
                    }

                    if (propertyID == UiaCore.UIA.ControlTypePropertyId)
                    {
                        return UiaCore.UIA.SpinnerControlTypeId;
                    }

                    return base.GetPropertyValue(propertyID);
                }

                internal override bool IsPatternSupported(UiaCore.UIA patternId)
                {
                    if (patternId == UiaCore.UIA.ValuePatternId)
                    {
                        return true;
                    }

                    return base.IsPatternSupported(patternId);
                }
            }
        }
    }
}
