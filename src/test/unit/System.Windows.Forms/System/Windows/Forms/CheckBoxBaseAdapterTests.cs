// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.ButtonInternal;

namespace System.Windows.Forms.Tests;

public class CheckBoxBaseAdapterTests : IDisposable
{
    private TestCheckBox? _checkBox;

    private (TestCheckBoxBaseAdapter, TestCheckBox) CreateAdapter(CheckState checkState)
    {
        _checkBox = new TestCheckBox
        {
            CheckState = checkState
        };

        TestCheckBoxBaseAdapter adapter = new(_checkBox);

        return (adapter, _checkBox);
    }

    public void Dispose() => _checkBox?.Dispose();

    private class TestCheckBox : CheckBox
    {
        public new bool Enabled
        {
            get => base.Enabled;
            set => base.Enabled = value;
        }

        public new CheckState CheckState
        {
            get => base.CheckState;
            set => base.CheckState = value;
        }
    }

    private class TestCheckBoxBaseAdapter : CheckBoxBaseAdapter
    {
        public TestCheckBoxBaseAdapter(ButtonBase control) : base(control) { }

        public void CallDrawCheckFlat(
            PaintEventArgs e,
            LayoutData layout,
            Color checkColor,
            Color checkBackground,
            Color checkBorder,
            ColorData colors)
            => DrawCheckFlat(e, layout, checkColor, checkBackground, checkBorder, colors);

        protected override LayoutOptions Layout(PaintEventArgs e)
            => new LayoutOptions();

        protected override ButtonBaseAdapter CreateButtonAdapter()
            => new StubButtonBaseAdapter(Control);

        private class StubButtonBaseAdapter : ButtonBaseAdapter
        {
            public StubButtonBaseAdapter(ButtonBase control) : base(control) { }
            internal override void PaintUp(PaintEventArgs e, CheckState state) { }
            internal override void PaintDown(PaintEventArgs e, CheckState state) { }
            internal override void PaintOver(PaintEventArgs e, CheckState state) { }
            protected override LayoutOptions Layout(PaintEventArgs e) => new LayoutOptions();
        }

        internal override void PaintOver(PaintEventArgs e, CheckState state)
        {
        }

        internal override void PaintDown(PaintEventArgs e, CheckState state)
        {
        }

        internal override void PaintUp(PaintEventArgs e, CheckState state)
        {
        }
    }

    private static ButtonBaseAdapter.LayoutData CreateLayoutData(bool dotNetOneButtonCompat = false)
    {
        ButtonBaseAdapter.LayoutData layout = new(
            new ButtonBaseAdapter.LayoutOptions
            {
                DotNetOneButtonCompat = dotNetOneButtonCompat
            })
        {
            CheckBounds = new Rectangle(2, 2, 11, 11)
        };

        return layout;
    }

    private static ButtonBaseAdapter.ColorData CreateColorData(Graphics graphics)
    {
        ButtonBaseAdapter.ColorOptions options = new(
            graphics,
            Color.Black,
            Color.White
        );

        ButtonBaseAdapter.ColorData colorData = new(options)
        {
            ButtonFace = Color.LightGray,
            ButtonShadow = Color.Gray,
            Highlight = Color.Yellow
        };

        return colorData;
    }

    [WinFormsTheory]
    [InlineData(CheckState.Unchecked, false)]
    [InlineData(CheckState.Checked, false)]
    [InlineData(CheckState.Indeterminate, false)]
    [InlineData(CheckState.Unchecked, true)]
    [InlineData(CheckState.Checked, true)]
    [InlineData(CheckState.Indeterminate, true)]
    public void DrawCheckFlat_DoesNotThrow(CheckState checkState, bool dotNetOneButtonCompat)
    {
        (TestCheckBoxBaseAdapter adapter, TestCheckBox checkBox) = CreateAdapter(checkState);
        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, new Rectangle(0, 0, 20, 20));
        ButtonBaseAdapter.LayoutData layout = CreateLayoutData(dotNetOneButtonCompat);
        Color checkColor = Color.Black;
        Color checkBackground = Color.White;
        Color checkBorder = Color.Blue;
        ButtonBaseAdapter.ColorData colors = CreateColorData(graphics);

        Exception? ex = Record.Exception(() =>
            adapter.CallDrawCheckFlat(e, layout, checkColor, checkBackground, checkBorder, colors));

        ex.Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(false, CheckState.Unchecked, false, 0)]
    [InlineData(false, CheckState.Unchecked, true, 1)]
    [InlineData(true, CheckState.Indeterminate, true, 2)]
    public void DrawCheckBackground_CoversAllBranches(
        bool controlEnabled,
        CheckState checkState,
        bool disabledColors,
        int expectedBranch)
    {
        Rectangle bounds = new(1, 2, 3, 4);
        using Bitmap bitmap = new(10, 10);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Color checkBackground = expectedBranch == 2 ? SystemColors.Window : Color.Red;

        Exception? ex = Record.Exception(() =>
            CheckBoxBaseAdapter.DrawCheckBackground(
                controlEnabled,
                checkState,
                graphics,
                bounds,
                checkBackground,
                disabledColors));

        ex.Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(CheckState.Checked, true, true)]
    [InlineData(CheckState.Checked, false, true)]
    [InlineData(CheckState.Indeterminate, true, true)]
    [InlineData(CheckState.Indeterminate, false, true)]
    [InlineData(CheckState.Unchecked, true, false)]
    public void DrawCheckOnly_Protected_DoesNotThrow(
        CheckState checkState,
        bool enabled,
        bool isChecked)
    {
        (TestCheckBoxBaseAdapter adapter, TestCheckBox checkBox) = CreateAdapter(checkState);
        checkBox.Enabled = enabled;
        checkBox.Checked = isChecked;

        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, new Rectangle(0, 0, 20, 20));
        ButtonBaseAdapter.LayoutData layout = CreateLayoutData();
        ButtonBaseAdapter.ColorData colors = CreateColorData(graphics);
        Color checkColor = Color.Black;

        Exception? ex = Record.Exception(() =>
            adapter.TestAccessor().Dynamic.DrawCheckOnly(
                e,
                layout,
                colors,
                checkColor));

        ex.Should().BeNull();
    }

    [WinFormsFact]
    public void DrawPopupBorder_DoesNotThrow_AndInflatesRectangle()
    {
        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle original = new(2, 2, 10, 10);

        ButtonBaseAdapter.ColorData colors = CreateColorData(graphics);

        Rectangle result = CheckBoxBaseAdapter.DrawPopupBorder(graphics, original, colors);

        Rectangle expected = original;
        expected.Inflate(-1, -1);

        result.Should().Be(expected);
    }

    [WinFormsFact]
    public void DrawPopupBorder_PaintEventArgs_DoesNotThrow_AndInflatesRectangle()
    {
        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle original = new(3, 3, 8, 8);

        ButtonBaseAdapter.ColorData colors = CreateColorData(graphics);

        using PaintEventArgs e = new(graphics, new Rectangle(0, 0, 20, 20));
        Rectangle result = CheckBoxBaseAdapter.DrawPopupBorder(e, original, colors);

        Rectangle expected = original;
        expected.Inflate(-1, -1);

        result.Should().Be(expected);
    }

    [WinFormsFact]
    public void DrawPopupBorder_HDC_DoesNotThrow_AndInflatesRectangle()
    {
        using Bitmap bitmap = new(20, 20);
        using Graphics graphics = Graphics.FromImage(bitmap);
        Rectangle original = new(4, 4, 6, 6);

        ButtonBaseAdapter.ColorData colors = CreateColorData(graphics);

        IntPtr hdcPtr = graphics.GetHdc();
        try
        {
            Rectangle result = CheckBoxBaseAdapter.DrawPopupBorder(
                new HDC(hdcPtr),
                original,
                colors);

            Rectangle expected = original;
            expected.Inflate(-1, -1);

            result.Should().Be(expected);
        }
        finally
        {
            graphics.ReleaseHdc(hdcPtr);
        }
    }

    [Theory]
    [InlineData(null, true, 2, 2, 10, 10)]
    [InlineData("", true, 2, 2, 10, 10)]
    [InlineData(null, false, 2, 2, 10, 10)]
    [InlineData("", false, 2, 2, 10, 10)]
    public void AdjustFocusRectangle_SetsFocusAsExpected(
        string? text,
        bool autoSize,
        int checkX, int checkY, int checkW, int checkH)
    {
        (TestCheckBoxBaseAdapter adapter, TestCheckBox checkBox) = CreateAdapter(CheckState.Unchecked);
        checkBox.Text = text;
        checkBox.AutoSize = autoSize;

        ButtonBaseAdapter.LayoutData layout = new(new ButtonBaseAdapter.LayoutOptions())
        {
            CheckBounds = new Rectangle(checkX, checkY, checkW, checkH),
            Field = new Rectangle(5, 5, 20, 20)
        };

        adapter.TestAccessor().Dynamic.AdjustFocusRectangle(layout);

        if (string.IsNullOrEmpty(text))
        {
            if (autoSize)
            {
                Rectangle expected = Rectangle.Inflate(layout.CheckBounds, -2, -2);
                layout.Focus.Should().Be(expected);
            }
            else
            {
                layout.Focus.Should().Be(new Rectangle(5, 5, 20, 20));
            }
        }
        else
        {
            layout.Focus.Should().Be(default);
        }
    }
}
