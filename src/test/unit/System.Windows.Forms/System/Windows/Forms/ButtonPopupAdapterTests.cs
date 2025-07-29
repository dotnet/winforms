// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.ButtonInternal;
using static System.Windows.Forms.ButtonInternal.ButtonBaseAdapter;

namespace System.Windows.Forms.Tests;

public class ButtonPopupAdapterTests : IDisposable
{
    public ButtonBase button { get; }
    internal ButtonPopupAdapter adapter { get; }

    public ButtonPopupAdapterTests()
    {
        button = new TestButtonBase();
        adapter = new(button);
    }

    public void Dispose() => button.Dispose();

    public static TheoryData<CheckState> PaintStates => new()
    {
        CheckState.Unchecked,
        CheckState.Checked,
        CheckState.Indeterminate
    };

    private class TestButtonBase : ButtonBase
    {
        public bool IsDefaultSet { get; set; }
        public Rectangle ClientRectangleValue { get; set; } = new(0, 0, 100, 30);

        internal override Rectangle OverChangeRectangle => ClientRectangleValue;

        internal override ButtonBaseAdapter CreateStandardAdapter() => new ButtonPopupAdapter(this);

        internal override StringFormat CreateStringFormat() => new StringFormat();

        internal override TextFormatFlags CreateTextFormatFlags() => TextFormatFlags.Default;
    }

    [WinFormsTheory]
    [MemberData(nameof(PaintStates))]
    public void PaintUp_DoesNotThrow(CheckState state)
    {
        using Bitmap bitmap = new(100, 30);
        using Graphics graphics = Graphics.FromImage(bitmap);
        PaintEventArgs e = new(graphics, new Rectangle(0, 0, 100, 30));

        Exception? exception = Record.Exception(() => adapter.PaintUp(e, state));

        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(PaintStates))]
    public void PaintOver_DoesNotThrow(CheckState state)
    {
        using Bitmap bitmap = new(100, 30);
        using Graphics graphics = Graphics.FromImage(bitmap);
        PaintEventArgs e = new(graphics, new Rectangle(0, 0, 100, 30));

        Exception? exception = Record.Exception(() => adapter.PaintOver(e, state));

        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [MemberData(nameof(PaintStates))]
    public void PaintDown_DoesNotThrow(CheckState state)
    {
        using Bitmap bitmap = new(100, 30);
        using Graphics graphics = Graphics.FromImage(bitmap);
        PaintEventArgs e = new(graphics, new Rectangle(0, 0, 100, 30));

        Exception? exception = Record.Exception(() => adapter.PaintDown(e, state));

        exception.Should().BeNull();
    }

    [Fact]
    public void PaintPopupLayout_ReturnsLayoutOptions_WithExpectedBorderAndPadding()
    {
        Rectangle clientRectangle = new(0, 0, 100, 30);
        Padding padding = new(2);
        bool isDefault = false;
        using Font font = SystemFonts.DefaultFont;
        string text = "Test";
        bool enabled = true;
        ContentAlignment textAlign = ContentAlignment.MiddleCenter;
        RightToLeft rightToLeft = RightToLeft.No;

        LayoutOptions layoutOptions = ButtonPopupAdapter.PaintPopupLayout(
            up: true,
            paintedBorder: 1,
            clientRectangle,
            padding,
            isDefault,
            font,
            text,
            enabled,
            textAlign,
            rightToLeft);

        layoutOptions.BorderSize.Should().Be(1);
        layoutOptions.PaddingSize.Should().Be(1);
        layoutOptions.HintTextUp.Should().BeFalse();
        layoutOptions.TextOffset.Should().BeFalse();
        layoutOptions.ShadowedText.Should().Be(SystemInformation.HighContrast);
    }

    [Theory]
    [InlineData(true, 0)]
    [InlineData(false, 2)]
    public void PaintPopupLayout_BorderAndPaddingSumIsTwo(bool up, int paintedBorder)
    {
        Rectangle clientRectangle = new(0, 0, 100, 30);
        Padding padding = new(0);
        bool isDefault = false;
        using Font font = SystemFonts.DefaultFont;
        string text = string.Empty;
        bool enabled = false;
        ContentAlignment textAlign = ContentAlignment.TopLeft;
        RightToLeft rightToLeft = RightToLeft.No;

        LayoutOptions layoutOptions = ButtonPopupAdapter.PaintPopupLayout(
            up,
            paintedBorder,
            clientRectangle,
            padding,
            isDefault,
            font,
            text,
            enabled,
            textAlign,
            rightToLeft);

        int sum = layoutOptions.BorderSize + layoutOptions.PaddingSize;

        sum.Should().Be(2);
    }
}
