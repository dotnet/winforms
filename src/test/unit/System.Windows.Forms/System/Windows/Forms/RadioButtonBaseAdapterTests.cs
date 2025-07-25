// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.ButtonInternal;
using static System.Windows.Forms.ButtonInternal.ButtonBaseAdapter;

namespace System.Windows.Forms.Tests;

public class RadioButtonBaseAdapterTests
{
    private class TestRadioButton : RadioButton
    {
        public TestRadioButton()
        {
        }

        public void InvokeOnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }

        public void InvokeOnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
        }
    }

    private class TestRadioButtonBaseAdapter : RadioButtonBaseAdapter
    {
        public TestRadioButtonBaseAdapter(RadioButton control) : base(control)
        {
        }

        public new RadioButton Control => base.Control;

        public void CallDrawCheckFlat(PaintEventArgs e, LayoutData layout, Color checkColor, Color checkBackground, Color checkBorder)
            => DrawCheckFlat(e, layout, checkColor, checkBackground, checkBorder);

        public void CallDrawCheckBackgroundFlat(PaintEventArgs e, Rectangle bounds, Color borderColor, Color checkBackground)
            => DrawCheckBackgroundFlat(e, bounds, borderColor, checkBackground);

        public void CallDrawCheckBackground3DLite(PaintEventArgs e, Rectangle bounds, Color checkBackground, ColorData colors, bool disabledColors)
            => DrawCheckBackground3DLite(e, bounds, checkBackground, colors, disabledColors);

        public void CallDrawCheckOnly(PaintEventArgs e, LayoutData layout, Color checkColor, bool disabledColors)
            => DrawCheckOnly(e, layout, checkColor, disabledColors);

        public ButtonState CallGetState() => GetState();

        public void CallDrawCheckBox(PaintEventArgs e, LayoutData layout)
            => DrawCheckBox(e, layout);

        public void CallAdjustFocusRectangle(LayoutData layout)
            => AdjustFocusRectangle(layout);

        public LayoutOptions CallCommonLayout() => CommonLayout();

        internal override void PaintOver(PaintEventArgs e, CheckState state)
        {
        }

        internal override void PaintUp(PaintEventArgs e, CheckState state)
        {
        }

        internal override void PaintDown(PaintEventArgs e, CheckState state)
        {
        }

        protected override LayoutOptions Layout(PaintEventArgs e)
        {
            return new LayoutOptions();
        }

        protected override ButtonBaseAdapter CreateButtonAdapter()
        {
            return this;
        }
    }

    private static (TestRadioButton, TestRadioButtonBaseAdapter) CreateAdapter()
    {
        using TestRadioButton radioButton = new();
        TestRadioButtonBaseAdapter radioButtonBaseAdapter = new(radioButton);

        return (radioButton, radioButtonBaseAdapter);
    }

    private static bool AnyPixelChanged(Bitmap bmp, Rectangle bounds)
    {
        Color reference = bmp.GetPixel(bounds.X, bounds.Y);

        for (int x = bounds.X; x < bounds.Right; x++)
        {
            for (int y = bounds.Y; y < bounds.Bottom; y++)
            {
                if (bmp.GetPixel(x, y) != reference)
                {
                    return true;
                }
            }
        }

        return false;
    }

    [WinFormsFact]
    public void DrawCheckFlat_CallsBackgroundAndCheckOnly()
    {
        (_, TestRadioButtonBaseAdapter radioButtonBaseAdapter) = CreateAdapter();
        using Bitmap bmp = new(20, 20);
        using Graphics g = Graphics.FromImage(bmp);
        using PaintEventArgs e = new(g, new Rectangle(0, 0, 20, 20));
        LayoutData layout = new(new LayoutOptions());

        radioButtonBaseAdapter.CallDrawCheckFlat(e, layout, Color.Red, Color.Blue, Color.Green);

        bool anyPixelChanged = AnyPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

        anyPixelChanged.Should().BeTrue("DrawCheckFlat should modify the bitmap.");
    }

    [WinFormsFact]
    public void DrawCheckBackgroundFlat_EnabledAndDisabled_ChangesColors()
    {
        (TestRadioButton radioButton, TestRadioButtonBaseAdapter radioButtonBaseAdapter) = CreateAdapter();
        using Bitmap bmp = new(20, 20);
        using Graphics g = Graphics.FromImage(bmp);
        using PaintEventArgs e = new(g, new Rectangle(0, 0, 20, 20));
        Rectangle bounds = new(1, 1, 12, 12);

        radioButton.Enabled = true;
        radioButtonBaseAdapter.CallDrawCheckBackgroundFlat(e, bounds, Color.Black, Color.White);

        bool anyPixelChangedEnabled = AnyPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

        anyPixelChangedEnabled.Should().BeTrue("drawing the background when enabled should modify the bitmap in the bounds area");

        using (Graphics clearG = Graphics.FromImage(bmp))
        {
            clearG.Clear(Color.Empty);
        }

        radioButton.Enabled = false;
        radioButtonBaseAdapter.CallDrawCheckBackgroundFlat(e, bounds, Color.Black, Color.White);

        bool anyPixelChangedDisabled = AnyPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

        anyPixelChangedDisabled.Should().BeTrue("drawing the background when disabled should modify the bitmap in the bounds area");
    }

    [WinFormsFact]
    public void DrawCheckBackground3DLite_EnabledAndDisabled_UsesCorrectColors()
    {
        (TestRadioButton radioButton, TestRadioButtonBaseAdapter radioButtonBaseAdapter) = CreateAdapter();
        using Bitmap bmp = new(20, 20);
        using Graphics g = Graphics.FromImage(bmp);
        using PaintEventArgs e = new(g, new Rectangle(0, 0, 20, 20));
        Rectangle bounds = new(1, 1, 12, 12);
        ColorData colors = new(
            new ColorOptions(
                g,
                Color.Black,
                Color.White
            )
        )
        {
            ButtonFace = Color.Gray,
            ButtonShadow = Color.DarkGray,
            Highlight = Color.LightGray
        };

        radioButton.Enabled = true;
        radioButtonBaseAdapter.CallDrawCheckBackground3DLite(e, bounds, Color.White, colors, disabledColors: false);

        bool anyPixelChangedEnabled = AnyPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

        anyPixelChangedEnabled.Should().BeTrue("drawing the 3D lite background when enabled should modify the bitmap in the bounds area");

        using (Graphics clearG = Graphics.FromImage(bmp))
        {
            clearG.Clear(Color.Empty);
        }

        radioButton.Enabled = false;
        radioButtonBaseAdapter.CallDrawCheckBackground3DLite(e, bounds, Color.White, colors, disabledColors: true);

        bool anyPixelChangedDisabled = AnyPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

        anyPixelChangedDisabled.Should().BeTrue("drawing the 3D lite background when disabled should modify the bitmap in the bounds area");
    }

    [WinFormsFact]
    public void DrawCheckOnly_CheckedAndUnchecked_DrawsOrSkips()
    {
        (TestRadioButton radioButton, TestRadioButtonBaseAdapter radioButtonBaseAdapter) = CreateAdapter();
        using Bitmap bmp = new(20, 20);
        using Graphics g = Graphics.FromImage(bmp);
        using PaintEventArgs e = new(g, new Rectangle(0, 0, 20, 20));
        LayoutData layoutData = new(new LayoutOptions())
        {
            CheckBounds = new Rectangle(2, 2, 12, 12)
        };

        radioButton.Checked = false;
        radioButtonBaseAdapter.CallDrawCheckOnly(e, layoutData, Color.Black, disabledColors: false);

        bool anyPixelChangedUnchecked = AnyPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

        anyPixelChangedUnchecked.Should().BeFalse("DrawCheckOnly should not modify the bitmap when unchecked.");

        radioButton.Checked = true;
        radioButton.Enabled = true;
        radioButtonBaseAdapter.CallDrawCheckOnly(e, layoutData, Color.Black, disabledColors: false);

        bool anyPixelChangedChecked = AnyPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

        anyPixelChangedChecked.Should().BeTrue("DrawCheckOnly should modify the bitmap when checked and enabled.");

        radioButton.Enabled = false;
        radioButtonBaseAdapter.CallDrawCheckOnly(e, layoutData, Color.Black, disabledColors: true);

        bool anyPixelChangedCheckedDisabled = AnyPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

        anyPixelChangedCheckedDisabled.Should().BeTrue("DrawCheckOnly should modify the bitmap when checked and disabled.");
    }

    [WinFormsFact]
    public void GetState_ReturnsCorrectButtonState()
    {
        (TestRadioButton radioButton, TestRadioButtonBaseAdapter radioButtonBaseAdapter) = CreateAdapter();

        radioButton.Checked = false;
        radioButton.Enabled = true;
        radioButton.InvokeOnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        ButtonState state = radioButtonBaseAdapter.CallGetState();

        state.Should().Be(ButtonState.Normal);

        radioButton.Checked = true;
        state = radioButtonBaseAdapter.CallGetState();

        state.HasFlag(ButtonState.Checked).Should().BeTrue();

        radioButton.Enabled = false;
        state = radioButtonBaseAdapter.CallGetState();

        state.HasFlag(ButtonState.Inactive).Should().BeTrue();

        radioButton.Enabled = true;
        radioButton.InvokeOnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        state = radioButtonBaseAdapter.CallGetState();

        state.HasFlag(ButtonState.Pushed).Should().BeTrue();
    }

    [WinFormsFact]
    public void DrawCheckBox_VisualStylesAndNoVisualStyles_DoesNotThrow()
    {
        (_, TestRadioButtonBaseAdapter radioButtonBaseAdapter) = CreateAdapter();
        using Bitmap bmp = new(20, 20);
        using Graphics g = Graphics.FromImage(bmp);
        using PaintEventArgs e = new(g, new Rectangle(0, 0, 20, 20));
        LayoutData layoutData = new(new LayoutOptions())
        {
            CheckBounds = new Rectangle(2, 2, 12, 12)
        };

        radioButtonBaseAdapter.CallDrawCheckBox(e, layoutData);

        bool anyPixelChanged = AnyPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

        anyPixelChanged.Should().BeTrue("DrawCheckBox should modify the bitmap.");
    }

    [WinFormsFact]
    public void AdjustFocusRectangle_AutoSizeAndNoText_ChangesFocus()
    {
        (TestRadioButton radioButton, TestRadioButtonBaseAdapter radioButtonBaseAdapter) = CreateAdapter();
        LayoutData layoutData = new(new LayoutOptions())
        {
            CheckBounds = new Rectangle(1, 2, 3, 4),
            Field = new Rectangle(5, 6, 7, 8)
        };

        radioButton.Text = string.Empty;
        radioButton.AutoSize = true;
        radioButtonBaseAdapter.CallAdjustFocusRectangle(layoutData);
        layoutData.Focus.Should().Be(layoutData.CheckBounds);

        radioButton.AutoSize = false;
        radioButtonBaseAdapter.CallAdjustFocusRectangle(layoutData);
        layoutData.Focus.Should().Be(layoutData.Field);
    }

    [WinFormsFact]
    public void CommonLayout_SetsCheckAlign()
    {
        (TestRadioButton radioButton, TestRadioButtonBaseAdapter radioButtonBaseAdapter) = CreateAdapter();
        radioButton.CheckAlign = ContentAlignment.BottomRight;
        LayoutOptions layoutOptions = radioButtonBaseAdapter.CallCommonLayout();

        layoutOptions.CheckAlign.Should().Be(ContentAlignment.BottomRight);
    }
}
