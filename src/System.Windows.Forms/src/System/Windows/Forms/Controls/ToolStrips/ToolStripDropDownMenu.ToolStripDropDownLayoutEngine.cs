// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

public partial class ToolStripDropDownMenu
{
    internal sealed class ToolStripDropDownLayoutEngine : FlowLayout
    {
        public static ToolStripDropDownLayoutEngine LayoutInstance = new();

        internal override Size GetPreferredSize(IArrangedElement container, Size proposedConstraints)
        {
            Size preferredSize = base.GetPreferredSize(container, proposedConstraints);
            if (container is ToolStripDropDownMenu dropDownMenu)
            {
                preferredSize.Width = dropDownMenu.MaxItemSize.Width - dropDownMenu.PaddingToTrim;
            }

            return preferredSize;
        }
    }
}
