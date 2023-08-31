// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace System.Drawing.Design;

public partial class ColorEditor
{
    private sealed partial class ColorUI
    {
        private sealed class ColorEditorTabControl : TabControl
        {
            public ColorEditorTabControl() : base()
            {
            }

            protected override void OnGotFocus(EventArgs e)
            {
                TabPage? selectedTab = SelectedTab;
                if (selectedTab is not null && selectedTab.Controls.Count > 0)
                {
                    selectedTab.Controls[0].Focus();
                }
            }
        }
    }
}
