// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class TabControlTest : Form
    {
        public TabControlTest()
        {
            InitializeComponent();
            //toolTip.SetToolTip(tabControl1, "TabControl");
        }

        int i = 0;

        private void ButtonClick(object sender, EventArgs e)
        {
            // It will be removed after testing
            switch (i)
            {
                case 0:
                    toolTip.SetToolTip(tabPage2, "This is page 1");
                    break;
                case 1:
                    toolTip.SetToolTip(tabPage2, "This is page 1");
                    break;
                case 2:
                    tabPage2.ToolTipText = "1) Some tt text";
                    break;
                case 3:
                    tabPage2.ToolTipText = "1) Some tt text";
                    break;
            }

            toolTip.SetToolTip(tabPage1, " \t  \r\n   ");

            i++;
        }

        int j = 0;

        private void Button2Click(object sender, EventArgs e)
        {
            new ToolTip().SetToolTip(internalButton, j.ToString());
            j++;

            new ToolTip().SetToolTip(_label, "");


        }

        private void Button3Click(object sender, EventArgs e)
        {
            new ToolTip().SetToolTip(_label, "New tooltip text");
        }
    }

    public class MyLabel : Label
    {
        public MyLabel()
        {
            this.SetToolTip(new ToolTip(), "balas");
            //new ToolTip().SetToolTip(this, "");
        }
    }
}
