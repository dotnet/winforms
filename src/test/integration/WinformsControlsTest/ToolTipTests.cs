// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class ToolTipTests : Form
{
    public ToolTipTests()
    {
        InitializeComponent();

        defaultAutomaticDelayToolTip.AutomaticDelay = 500;
        defaultAutoPopDelayToolTip.AutoPopDelay = 5000;
    }
}
