// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class TabControlTest : Form
    {
        public TabControlTest()
        {
            InitializeComponent();
        }

        int i = 0;

        private void ButtonClick(object sender, EventArgs e)
        {
            switch (i)
            {
                case 0:
                    toolTip.SetToolTip(tabPage1, "tabpage1");
                    break;
                case 1:
                    toolTip.SetToolTip(tabPage2, "tabpage2");
                    break;
                case 2:
                    tabPage1.ToolTipText = "page 1 - new text";
                    break;
                case 3:
                    tabPage2.ToolTipText = "new text for page 2";
                    break;
                case 4:
                    toolTip.SetToolTip(tabControl1, "TabControl tooltip");
                    break;
            }

            i++;
        }
    }
}
