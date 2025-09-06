// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;
using System.Windows.Forms.ButtonInternal;
using static System.Windows.Forms.ButtonInternal.ButtonBaseAdapter;

namespace System.Windows.Forms.Tests;

public class CheckBoxPopupAdapterTests : AbstractButtonBaseTests
{
    [WinFormsTheory]
    [InlineData(CheckState.Unchecked)]
    [InlineData(CheckState.Checked)]
    [InlineData(CheckState.Indeterminate)]
    public void PaintUp_DoesNotThrow_ForAllCheckStates(CheckState state)
    {
        using CheckBox checkBox = new();
        CheckBoxPopupAdapter adapter = new(checkBox);
        using Bitmap bitmap = new(checkBox.Width, checkBox.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        PaintEventArgs e = new(graphics, checkBox.ClientRectangle);

        Action action = () => adapter.PaintUp(e, state);

        action.Should().NotThrow();
    }

    [WinFormsTheory]
    [InlineData(CheckState.Unchecked)]
    [InlineData(CheckState.Checked)]
    [InlineData(CheckState.Indeterminate)]
    public void PaintOver_DoesNotThrow_ForAllCheckStates(CheckState state)
    {
        using CheckBox checkBox = new();
        checkBox.Text = "Test";
        CheckBoxPopupAdapter adapter = new(checkBox);
        using Bitmap bitmap = new(checkBox.Width, checkBox.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        PaintEventArgs e = new(graphics, checkBox.ClientRectangle);

        Action action = () => adapter.PaintOver(e, state);

        action.Should().NotThrow();
    }

    [WinFormsTheory]
    [InlineData(CheckState.Unchecked)]
    [InlineData(CheckState.Checked)]
    [InlineData(CheckState.Indeterminate)]
    public void PaintDown_DoesNotThrow_ForAllCheckStates(CheckState state)
    {
        using CheckBox checkBox = new();
        checkBox.Text = "Test";
        CheckBoxPopupAdapter adapter = new(checkBox);
        using Bitmap bitmap = new(checkBox.Width, checkBox.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        PaintEventArgs e = new(graphics, checkBox.ClientRectangle);

        Action action = () => adapter.PaintDown(e, state);

        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void Layout_DoesNotThrow_WhenInvokedViaTestAccessor()
    {
        using CheckBox checkBox = new();
        CheckBoxPopupAdapter adapter = new(checkBox);
        using Bitmap bitmap = new(checkBox.Width, checkBox.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        PaintEventArgs e = new(graphics, checkBox.ClientRectangle);

        Action action = () => adapter.TestAccessor().Dynamic.Layout(e);

        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void CreateButtonAdapter_DoesNotThrow_WhenInvokedViaTestAccessor()
    {
        using CheckBox checkBox = new();
        CheckBoxPopupAdapter adapter = new(checkBox);

        Action action = () => adapter.TestAccessor().Dynamic.CreateButtonAdapter();

        action.Should().NotThrow();
    }

    [WinFormsTheory]
    [InlineData(true, 11, 50, 20, 2, false, "First Test", true, ContentAlignment.MiddleLeft, RightToLeft.No)]
    [InlineData(false, 15, 100, 40, 4, true, "Sec Test", false, ContentAlignment.TopRight, RightToLeft.Yes)]
    public void PaintPopupLayout_Static_Properties_AreSet(
        bool show3D,
        int checkSize,
        int width,
        int height,
        int paddingValue,
        bool isDefault,
        string text,
        bool enabled,
        ContentAlignment textAlign,
        RightToLeft rtl)
    {
        Rectangle clientRect = new(0, 0, width, height);
        Padding padding = new(paddingValue);
        using Font font = SystemFonts.DefaultFont;

        LayoutOptions layout = CheckBoxPopupAdapter.PaintPopupLayout(
            show3D,
            checkSize,
            clientRect,
            padding,
            isDefault,
            font,
            text,
            enabled,
            textAlign,
            rtl);

        layout.Should().NotBeNull();
        layout.CheckSize.Should().BeGreaterThan(0);
        layout.ShadowedText.Should().BeFalse();
    }

    protected override ButtonBase CreateButton() => new CheckBox();
}
