﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;

namespace Accessibility_Core_App
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            CommonControl1 commonControl1 = new CommonControl1();
            commonControl1.Show();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            CommonControl2 commonControl2 = new CommonControl2();
            commonControl2.Show();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            DataControls dataControls = new DataControls();
            dataControls.Show();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            DialogControls dialogsTesting = new DialogControls();
            dialogsTesting.Show();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            MenuForm menuForm = new MenuForm();
            menuForm.Show();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            PrintingControls printingTesting = new PrintingControls();
            printingTesting.Show();
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            RemainingControls remainingControls = new RemainingControls();
            remainingControls.Show();
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            ContainerControls containerControl = new ContainerControls();
            containerControl.Show();
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ContainerControls2 containerControl2 = new ContainerControls2();
            containerControl2.Show();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            TaskDialogTesting taskDialogTesting = new();
            taskDialogTesting.ShowEventsDemoTaskDialog();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            CustomAccessiblePropertiesForm cusAcccName = new();
            cusAcccName.StartPosition = FormStartPosition.CenterParent;
            cusAcccName.Show();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            DataBindingExample dataBindingExample = new();
            dataBindingExample.Show();
        }
    }
}
