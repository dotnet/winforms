// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.ButtonInternal;

namespace System.Windows.Forms.Tests;

public class RadioButtonFlatAdapterTests
{
    [WinFormsFact]
    public void PaintUp_AppearanceButton_CallsButtonFlatAdapterPaintUp()
    {
        using RadioButton control = new RadioButton
        {
            Appearance = Appearance.Button,
            Checked = true
        };

        RadioButtonFlatAdapter adapter = new(control);
        using Bitmap bitmap = new(1, 1);
        using Graphics graphics = Graphics.FromImage(bitmap);
        PaintEventArgs e = new(graphics, control.ClientRectangle);

        adapter.PaintUp(e, CheckState.Checked);

        control.Appearance.Should().Be(Appearance.Button);
        control.Checked.Should().BeTrue();
    }

    [WinFormsFact]
    public void PaintUp_AppearanceNormal_PaintsCorrectly()
    {
        using RadioButton control = new RadioButton
        {
            Appearance = Appearance.Normal,
            Checked = true
        };

        RadioButtonFlatAdapter adapter = new(control);
        using Bitmap bitmap = new(1, 1);
        using Graphics graphics = Graphics.FromImage(bitmap);
        PaintEventArgs e = new(graphics, control.ClientRectangle);

        adapter.PaintUp(e, CheckState.Checked);

        control.Appearance.Should().Be(Appearance.Normal);
        control.Checked.Should().BeTrue();
    }

    [WinFormsFact]
    public void PaintDown_AppearanceButton_CallsButtonFlatAdapterPaintDown()
    {
        using RadioButton control = new RadioButton
        {
            Appearance = Appearance.Button,
            Checked = false
        };

        RadioButtonFlatAdapter adapter = new(control);
        using Bitmap bitmap = new(1, 1);
        using Graphics graphics = Graphics.FromImage(bitmap);
        PaintEventArgs e = new(graphics, control.ClientRectangle);

        adapter.PaintDown(e, CheckState.Unchecked);

        control.Appearance.Should().Be(Appearance.Button);
        control.Checked.Should().BeFalse();
    }

    [WinFormsFact]
    public void PaintDown_AppearanceNormal_PaintsCorrectly()
    {
        using RadioButton control = new RadioButton
        {
            Appearance = Appearance.Normal,
            Checked = false
        };

        RadioButtonFlatAdapter adapter = new(control);
        using Bitmap bitmap = new(1, 1);
        using Graphics graphics = Graphics.FromImage(bitmap);
        PaintEventArgs e = new(graphics, control.ClientRectangle);

        adapter.PaintDown(e, CheckState.Unchecked);

        control.Appearance.Should().Be(Appearance.Normal);
        control.Checked.Should().BeFalse();
    }

    [WinFormsFact]
    public void PaintOver_AppearanceButton_CallsButtonFlatAdapterPaintOver()
    {
        using RadioButton control = new RadioButton
        {
            Appearance = Appearance.Button,
            Checked = true
        };

        RadioButtonFlatAdapter adapter = new(control);
        using Bitmap bitmap = new(1, 1);
        using Graphics graphics = Graphics.FromImage(bitmap);
        PaintEventArgs e = new(graphics, control.ClientRectangle);

        adapter.PaintOver(e, CheckState.Checked);

        control.Appearance.Should().Be(Appearance.Button);
        control.Checked.Should().BeTrue();
    }

    [WinFormsFact]
    public void PaintOver_AppearanceNormal_PaintsCorrectly()
    {
        using RadioButton control = new RadioButton
        {
            Appearance = Appearance.Normal,
            Checked = false
        };

        RadioButtonFlatAdapter adapter = new(control);
        using Bitmap bitmap = new(1, 1);
        using Graphics graphics = Graphics.FromImage(bitmap);
        PaintEventArgs e = new(graphics, control.ClientRectangle);

        adapter.PaintOver(e, CheckState.Unchecked);

        control.Appearance.Should().Be(Appearance.Normal);
        control.Checked.Should().BeFalse();
    }
}
