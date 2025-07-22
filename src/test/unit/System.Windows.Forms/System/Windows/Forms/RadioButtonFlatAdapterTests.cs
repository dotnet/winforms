// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.ButtonInternal;
using static System.Windows.Forms.ButtonInternal.ButtonBaseAdapter;

namespace System.Windows.Forms.Tests;

public class RadioButtonFlatAdapterTests
{
    private class TestRadioButton : RadioButton
    {
        public TestRadioButton()
        {
        }
    }

    private class TestRadioButtonFlatAdapter : RadioButtonFlatAdapter
    {
        public TestRadioButtonFlatAdapter(ButtonBase control) : base(control)
        {
        }

        public void CallPaintDown(PaintEventArgs e, CheckState state) => PaintDown(e, state);
        public void CallPaintOver(PaintEventArgs e, CheckState state) => PaintOver(e, state);
        public void CallPaintUp(PaintEventArgs e, CheckState state) => PaintUp(e, state);
        public LayoutOptions CallLayout(PaintEventArgs e) => Layout(e);
        public ButtonBaseAdapter CallCreateButtonAdapter() => CreateButtonAdapter();
    }

    private static (TestRadioButton, TestRadioButtonFlatAdapter) CreateAdapter()
    {
        using TestRadioButton radioButton = new();
        TestRadioButtonFlatAdapter radioButtonBaseAdapter = new(radioButton);

        return (radioButton, radioButtonBaseAdapter);
    }

    [WinFormsFact]
    public void PaintDown_ButtonAppearance_DelegatesToButtonFlatAdapter()
    {
        (TestRadioButton radioButton, TestRadioButtonFlatAdapter radioButtonBaseAdapter) = CreateAdapter();
        radioButton.Appearance = Appearance.Button;
        using Bitmap bmp = new(20, 20);
        using Graphics g = Graphics.FromImage(bmp);
        using PaintEventArgs e = new(g, new Rectangle(0, 0, 20, 20));

        radioButtonBaseAdapter.CallPaintDown(e, CheckState.Checked);

        bool anyPixelChanged = false;
        Color defaultColor = bmp.GetPixel(0, 0);
        for (int x = 0; x < bmp.Width && !anyPixelChanged; x++)
        {
            for (int y = 0; y < bmp.Height && !anyPixelChanged; y++)
            {
                if (bmp.GetPixel(x, y) != defaultColor)
                {
                    anyPixelChanged = true;
                }
            }
        }

        anyPixelChanged.Should().BeTrue("PaintDown should modify the bitmap when Appearance is Button.");
    }

    [WinFormsFact]
    public void PaintDown_RadioAppearance_EnabledAndDisabled_Draws()
    {
        (TestRadioButton radioButton, TestRadioButtonFlatAdapter radioButtonBaseAdapter) = CreateAdapter();
        radioButton.Appearance = Appearance.Normal;
        using Bitmap bmp = new(20, 20);
        using Graphics g = Graphics.FromImage(bmp);
        using PaintEventArgs e = new(g, new Rectangle(0, 0, 20, 20));

        radioButton.Enabled = true;
        radioButtonBaseAdapter.CallPaintDown(e, CheckState.Checked);

        bool anyPixelChangedEnabled = false;
        Color defaultColorEnabled = bmp.GetPixel(0, 0);
        for (int x = 0; x < bmp.Width && !anyPixelChangedEnabled; x++)
        {
            for (int y = 0; y < bmp.Height && !anyPixelChangedEnabled; y++)
            {
                if (bmp.GetPixel(x, y) != defaultColorEnabled)
                {
                    anyPixelChangedEnabled = true;
                }
            }
        }

        anyPixelChangedEnabled.Should().BeTrue("PaintDown should modify the bitmap when enabled and Appearance is Normal.");

        using (Graphics clearG = Graphics.FromImage(bmp))
        {
            clearG.Clear(Color.Empty);
        }

        radioButton.Enabled = false;
        radioButtonBaseAdapter.CallPaintDown(e, CheckState.Unchecked);

        bool anyPixelChangedDisabled = false;
        Color defaultColorDisabled = bmp.GetPixel(0, 0);
        for (int x = 0; x < bmp.Width && !anyPixelChangedDisabled; x++)
        {
            for (int y = 0; y < bmp.Height && !anyPixelChangedDisabled; y++)
            {
                if (bmp.GetPixel(x, y) != defaultColorDisabled)
                {
                    anyPixelChangedDisabled = true;
                }
            }
        }

        anyPixelChangedDisabled.Should().BeTrue("PaintDown should modify the bitmap when disabled and Appearance is Normal.");
    }

    [WinFormsFact]
    public void PaintOver_ButtonAppearance_DelegatesToButtonFlatAdapter()
    {
        (TestRadioButton radioButton, TestRadioButtonFlatAdapter radioButtonBaseAdapter) = CreateAdapter();
        radioButton.Appearance = Appearance.Button;
        using Bitmap bmp = new(20, 20);
        using Graphics g = Graphics.FromImage(bmp);
        using PaintEventArgs e = new(g, new Rectangle(0, 0, 20, 20));

        radioButtonBaseAdapter.CallPaintOver(e, CheckState.Checked);

        bool anyPixelChanged = false;
        Color defaultColor = bmp.GetPixel(0, 0);
        for (int x = 0; x < bmp.Width && !anyPixelChanged; x++)
        {
            for (int y = 0; y < bmp.Height && !anyPixelChanged; y++)
            {
                if (bmp.GetPixel(x, y) != defaultColor)
                {
                    anyPixelChanged = true;
                }
            }
        }

        anyPixelChanged.Should().BeTrue("PaintOver should modify the bitmap when Appearance is Button.");
    }

    [WinFormsFact]
    public void PaintOver_RadioAppearance_EnabledAndDisabled_Draws()
    {
        (TestRadioButton radioButton, TestRadioButtonFlatAdapter radioButtonBaseAdapter) = CreateAdapter();
        radioButton.Appearance = Appearance.Normal;
        using Bitmap bmp = new(20, 20);
        using Graphics g = Graphics.FromImage(bmp);
        using PaintEventArgs e = new(g, new Rectangle(0, 0, 20, 20));

        radioButton.Enabled = true;
        radioButtonBaseAdapter.CallPaintOver(e, CheckState.Checked);

        bool anyPixelChangedEnabled = false;
        Color defaultColorEnabled = bmp.GetPixel(0, 0);
        for (int x = 0; x < bmp.Width && !anyPixelChangedEnabled; x++)
        {
            for (int y = 0; y < bmp.Height && !anyPixelChangedEnabled; y++)
            {
                if (bmp.GetPixel(x, y) != defaultColorEnabled)
                {
                    anyPixelChangedEnabled = true;
                }
            }
        }

        anyPixelChangedEnabled.Should().BeTrue("PaintOver should modify the bitmap when enabled and Appearance is Normal.");

        using (Graphics clearG = Graphics.FromImage(bmp))
        {
            clearG.Clear(Color.Empty);
        }

        radioButton.Enabled = false;
        radioButtonBaseAdapter.CallPaintOver(e, CheckState.Unchecked);

        bool anyPixelChangedDisabled = false;
        Color defaultColorDisabled = bmp.GetPixel(0, 0);
        for (int x = 0; x < bmp.Width && !anyPixelChangedDisabled; x++)
        {
            for (int y = 0; y < bmp.Height && !anyPixelChangedDisabled; y++)
            {
                if (bmp.GetPixel(x, y) != defaultColorDisabled)
                {
                    anyPixelChangedDisabled = true;
                }
            }
        }

        anyPixelChangedDisabled.Should().BeTrue("PaintOver should modify the bitmap when disabled and Appearance is Normal.");
    }

    [WinFormsFact]
    public void PaintUp_ButtonAppearance_DelegatesToButtonFlatAdapter()
    {
        (TestRadioButton radioButton, TestRadioButtonFlatAdapter radioButtonBaseAdapter) = CreateAdapter();
        radioButton.Appearance = Appearance.Button;
        using Bitmap bmp = new(20, 20);
        using Graphics g = Graphics.FromImage(bmp);
        using PaintEventArgs e = new(g, new Rectangle(0, 0, 20, 20));

        radioButtonBaseAdapter.CallPaintUp(e, CheckState.Checked);

        bool anyPixelChanged = false;
        Color defaultColor = bmp.GetPixel(0, 0);
        for (int x = 0; x < bmp.Width && !anyPixelChanged; x++)
        {
            for (int y = 0; y < bmp.Height && !anyPixelChanged; y++)
            {
                if (bmp.GetPixel(x, y) != defaultColor)
                {
                    anyPixelChanged = true;
                }
            }
        }

        anyPixelChanged.Should().BeTrue("PaintUp should modify the bitmap when Appearance is Button.");
    }

    [WinFormsFact]
    public void PaintUp_RadioAppearance_EnabledAndDisabled_Draws()
    {
        (TestRadioButton radioButton, TestRadioButtonFlatAdapter radioButtonBaseAdapter) = CreateAdapter();
        radioButton.Appearance = Appearance.Normal;
        using Bitmap bmp = new(20, 20);
        using Graphics g = Graphics.FromImage(bmp);
        using PaintEventArgs e = new(g, new Rectangle(0, 0, 20, 20));

        radioButton.Enabled = true;
        radioButtonBaseAdapter.CallPaintUp(e, CheckState.Checked);

        bool anyPixelChangedEnabled = false;
        Color defaultColorEnabled = bmp.GetPixel(0, 0);
        for (int x = 0; x < bmp.Width && !anyPixelChangedEnabled; x++)
        {
            for (int y = 0; y < bmp.Height && !anyPixelChangedEnabled; y++)
            {
                if (bmp.GetPixel(x, y) != defaultColorEnabled)
                {
                    anyPixelChangedEnabled = true;
                }
            }
        }

        anyPixelChangedEnabled.Should().BeTrue("PaintUp should modify the bitmap when enabled and Appearance is Normal.");

        using (Graphics clearG = Graphics.FromImage(bmp))
        {
            clearG.Clear(Color.Empty);
        }

        radioButton.Enabled = false;
        radioButtonBaseAdapter.CallPaintUp(e, CheckState.Unchecked);

        bool anyPixelChangedDisabled = false;
        Color defaultColorDisabled = bmp.GetPixel(0, 0);
        for (int x = 0; x < bmp.Width && !anyPixelChangedDisabled; x++)
        {
            for (int y = 0; y < bmp.Height && !anyPixelChangedDisabled; y++)
            {
                if (bmp.GetPixel(x, y) != defaultColorDisabled)
                {
                    anyPixelChangedDisabled = true;
                }
            }
        }

        anyPixelChangedDisabled.Should().BeTrue("PaintUp should modify the bitmap when disabled and Appearance is Normal.");
    }

    [WinFormsFact]
    public void Layout_SetsFlatCheckSizeAndShadowedText()
    {
        (_, TestRadioButtonFlatAdapter radioButtonBaseAdapter) = CreateAdapter();
        using Bitmap bmp = new(20, 20);
        using Graphics g = Graphics.FromImage(bmp);
        using PaintEventArgs e = new(g, new Rectangle(0, 0, 20, 20));

        LayoutOptions options = radioButtonBaseAdapter.CallLayout(e);

        options.CheckSize.Should().Be(12);
        options.ShadowedText.Should().BeFalse();
    }

    [WinFormsFact]
    public void CreateButtonAdapter_ReturnsButtonFlatAdapter()
    {
        (_, TestRadioButtonFlatAdapter radioButtonBaseAdapter) = CreateAdapter();
        ButtonBaseAdapter result = radioButtonBaseAdapter.CallCreateButtonAdapter();

        result.Should().BeOfType<ButtonFlatAdapter>();
    }
}
