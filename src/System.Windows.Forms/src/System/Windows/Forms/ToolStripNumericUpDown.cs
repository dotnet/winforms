// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.System.Windows.Forms
{
    internal class ToolStripNumericUpDown : ToolStripControlHost
    {
        public ToolStripNumericUpDown() : base(CreateControlInstance())
        {
            ToolStripNumericUpDownControl numericUpDown = Control as ToolStripNumericUpDownControl;
            numericUpDown.Owner = this;
        }

        private static Control CreateControlInstance()
        {
            return new ToolStripNumericUpDownControl();
        }

        internal class ToolStripNumericUpDownControl : NumericUpDown
        {
            public ToolStrip ParentToolStrip { get; private set; }

            public ToolStripNumericUpDown Owner { get; set; }

            internal override bool SupportsUiaProviders => true;

            private class ToolStripNumericUpDownAccessibleObject : ToolStripHostedControlAccessibleObject
            {
                private ToolStripNumericUpDown _toolStripNumericUpDown;

                public ToolStripNumericUpDownAccessibleObject(Control toolStripHostedControl, ToolStripControlHost toolStripControlHost) : base(toolStripHostedControl, toolStripControlHost)
                {
                }

                internal override object GetPropertyValue(int propertyID)
                {
                    if (propertyID == NativeMethods.UIA_ControlTypePropertyId)
                    {
                        return NativeMethods.UIA_SpinnerControlTypeId;
                    }

                    return base.GetPropertyValue(propertyID);
                }

                internal override bool IsPatternSupported(int patternId)
                {
                    if (patternId == NativeMethods.UIA_ValuePatternId)
                    {
                        return true;
                    }

                    return base.IsPatternSupported(patternId);
                }
            }
        }
    }
}
