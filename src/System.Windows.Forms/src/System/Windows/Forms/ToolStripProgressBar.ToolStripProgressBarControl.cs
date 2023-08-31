// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ToolStripProgressBar
{
    internal class ToolStripProgressBarControl : ProgressBar
    {
        private ToolStripProgressBar? _ownerItem;

        public ToolStripProgressBar? Owner
        {
            get { return _ownerItem; }
            set { _ownerItem = value; }
        }

        internal override bool SupportsUiaProviders => true;

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ToolStripProgressBarControlAccessibleObject(this);
        }
    }
}
