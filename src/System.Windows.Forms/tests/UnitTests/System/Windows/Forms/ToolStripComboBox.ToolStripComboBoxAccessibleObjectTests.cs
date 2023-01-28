﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripComboBox_ToolStripComboBoxAccessibleObjectTests
    {
        [WinFormsFact]
        public void ToolStripComboBoxAccessibleObject_ReleaseUiaProvider_ToolStripComboBoxControl()
        {
            using var toolStrip = new ToolStrip();
            using var toolStripComboBox = new ToolStripComboBox();
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
}
