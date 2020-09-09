// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripTextBox
    {
        private class ToolStripTextBoxControlAccessibleObject : ToolStripHostedControlAccessibleObject
        {
            public ToolStripTextBoxControlAccessibleObject(Control toolStripHostedControl, ToolStripControlHost toolStripControlHost) : base(toolStripHostedControl, toolStripControlHost)
            {
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.EditControlTypeId;
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
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
