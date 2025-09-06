// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.ButtonInternal;

namespace System.Windows.Forms.Tests;

public class ButtonFlatAdapterTests : IDisposable
{
    public TestButtonBase buttonBase { get; }
    internal ButtonFlatAdapter buttonFlatAdapter { get; }

    public ButtonFlatAdapterTests()
    {
        buttonBase = new TestButtonBase();
        buttonFlatAdapter = new(buttonBase);
    }

    public void Dispose() => buttonBase.Dispose();

    public class TestButtonBase : ButtonBase
    {
        public Rectangle ClientRectangleValue { get; set; } = new(0, 0, 100, 30);

        internal override Rectangle OverChangeRectangle => ClientRectangleValue;

        internal override ButtonBaseAdapter CreateStandardAdapter() => new ButtonFlatAdapter(this);

        internal override StringFormat CreateStringFormat() => new();

        internal override TextFormatFlags CreateTextFormatFlags() => TextFormatFlags.Default;
    }

    public static TheoryData<CheckState> PaintStates => new()
    {
        CheckState.Unchecked,
        CheckState.Checked,
        CheckState.Indeterminate
    };

    [WinFormsTheory]
    [MemberData(nameof(PaintStates))]
    public void PaintUp_DoesNotThrow(CheckState state)
    {
        using Bitmap bitmap = new(100, 30);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, new Rectangle(0, 0, 100, 30));

        Exception? exception = Record.Exception(() => buttonFlatAdapter.PaintUp(e, state));
        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(PaintStates))]
    public void PaintDown_DoesNotThrow(CheckState state)
    {
        using Bitmap bitmap = new(100, 30);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, new Rectangle(0, 0, 100, 30));

        Exception? exception = Record.Exception(() => buttonFlatAdapter.PaintDown(e, state));
        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(PaintStates))]
    public void PaintOver_DoesNotThrow(CheckState state)
    {
        using Bitmap bitmap = new(100, 30);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, new Rectangle(0, 0, 100, 30));

        Exception? exception = Record.Exception(() => buttonFlatAdapter.PaintOver(e, state));
        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(PaintStates))]
    public void PaintUp_WithCustomBorder_DoesNotThrow(CheckState state)
    {
        using TestButtonBase button = new();
        button.FlatAppearance.BorderSize = 2;
        button.FlatAppearance.BorderColor = Color.Red;
        ButtonFlatAdapter buttonFlatAdapter = new(button);

        using Bitmap bitmap = new(100, 30);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, new Rectangle(0, 0, 100, 30));

        Exception? exception = Record.Exception(() => buttonFlatAdapter.PaintUp(e, state));
        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(PaintStates))]
    public void PaintDown_WithMouseDownBackColor_DoesNotThrow(CheckState state)
    {
        using TestButtonBase button = new();
        button.FlatAppearance.MouseDownBackColor = Color.Blue;
        ButtonFlatAdapter buttonFlatAdapter = new(button);

        using Bitmap bitmap = new(100, 30);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, new Rectangle(0, 0, 100, 30));

        Exception? exception = Record.Exception(() => buttonFlatAdapter.PaintDown(e, state));
        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(PaintStates))]
    public void PaintOver_WithMouseOverBackColor_DoesNotThrow(CheckState state)
    {
        using TestButtonBase button = new();
        button.FlatAppearance.MouseOverBackColor = Color.Green;
        ButtonFlatAdapter buttonFlatAdapter = new(button);

        using Bitmap bitmap = new(100, 30);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, new Rectangle(0, 0, 100, 30));

        Exception? exception = Record.Exception(() => buttonFlatAdapter.PaintOver(e, state));
        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(PaintStates))]
    public void PaintUp_WithCheckedBackColor_DoesNotThrow(CheckState state)
    {
        using TestButtonBase button = new();
        button.FlatAppearance.CheckedBackColor = Color.Yellow;
        ButtonFlatAdapter buttonFlatAdapter = new(button);

        using Bitmap bitmap = new(100, 30);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, new Rectangle(0, 0, 100, 30));

        Exception? exception = Record.Exception(() => buttonFlatAdapter.PaintUp(e, state));
        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [BoolData]
    public void PaintUp_WithIsDefault_DoesNotThrow(bool isDefault)
    {
        using TestButtonBase button = new();
        button.IsDefault = isDefault;
        ButtonFlatAdapter buttonFlatAdapter = new(button);

        using Bitmap bitmap = new(100, 30);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, new Rectangle(0, 0, 100, 30));

        Exception? exception = Record.Exception(() => buttonFlatAdapter.PaintUp(e, CheckState.Unchecked));
        exception.Should().BeNull();
    }

    [Fact]
    public void PaintFlatLayout_SetsProperties_Correctly_WhenUpIsFalse()
    {
        Rectangle clientRectangle = new(0, 0, 50, 20);
        Padding padding = new(1, 2, 3, 4);
        using Font font = new("Segoe UI", 9);
        string text = "CombinedTest";
        bool enabled = true;
        ContentAlignment textAlign = ContentAlignment.TopLeft;
        RightToLeft rightToLeft = RightToLeft.No;

        ButtonBaseAdapter.LayoutOptions layoutOptionsTrue = ButtonFlatAdapter.PaintFlatLayout(
            up: false,
            check: true,
            borderSize: 2,
            clientRectangle,
            padding,
            isDefault: false,
            font,
            text,
            enabled,
            textAlign,
            rightToLeft);

        ButtonBaseAdapter.LayoutOptions layoutOptionsFalse = ButtonFlatAdapter.PaintFlatLayout(
            up: false,
            check: false,
            borderSize: 2,
            clientRectangle,
            padding,
            isDefault: false,
            font,
            text,
            enabled,
            textAlign,
            rightToLeft);

        layoutOptionsTrue.PaddingSize.Should().Be(1);
        layoutOptionsFalse.PaddingSize.Should().Be(2);

        layoutOptionsTrue.FocusOddEvenFixup.Should().BeFalse();
        layoutOptionsFalse.FocusOddEvenFixup.Should().BeFalse();

        layoutOptionsTrue.BorderSize.Should().Be(3);
        layoutOptionsFalse.BorderSize.Should().Be(2);
    }

    [Theory]
    [BoolData]
    public void PaintFlatLayout_SetsTextOffset_Correctly(bool up)
    {
        Rectangle clientRectangle = new(0, 0, 50, 20);
        Padding padding = new(1, 2, 3, 4);
        using Font font = new("Segoe UI", 9);
        string text = "TextOffsetTest";
        bool enabled = true;
        ContentAlignment textAlign = ContentAlignment.TopLeft;
        RightToLeft rightToLeft = RightToLeft.No;

        ButtonBaseAdapter.LayoutOptions layoutOptions = ButtonFlatAdapter.PaintFlatLayout(
            up,
            check: false,
            borderSize: 2,
            clientRectangle,
            padding,
            isDefault: false,
            font,
            text,
            enabled,
            textAlign,
            rightToLeft);

        layoutOptions.TextOffset.Should().Be(!up);
    }
}
