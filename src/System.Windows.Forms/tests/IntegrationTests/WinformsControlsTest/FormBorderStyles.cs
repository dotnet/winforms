// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FormBorderStyles : Form
    {
        public FormBorderStyles()
        {
            InitializeComponent();
            ShowCurrentBorderStyle();
        }

        private void ShowCurrentBorderStyle()
        {
            lblFormBorderStyle.Text = $"Current border style: {FormBorderStyle}";
        }

        private void btnChangeFormBorderStyle_Click(object sender, EventArgs e)
        {
            var currentBorderStyle = (int)FormBorderStyle;
            currentBorderStyle++;
            if (currentBorderStyle > (int)FormBorderStyle.SizableToolWindow)
            {
                currentBorderStyle = 0;
            }

            FormBorderStyle = (FormBorderStyle)currentBorderStyle;
            ShowCurrentBorderStyle();
        }
    }
}
