// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    internal partial class ToolStripNumericUpDown : ToolStripControlHost
    {
        private ToolStripNumericUpDownControl _numericUpDownControl;

        public ToolStripNumericUpDown()
            : base(CreateControlInstance())
        {
            _numericUpDownControl = (ToolStripNumericUpDownControl)Control;
            _numericUpDownControl.Owner = this;
        }

        public NumericUpDown NumericUpDownControl
        {
            get => _numericUpDownControl;
        }

        private static Control CreateControlInstance() => new ToolStripNumericUpDownControl();
    }
}
