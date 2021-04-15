// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.ComponentModel.Design
{
    internal sealed partial class DesignerActionPanel
    {
        private sealed class HeaderLine : TextLine
        {
            public HeaderLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel) : base(serviceProvider, actionPanel)
            {
            }

            protected override Font GetFont() => new Font(ActionPanel.Font, FontStyle.Bold);
        }
    }
}
