// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class ToolStripComboBox_ToolStripComboBoxAccessibleObjectTests
{
    [WinFormsFact]
    public void ToolStripComboBoxAccessibleObject_ReleaseUiaProvider_ToolStripComboBoxControl()
    {
        using ToolStrip toolStrip = new();
        using ToolStripComboBox toolStripComboBox = new();
        toolStrip.Items.Add(toolStripComboBox);
        toolStripComboBox.Parent = toolStrip;
        toolStrip.CreateControl();

        _ = toolStripComboBox.AccessibilityObject;
        _ = toolStripComboBox.Control.AccessibilityObject;

        Assert.True(toolStripComboBox.Control.IsAccessibilityObjectCreated);

        toolStripComboBox.ReleaseUiaProvider();

        Assert.False(toolStripComboBox.Control.IsAccessibilityObjectCreated);
    }
}
