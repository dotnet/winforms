// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ToolStripSplitButton
{
    public class ToolStripSplitButtonAccessibleObject : ToolStripItemAccessibleObject
    {
        private readonly ToolStripSplitButton _owningToolStripSplitButton;

        public ToolStripSplitButtonAccessibleObject(ToolStripSplitButton item) : base(item)
        {
            _owningToolStripSplitButton = item;
        }

        public override void DoDefaultAction()
        {
            _owningToolStripSplitButton.PerformButtonClick();
        }
    }
}
