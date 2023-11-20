// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.ButtonInternal;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

public partial class ToolStripItem
{
    internal partial class ToolStripItemInternalLayout
    {
        internal class ToolStripItemLayoutOptions : ButtonBaseAdapter.LayoutOptions
        {
            private Size _cachedSize = LayoutUtils.s_invalidSize;
            private Size _cachedProposedConstraints = LayoutUtils.s_invalidSize;

            // override GetTextSize to provide simple text caching.
            protected override Size GetTextSize(Size proposedConstraints)
            {
                if (_cachedSize != LayoutUtils.s_invalidSize
                    && (_cachedProposedConstraints == proposedConstraints
                    || _cachedSize.Width <= proposedConstraints.Width))
                {
                    return _cachedSize;
                }

                _cachedSize = base.GetTextSize(proposedConstraints);
                _cachedProposedConstraints = proposedConstraints;
                return _cachedSize;
            }
        }
    }
}
