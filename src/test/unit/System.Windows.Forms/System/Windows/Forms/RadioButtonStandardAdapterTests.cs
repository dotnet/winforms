// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.ButtonInternal;

namespace System.Windows.Forms.Tests;

public class RadioButtonStandardAdapterTests : IDisposable
{
    private RadioButton? _radioButton;

    public void Dispose()
    {
        _radioButton?.Dispose();

        // Restore default color mode after dark mode tests.
        Application.SetColorMode(SystemColorMode.Classic);
    }

    private (RadioButtonStandardAdapter, RadioButton) CreateAdapter(Appearance appearance = Appearance.Normal, bool enabled = true)
    {
        _radioButton = new RadioButton
        {
            Appearance = appearance,
            Enabled = enabled,
            Width = 100,
            Height = 25
        };

        RadioButtonStandardAdapter adapter = new(_radioButton);

        return (adapter, _radioButton);
    }

    [WinFormsFact]
    public void CreateButtonAdapter_InClassicMode_ReturnsButtonStandardAdapter()
    {
        Application.SetColorMode(SystemColorMode.Classic);
        (RadioButtonStandardAdapter adapter, _) = CreateAdapter();

        ButtonBaseAdapter result = adapter.TestAccessor.Dynamic.CreateButtonAdapter();

        result.Should().BeOfType<ButtonStandardAdapter>();
    }

    [WinFormsFact]
    public void CreateButtonAdapter_InDarkMode_ReturnsButtonDarkModeAdapter()
    {
        if (SystemInformation.HighContrast)
        {
            return;
        }

        Application.SetColorMode(SystemColorMode.Dark);
        (RadioButtonStandardAdapter adapter, _) = CreateAdapter();

        ButtonBaseAdapter result = adapter.TestAccessor.Dynamic.CreateButtonAdapter();

        result.Should().BeOfType<ButtonDarkModeAdapter>();
    }

    [WinFormsTheory]
    [InlineData(Appearance.Button, true)]
    [InlineData(Appearance.Button, false)]
    [InlineData(Appearance.Normal, true)]
    [InlineData(Appearance.Normal, false)]
    public void PaintUp_InDarkMode_DoesNotThrow(Appearance appearance, bool isChecked)
    {
        if (SystemInformation.HighContrast)
        {
            return;
        }

        Application.SetColorMode(SystemColorMode.Dark);
        (RadioButtonStandardAdapter adapter, RadioButton radioButton) = CreateAdapter(appearance);
        radioButton.Checked = isChecked;

        using Bitmap bitmap = new(radioButton.Width, radioButton.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, radioButton.ClientRectangle);

        Exception? exception = Record.Exception(
            () => adapter.PaintUp(e, isChecked ? CheckState.Checked : CheckState.Unchecked));

        exception.Should().BeNull();
    }

    [WinFormsTheory]
    [InlineData(Appearance.Button, true)]
    [InlineData(Appearance.Button, false)]
    [InlineData(Appearance.Normal, true)]
    [InlineData(Appearance.Normal, false)]
    public void PaintDown_InDarkMode_DoesNotThrow(Appearance appearance, bool isChecked)
    {
        if (SystemInformation.HighContrast)
        {
            return;
        }

        Application.SetColorMode(SystemColorMode.Dark);
        (RadioButtonStandardAdapter adapter, RadioButton radioButton) = CreateAdapter(appearance);
        radioButton.Checked = isChecked;

        using Bitmap bitmap = new(radioButton.Width, radioButton.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, radioButton.ClientRectangle);

        Exception? exception = Record.Exception(
            () => adapter.PaintDown(e, isChecked ? CheckState.Checked : CheckState.Unchecked));

        exception.Should().BeNull();
    }
}
