// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal partial class ToolStripNumericUpDown : ToolStripControlHost
{
    private readonly ToolStripNumericUpDownControl _numericUpDownControl;

    public ToolStripNumericUpDown() : base(CreateControlInstance())
    {
        _numericUpDownControl = (ToolStripNumericUpDownControl)Control;
        _numericUpDownControl.Owner = this;
    }

    public NumericUpDown NumericUpDownControl => _numericUpDownControl;

    private static ToolStripNumericUpDownControl CreateControlInstance() => new();
}
