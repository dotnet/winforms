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
        public bool TestFocused { get; set; }

        public override bool Focused => TestFocused;

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

        public void DrawFlatCheckMark(PaintEventArgs e, LayoutData layout, Color checkColor, Color checkBackground, Color checkBorder)
            => DrawCheckFlat(e, layout, checkColor, checkBackground, checkBorder);

        public void DrawBackgroundFlatMark(PaintEventArgs e, Rectangle bounds, Color borderColor, Color checkBackground)
            => DrawCheckBackgroundFlat(e, bounds, borderColor, checkBackground);

        public void DrawBackground3DLiteMark(PaintEventArgs e, Rectangle bounds, Color checkBackground, ColorData colors, bool disabledColors)
            => DrawCheckBackground3DLite(e, bounds, checkBackground, colors, disabledColors);

        public void DrawCheckOnlyMark(PaintEventArgs e, LayoutData layout, Color checkColor, bool disabledColors)
            => DrawCheckOnly(e, layout, checkColor, disabledColors);

        public ButtonState GetStateMark() => GetState();

        public void DrawCheckBoxMark(PaintEventArgs e, LayoutData layout)
            => DrawCheckBox(e, layout);

        public void AdjustFocusRectangleMark(LayoutData layout)
            => AdjustFocusRectangle(layout);

        public LayoutOptions GetLayoutOptions() => CommonLayout();
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

    /// <summary>
    ///  Determines whether any pixel within the specified bounds of the bitmap differs from the reference pixel
    ///  at the top-left corner of the bounds. Used to verify if drawing operations have modified the bitmap.
    /// </summary>
    /// <param name="bmp">The bitmap to inspect.</param>
    /// <param name="bounds">The rectangle area within the bitmap to check for changes.</param>
    /// <returns>
    ///  <see langword="true"/> if any pixel in the bounds differs from the reference pixel; otherwise, <see langword="false"/>.
    /// </returns>
    private static bool HasPixelChanged(Bitmap bmp, Rectangle bounds)
    {
        Color referenceColor = bmp.GetPixel(bounds.X, bounds.Y);

        for (int x = bounds.X; x < bounds.Right; x++)
        {
            for (int y = bounds.Y; y < bounds.Bottom; y++)
            {
                if (bmp.GetPixel(x, y) != referenceColor)
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

        radioButtonBaseAdapter.DrawFlatCheckMark(e, layout, Color.Red, Color.Blue, Color.Green);

        bool anyPixelChanged = HasPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

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
        radioButtonBaseAdapter.DrawBackgroundFlatMark(e, bounds, Color.Black, Color.White);

        bool anyPixelChangedEnabled = HasPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

        anyPixelChangedEnabled.Should().BeTrue("drawing the background when enabled should modify the bitmap in the bounds area");

        using (Graphics clearG = Graphics.FromImage(bmp))
        {
            clearG.Clear(Color.Empty);
        }

        radioButton.Enabled = false;
        radioButtonBaseAdapter.DrawBackgroundFlatMark(e, bounds, Color.Black, Color.White);

        bool anyPixelChangedDisabled = HasPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

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
        radioButtonBaseAdapter.DrawBackground3DLiteMark(e, bounds, Color.White, colors, disabledColors: false);

        bool anyPixelChangedEnabled = HasPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

        anyPixelChangedEnabled.Should().BeTrue("drawing the 3D lite background when enabled should modify the bitmap in the bounds area");

        using (Graphics clearG = Graphics.FromImage(bmp))
        {
            clearG.Clear(Color.Empty);
        }

        radioButton.Enabled = false;
        radioButtonBaseAdapter.DrawBackground3DLiteMark(e, bounds, Color.White, colors, disabledColors: true);

        bool anyPixelChangedDisabled = HasPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

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
        radioButtonBaseAdapter.DrawCheckOnlyMark(e, layoutData, Color.Black, disabledColors: false);

        bool anyPixelChangedUnchecked = HasPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

        anyPixelChangedUnchecked.Should().BeFalse("DrawCheckOnly should not modify the bitmap when unchecked.");

        radioButton.Checked = true;
        radioButton.Enabled = true;
        radioButtonBaseAdapter.DrawCheckOnlyMark(e, layoutData, Color.Black, disabledColors: false);

        bool anyPixelChangedChecked = HasPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

        anyPixelChangedChecked.Should().BeTrue("DrawCheckOnly should modify the bitmap when checked and enabled.");

        radioButton.Enabled = false;
        radioButtonBaseAdapter.DrawCheckOnlyMark(e, layoutData, Color.Black, disabledColors: true);

        bool anyPixelChangedCheckedDisabled = HasPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

        anyPixelChangedCheckedDisabled.Should().BeTrue("DrawCheckOnly should modify the bitmap when checked and disabled.");
    }

    [WinFormsFact]
    public void GetState_ReturnsCorrectButtonState()
    {
        (TestRadioButton radioButton, TestRadioButtonBaseAdapter radioButtonBaseAdapter) = CreateAdapter();

        radioButton.Checked = false;
        radioButton.Enabled = true;
        radioButton.InvokeOnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        ButtonState state = radioButtonBaseAdapter.GetStateMark();

        state.Should().Be(ButtonState.Normal);

        radioButton.Checked = true;
        state = radioButtonBaseAdapter.GetStateMark();

        state.HasFlag(ButtonState.Checked).Should().BeTrue();

        radioButton.Enabled = false;
        state = radioButtonBaseAdapter.GetStateMark();

        state.HasFlag(ButtonState.Inactive).Should().BeTrue();

        radioButton.Enabled = true;
        radioButton.InvokeOnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
        state = radioButtonBaseAdapter.GetStateMark();

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

        radioButtonBaseAdapter.DrawCheckBoxMark(e, layoutData);

        bool anyPixelChanged = HasPixelChanged(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));

        anyPixelChanged.Should().BeTrue("DrawCheckBox should modify the bitmap.");
    }

    [WinFormsTheory]
    [InlineData(false)]
    [InlineData(true)]
    public void AdjustFocusRectangle_AutoSizeAndNoText_ChangesFocus(bool hasImage)
    {
        using TestRadioButton radioButton = new();
        using Bitmap image = new(256, 256);
        radioButton.Image = hasImage ? image : null;
        TestRadioButtonBaseAdapter radioButtonBaseAdapter = new(radioButton);
        LayoutData layoutData = new(new LayoutOptions())
        {
            CheckBounds = new Rectangle(1, 2, 3, 4),
            Field = new Rectangle(5, 6, 7, 8)
        };

        radioButton.Text = string.Empty;
        radioButton.AutoSize = true;
        radioButtonBaseAdapter.AdjustFocusRectangleMark(layoutData);
        layoutData.Focus.Should().Be(layoutData.CheckBounds);

        radioButton.AutoSize = false;
        radioButtonBaseAdapter.AdjustFocusRectangleMark(layoutData);
        layoutData.Focus.Should().Be(layoutData.Field);
    }

    [WinFormsFact]
    public void CommonLayout_SetsCheckAlign()
    {
        (TestRadioButton radioButton, TestRadioButtonBaseAdapter radioButtonBaseAdapter) = CreateAdapter();
        radioButton.CheckAlign = ContentAlignment.BottomRight;
        LayoutOptions layoutOptions = radioButtonBaseAdapter.CommonLayout();

        layoutOptions.CheckAlign.Should().Be(ContentAlignment.BottomRight);
    }

    [WinFormsTheory]
    [InlineData(false, false)]
    [InlineData(false, true)]
    [InlineData(true, false)]
    [InlineData(true, true)]
    public void AdjustFocusRectangle_WithTextAndImage_FocusesTextOnly(bool autoSize, bool hasImage)
    {
        using TestRadioButton radioButton = new()
        {
            AutoSize = autoSize,
            Text = "RadioButton"
        };
        using Bitmap image = new(256, 256);
        radioButton.Image = hasImage ? image : null;
        TestRadioButtonBaseAdapter adapter = new(radioButton);
        Rectangle originalFocus = new(-50, -50, 256, 256);
        LayoutData layout = new(new LayoutOptions())
        {
            Field = new Rectangle(14, 0, 90, 24),
            TextBounds = new Rectangle(20, 5, 70, 13),
            Focus = originalFocus
        };

        adapter.AdjustFocusRectangleMark(layout);

        layout.Focus.Should().Be(hasImage
            ? layout.TextBounds
            : originalFocus);
    }

    [WinFormsTheory]
    [InlineData(FlatStyle.Standard, false, 256)]
    [InlineData(FlatStyle.Flat, false, 256)]
    [InlineData(FlatStyle.Popup, false, 256)]
    [InlineData(FlatStyle.Standard, false, 16)]
    [InlineData(FlatStyle.Flat, false, 16)]
    [InlineData(FlatStyle.Popup, false, 16)]
    [InlineData(FlatStyle.Standard, true, 256)]
    [InlineData(FlatStyle.Flat, true, 256)]
    [InlineData(FlatStyle.Popup, true, 256)]
    public void PaintUp_WithImage_DrawsFocusRectangleAroundTextOnly(FlatStyle flatStyle, bool autoSize, int imageSize)
    {
        using Bitmap image = new(imageSize, imageSize);
        using (Graphics imageGraphics = Graphics.FromImage(image))
        {
            imageGraphics.Clear(Color.Fuchsia);
        }

        using TestRadioButton radioButton = new()
        {
            AutoSize = autoSize,
            BackColor = Color.White,
            ForeColor = Color.Black,
            FlatStyle = flatStyle,
            Image = image,
            Size = new Size(104, 24),
            Text = "RadioButton",
            VisualStylesMode = VisualStylesMode.Classic
        };
        if (autoSize)
        {
            radioButton.Size = radioButton.GetPreferredSize(Size.Empty);
        }

        ButtonBaseAdapter adapter = flatStyle switch
        {
            FlatStyle.Flat => new RadioButtonFlatAdapter(radioButton),
            FlatStyle.Popup => new RadioButtonPopupAdapter(radioButton),
            _ => new RadioButtonStandardAdapter(radioButton)
        };
        using Bitmap unfocused = new(radioButton.Width, radioButton.Height);
        using Bitmap focused = new(radioButton.Width, radioButton.Height);
        Rectangle textBounds;
        using (Graphics graphics = Graphics.FromImage(unfocused))
        using (PaintEventArgs e = new(graphics, radioButton.ClientRectangle))
        {
            LayoutOptions options = adapter.TestAccessor.Dynamic.Layout(e);
            textBounds = options.Layout().TextBounds;
            adapter.PaintUp(e, CheckState.Unchecked);
        }

        radioButton.TestFocused = true;
        using (Graphics graphics = Graphics.FromImage(focused))
        using (PaintEventArgs e = new(graphics, radioButton.ClientRectangle))
        {
            adapter.PaintUp(e, CheckState.Unchecked);
        }

        int changedPixels = 0;
        for (int y = 0; y < focused.Height; y++)
        {
            for (int x = 0; x < focused.Width; x++)
            {
                if (focused.GetPixel(x, y) != unfocused.GetPixel(x, y))
                {
                    textBounds.Contains(x, y).Should().BeTrue("the focus rectangle must only surround the text");
                    changedPixels++;
                }
            }
        }

        changedPixels.Should().BeGreaterThan(0, "the focus rectangle must remain visible over the image");
    }
}
