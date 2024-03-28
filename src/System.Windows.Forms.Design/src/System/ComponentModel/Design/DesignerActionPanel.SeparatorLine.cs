// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms;

namespace System.ComponentModel.Design;

internal sealed partial class DesignerActionPanel
{
    private sealed class SeparatorLine : Line
    {
        private SeparatorLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel, bool isSubSeparator) : base(serviceProvider, actionPanel)
        {
            IsSubSeparator = isSubSeparator;
        }

        public override string FocusId => string.Empty;

        public bool IsSubSeparator { get; }

        public sealed override void Focus() => Debug.Fail("Should never try to focus a SeparatorLine");

        public override Size LayoutControls(int top, int width, bool measureOnly) => new(MinimumWidth, 1);

        public override void PaintLine(Graphics g, int lineWidth, int lineHeight)
        {
            using Pen p = new(ActionPanel.SeparatorColor);
            g.DrawLine(p, SeparatorHorizontalPadding, 0, lineWidth - (SeparatorHorizontalPadding + 1), 0);
        }

        internal override void UpdateActionItem(LineInfo lineInfo, ToolTip toolTip, ref int currentTabIndex)
        {
        }

        public sealed class Info(bool isSubSeparator = false) : LineInfo
        {
            private readonly bool _isSubSeparator = isSubSeparator;
            public override DesignerActionItem? Item => null;
            public override Line CreateLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel)
            {
                return new SeparatorLine(serviceProvider, actionPanel, _isSubSeparator);
            }

            public override Type LineType => typeof(SeparatorLine);
        }
    }
}
