// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    internal class ToolStripNumericUpDown : ToolStripControlHost
    {
        private ToolStripNumericUpDownControl _numericUpDownControl;

        public ToolStripNumericUpDown() : base(CreateControlInstance())
        {
            _numericUpDownControl = Control as ToolStripNumericUpDownControl;
            _numericUpDownControl.Owner = this;
        }

        public NumericUpDown NumericUpDownControl
        {
            get => _numericUpDownControl;
        }

        private static Control CreateControlInstance() => new ToolStripNumericUpDownControl();

        internal class ToolStripNumericUpDownControl : NumericUpDown
        {
            public ToolStrip ParentToolStrip { get; private set; }

            public ToolStripNumericUpDown Owner { get; set; }

            internal override bool SupportsUiaProviders => true;

            protected override AccessibleObject CreateAccessibilityInstance() => new ToolStripNumericUpDownAccessibleObject(this, Owner);

            private class ToolStripNumericUpDownAccessibleObject : ToolStripHostedControlAccessibleObject
            {
                public ToolStripNumericUpDownAccessibleObject(Control toolStripHostedControl, ToolStripControlHost toolStripControlHost) : base(toolStripHostedControl, toolStripControlHost)
                {
                }

                internal override object GetPropertyValue(int propertyID)
                {
                    if (propertyID == NativeMethods.UIA_NamePropertyId)
                    {
                        return Name;
                    }

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
