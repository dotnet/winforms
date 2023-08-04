﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinformsControlsTest;

public partial class ScrollBars : Form
{
    public ScrollBars()
    {
        InitializeComponent();
        SetValueLabels();
    }

    private void numericMaximum_ValueChanged(object sender, EventArgs e)
    {
        hScrollBar.Maximum = Decimal.ToInt32(numericMaximum.Value);
        vScrollBar.Maximum = Decimal.ToInt32(numericMaximum.Value);
        numericMinimum.Maximum = numericMaximum.Value;
        SetValueLabels();
    }

    private void numericMinimum_ValueChanged(object sender, EventArgs e)
    {
        hScrollBar.Minimum = Decimal.ToInt32(numericMinimum.Value);
        vScrollBar.Minimum = Decimal.ToInt32(numericMinimum.Value);
        numericMaximum.Minimum = numericMinimum.Value;
        SetValueLabels();
    }

    private void hScrollBar_Scroll(object sender, ScrollEventArgs e) => SetValueLabels();

    private void vScrollBar_Scroll(object sender, ScrollEventArgs e) => SetValueLabels();

    private void SetValueLabels()
    {
        lblHValue.Text = $"Value: {hScrollBar.Value}";
        lblVValue.Text = $"Value: {vScrollBar.Value}";
    }

    private void chbRightToLeft_CheckedChanged(object sender, EventArgs e)
    {
        hScrollBar.RightToLeft = chbRightToLeft.Checked ? RightToLeft.Yes : RightToLeft.No;
        SetValueLabels();
    }
}
