// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.ButtonInternal;

namespace System.Windows.Forms.Tests;

public class RadioButtonPopupAdapterTests
{
    private class TestRadioButton : RadioButton
    {
        public TestRadioButton()
        {
        }
    }

    private class TestRadioButtonPopupAdapter : RadioButtonPopupAdapter
    {
        public TestRadioButtonPopupAdapter(ButtonBase control) : base(control)
        {
        }

        public void CallPaintUp(PaintEventArgs e, CheckState state) => PaintUp(e, state);
        public void CallPaintOver(PaintEventArgs e, CheckState state) => PaintOver(e, state);
        public void CallPaintDown(PaintEventArgs e, CheckState state) => PaintDown(e, state);
        public LayoutOptions CallLayout(PaintEventArgs e) => Layout(e);
        public ButtonBaseAdapter CallCreateButtonAdapter() => CreateButtonAdapter();
    }

    private static (TestRadioButton, TestRadioButtonPopupAdapter) CreateAdapter()
    {
        TestRadioButton radioButton = new();
        TestRadioButtonPopupAdapter adapter = new(radioButton);

        return (radioButton, adapter);
    }

    [WinFormsFact]
    public void PaintUp_ButtonAppearance_DelegatesToButtonPopupAdapter()
    {
        (TestRadioButton radioButton, TestRadioButtonPopupAdapter adapter) = CreateAdapter();
        radioButton.Appearance = Appearance.Button;
        using Bitmap bmp = new(20, 20);
        using Graphics g = Graphics.FromImage(bmp);
        using PaintEventArgs e = new(g, new Rectangle(0, 0, 20, 20));

        adapter.CallPaintUp(e, CheckState.Checked);

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
    public void PaintUp_RadioAppearance_Draws()
    {
        (TestRadioButton radioButton, TestRadioButtonPopupAdapter adapter) = CreateAdapter();
        radioButton.Appearance = Appearance.Normal;
        using Bitmap bmp = new(20, 20);
        using Graphics g = Graphics.FromImage(bmp);
        using PaintEventArgs e = new(g, new Rectangle(0, 0, 20, 20));

        adapter.CallPaintUp(e, CheckState.Checked);

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

        anyPixelChanged.Should().BeTrue("PaintUp should modify the bitmap when Appearance is Normal.");
    }
}
