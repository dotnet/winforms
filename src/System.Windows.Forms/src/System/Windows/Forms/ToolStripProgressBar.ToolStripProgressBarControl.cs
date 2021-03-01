// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public partial class ToolStripProgressBar
    {
        internal class ToolStripProgressBarControl : ProgressBar
        {
            private ToolStripProgressBar ownerItem;

            public ToolStripProgressBar Owner
            {
                get { return ownerItem; }
                set { ownerItem = value; }
            }

            internal override bool SupportsUiaProviders => true;

            protected override AccessibleObject CreateAccessibilityInstance()
            {
                return new ToolStripProgressBarControlAccessibleObject(this);
            }
        }
    }
}
