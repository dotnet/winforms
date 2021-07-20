// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace System.ComponentModel.Design
{
    internal sealed partial class DesignerActionPanel
    {
        private sealed class SeparatorLine : Line
        {
            private readonly bool _isSubSeparator;
            public SeparatorLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel) : this(serviceProvider, actionPanel, false)
            {
            }

            public SeparatorLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel, bool isSubSeparator) : base(serviceProvider, actionPanel)
            {
                _isSubSeparator = isSubSeparator;
            }

            public sealed override string FocusId
            {
                get => string.Empty;
            }

            public bool IsSubSeparator => _isSubSeparator;

            protected override void AddControls(List<Control> controls)
            {
            }

            public sealed override void Focus() => Debug.Fail("Should never try to focus a SeparatorLine");

            public override Size LayoutControls(int top, int width, bool measureOnly) => new Size(MinimumWidth, 1);

            public override void PaintLine(Graphics g, int lineWidth, int lineHeight)
            {
                using (Pen p = new Pen(ActionPanel.SeparatorColor))
                {
                    g.DrawLine(p, SeparatorHorizontalPadding, 0, lineWidth - (SeparatorHorizontalPadding + 1), 0);
                }
            }

            internal override void UpdateActionItem(DesignerActionList actionList, DesignerActionItem actionItem, ToolTip toolTip, ref int currentTabIndex)
            {
            }
        }
    }
}
