// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal partial class ToolStripNumericUpDown
{
    internal partial class ToolStripNumericUpDownControl : NumericUpDown
    {
        public ToolStrip? ParentToolStrip { get; private set; }

        public ToolStripNumericUpDown? Owner { get; set; }

        internal override bool SupportsUiaProviders => true;

        protected override AccessibleObject CreateAccessibilityInstance() => new ToolStripNumericUpDownAccessibleObject(this, Owner);
    }
}
