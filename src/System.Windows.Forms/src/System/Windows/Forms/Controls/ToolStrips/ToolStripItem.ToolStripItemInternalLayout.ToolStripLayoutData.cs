// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class ToolStripItem
{
    internal partial class ToolStripItemInternalLayout
    {
        private class ToolStripLayoutData
        {
            private readonly ToolStripLayoutStyle _layoutStyle;
            private readonly bool _autoSize;
            private Size _size;

            public ToolStripLayoutData(ToolStrip toolStrip)
            {
                _layoutStyle = toolStrip.LayoutStyle;
                _autoSize = toolStrip.AutoSize;
                _size = toolStrip.Size;
            }

            public bool IsCurrent(ToolStrip? toolStrip)
                => toolStrip is not null && toolStrip.Size == _size && toolStrip.LayoutStyle == _layoutStyle && toolStrip.AutoSize == _autoSize;
        }
    }
}
