// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ToolStripLabel
{
    /// <summary>
    ///  This class performs internal layout for the "split button button" portion of a split button.
    ///  Its main job is to make sure the inner button has the same parent as the split button, so
    ///  that layout can be performed using the correct graphics context.
    /// </summary>
    private class ToolStripLabelLayout : ToolStripItemInternalLayout
    {
        public ToolStripLabelLayout(ToolStripLabel owner)
            : base(owner)
        {
        }

        protected override ToolStripItemLayoutOptions CommonLayoutOptions()
        {
            ToolStripItemLayoutOptions layoutOptions = base.CommonLayoutOptions();
            layoutOptions.BorderSize = 0;
            return layoutOptions;
        }
    }
}
