// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.ComponentModel.Design;

internal sealed partial class DesignerActionPanel
{
    private sealed class HeaderLine : TextLine
    {
        private HeaderLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel) : base(serviceProvider, actionPanel)
        {
        }

        protected override Font GetFont() => new(ActionPanel.Font, FontStyle.Bold);

        public new sealed class Info(DesignerActionList list, DesignerActionItem item) : LineInfo(list, item)
        {
            public override Line CreateLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel)
            {
                return new HeaderLine(serviceProvider, actionPanel);
            }

            public override Type LineType => typeof(HeaderLine);
        }
    }
}
