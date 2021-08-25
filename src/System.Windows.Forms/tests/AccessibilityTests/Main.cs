// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;

namespace AccessibilityTests
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            Commontrol1 commontrol1 = new();
            commontrol1.Show();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            CommonControl2 commonControl2 = new();
            commonControl2.Show();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            DataControls dataControls = new();
            dataControls.Show();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            TaskDialogTesting TaskDialogTesting = new();
            TaskDialogTesting.ShowEventsDemoTaskDialog();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            MenuForm menuForm = new();
            menuForm.Show();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            PrintingTesting printingTesting = new();
            printingTesting.Show();
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            RemainingControls remainingControls = new();
            remainingControls.Show();
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            ContainersTesting containerControl = new ContainersTesting();
            containerControl.Show();
        }

        private void btnDataBindingExample_Click(object sender, EventArgs e)
        {
            DataBindingExample dataBindingExample = new();
            dataBindingExample.Show();
        }

        private void customAccessiblePropertiesButton_Click(object sender, EventArgs e)
        {
            CustomAccessiblePropertiesForm cusAcccName = new();
            cusAcccName.StartPosition = FormStartPosition.CenterParent;
            cusAcccName.Show();
        }
    }
}
