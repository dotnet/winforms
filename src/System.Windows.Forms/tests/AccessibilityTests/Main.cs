// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace Accessibility_Core_App;

public partial class Main : Form
{
    public Main()
    {
        InitializeComponent();
    }

    private void Button1_Click(object sender, EventArgs e)
    {
        CommonControl1 commonControl1 = new();
        commonControl1.Show();
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
        DialogControls dialogsTesting = new();
        dialogsTesting.Show();
    }

    private void Button5_Click(object sender, EventArgs e)
    {
        MenuForm menuForm = new();
        menuForm.Show();
    }

    private void Button6_Click(object sender, EventArgs e)
    {
        PrintingControls printingTesting = new();
        printingTesting.Show();
    }

    private void Button7_Click(object sender, EventArgs e)
    {
        RemainingControls remainingControls = new();
        remainingControls.Show();
    }

    private void Button8_Click(object sender, EventArgs e)
    {
        ContainerControls containerControl = new();
        containerControl.Show();
    }

    private void button9_Click(object sender, EventArgs e)
    {
        ContainerControls2 containerControl2 = new();
        containerControl2.Show();
    }

    private void button10_Click(object sender, EventArgs e)
    {
        TaskDialogTesting taskDialogTesting = new();
        taskDialogTesting.ShowEventsDemoTaskDialog();
    }

    private void button12_Click(object sender, EventArgs e)
    {
        DataBindingExample dataBindingExample = new();
        dataBindingExample.Show();
    }
}
