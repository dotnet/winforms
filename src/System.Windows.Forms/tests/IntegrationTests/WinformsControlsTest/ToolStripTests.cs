// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class ToolStripTests : Form
    {
        public ToolStripTests()
        {
            InitializeComponent();

            toolStrip1.Items.Add(new ToolStripControlHost(new RadioButton() { Text = "RadioButton" })); // RadioButton supports UIA
            toolStrip1.Items.Add(new ToolStripControlHost(new HScrollBar() { Value = 30 })); // HScrollBar doesn't support UIA
            statusStrip1.Items.Add(new ToolStripControlHost(new RadioButton() { Text = "RadioButton" })); // RadioButton supports UIA
            statusStrip1.Items.Add(new ToolStripControlHost(new HScrollBar() { Value = 30 })); // HScrollBar doesn't support UIA
        }
    }
}
