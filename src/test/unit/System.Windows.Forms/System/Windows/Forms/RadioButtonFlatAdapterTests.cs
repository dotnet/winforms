// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using System.Windows.Forms.ButtonInternal;
using static System.Windows.Forms.ButtonInternal.ButtonBaseAdapter;

namespace System.Windows.Forms.Tests;

public class RadioButtonFlatAdapterTests : IDisposable
{
    private RadioButton? _radioButton;

    private (RadioButtonFlatAdapter Adapter, RadioButton Control) CreateAdapter(
        Appearance appearance = Appearance.Normal,
        bool enabled = true,
        bool @checked = false)
    {
        _radioButton?.Dispose();
        _radioButton = new RadioButton
        {
            Appearance = appearance,
            Enabled = enabled,
            Checked = @checked,
            Size = new Size(100, 30),
            Text = "Radio"
        };

        return (new RadioButtonFlatAdapter(_radioButton), _radioButton);
    }

    public void Dispose() => _radioButton?.Dispose();

    [WinFormsTheory]
    [InlineData(Appearance.Button, true, true)]
    [InlineData(Appearance.Button, true, false)]
    [InlineData(Appearance.Button, false, true)]
    [InlineData(Appearance.Button, false, false)]
    [InlineData(Appearance.Normal, true, true)]
    [InlineData(Appearance.Normal, true, false)]
    [InlineData(Appearance.Normal, false, true)]
    [InlineData(Appearance.Normal, false, false)]
    public void PaintDown_DoesNotThrow(Appearance appearance, bool enabled, bool @checked)
    {
        (RadioButtonFlatAdapter adapter, RadioButton control) = CreateAdapter(appearance, enabled, @checked);
        using Bitmap bitmap = new(control.Width, control.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, control.ClientRectangle);

        Action action = () => adapter.PaintDown(e, control.Checked ? CheckState.Checked : CheckState.Unchecked);

        action.Should().NotThrow();
    }

    [WinFormsTheory]
    [InlineData(Appearance.Button, true, true)]
    [InlineData(Appearance.Button, true, false)]
    [InlineData(Appearance.Button, false, true)]
    [InlineData(Appearance.Button, false, false)]
    [InlineData(Appearance.Normal, true, true)]
    [InlineData(Appearance.Normal, true, false)]
    [InlineData(Appearance.Normal, false, true)]
    [InlineData(Appearance.Normal, false, false)]
    public void PaintOver_DoesNotThrow(Appearance appearance, bool enabled, bool @checked)
    {
        (RadioButtonFlatAdapter adapter, RadioButton control) = CreateAdapter(appearance, enabled, @checked);
        using Bitmap bitmap = new(control.Width, control.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, control.ClientRectangle);

        Action action = () => adapter.PaintOver(e, control.Checked ? CheckState.Checked : CheckState.Unchecked);

        action.Should().NotThrow();
    }

    [WinFormsTheory]
    [InlineData(Appearance.Button, true, true)]
    [InlineData(Appearance.Button, true, false)]
    [InlineData(Appearance.Button, false, true)]
    [InlineData(Appearance.Button, false, false)]
    [InlineData(Appearance.Normal, true, true)]
    [InlineData(Appearance.Normal, true, false)]
    [InlineData(Appearance.Normal, false, true)]
    [InlineData(Appearance.Normal, false, false)]
    public void PaintUp_DoesNotThrow(Appearance appearance, bool enabled, bool @checked)
    {
        (RadioButtonFlatAdapter adapter, RadioButton control) = CreateAdapter(appearance, enabled, @checked);
        using Bitmap bitmap = new(control.Width, control.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, control.ClientRectangle);

        Action action = () => adapter.PaintUp(e, control.Checked ? CheckState.Checked : CheckState.Unchecked);

        action.Should().NotThrow();
    }

    [WinFormsFact]
    public void CreateButtonAdapter_ReturnsButtonFlatAdapter()
    {
        (RadioButtonFlatAdapter adapter, _) = CreateAdapter();

        ButtonBaseAdapter result = adapter.TestAccessor.Dynamic.CreateButtonAdapter();

        result.Should().NotBeNull();
        result.Should().BeOfType<ButtonFlatAdapter>();
    }

    [WinFormsFact]
    public void Layout_SetsCheckSizeAndDisablesShadowedText()
    {
        (RadioButtonFlatAdapter adapter, RadioButton control) = CreateAdapter();
        using Bitmap bitmap = new(control.Width, control.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, control.ClientRectangle);

        LayoutOptions layout = adapter.TestAccessor.Dynamic.Layout(e);

        layout.Should().NotBeNull();
        layout.CheckSize.Should().BeGreaterThan(0);
        layout.ShadowedText.Should().BeFalse();
    }

    [WinFormsFact]
    public void Layout_CheckSize_MatchesFlatCheckSizeScaledByDpi()
    {
        (RadioButtonFlatAdapter adapter, RadioButton control) = CreateAdapter();
        using Bitmap bitmap = new(control.Width, control.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, control.ClientRectangle);

        LayoutOptions layout = adapter.TestAccessor.Dynamic.Layout(e);
        double dpiScale = adapter.TestAccessor.Dynamic.GetDpiScaleRatio();
        int expectedCheckSize = (int)(12 * dpiScale);

        layout.CheckSize.Should().Be(expectedCheckSize);
    }

    [WinFormsTheory]
    [InlineData(true)]
    [InlineData(false)]
    public void PaintUp_NormalAppearance_WithTextAndImage_DoesNotThrow(bool enabled)
    {
        (RadioButtonFlatAdapter adapter, RadioButton control) = CreateAdapter(Appearance.Normal, enabled, @checked: true);
        control.Text = "Option";
        using Bitmap image = new(16, 16);
        using (Graphics g = Graphics.FromImage(image))
        {
            g.Clear(Color.Red);
        }

        control.Image = image;

        using Bitmap bitmap = new(control.Width, control.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, control.ClientRectangle);

        Action action = () => adapter.PaintUp(e, CheckState.Checked);

        action.Should().NotThrow();
    }

    [WinFormsTheory]
    [InlineData(CheckState.Unchecked)]
    [InlineData(CheckState.Checked)]
    public void PaintDown_NormalAppearance_CheckedStates_DoesNotThrow(CheckState state)
    {
        (RadioButtonFlatAdapter adapter, RadioButton control) = CreateAdapter(
            Appearance.Normal,
            enabled: true,
            @checked: state == CheckState.Checked);

        using Bitmap bitmap = new(control.Width, control.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, control.ClientRectangle);

        Action action = () => adapter.PaintDown(e, state);

        action.Should().NotThrow();
    }

    [WinFormsTheory]
    [InlineData(CheckState.Unchecked)]
    [InlineData(CheckState.Checked)]
    public void PaintOver_NormalAppearance_CheckedStates_DoesNotThrow(CheckState state)
    {
        (RadioButtonFlatAdapter adapter, RadioButton control) = CreateAdapter(
            Appearance.Normal,
            enabled: true,
            @checked: state == CheckState.Checked);

        using Bitmap bitmap = new(control.Width, control.Height);
        using Graphics graphics = Graphics.FromImage(bitmap);
        using PaintEventArgs e = new(graphics, control.ClientRectangle);

        Action action = () => adapter.PaintOver(e, state);

        action.Should().NotThrow();
    }
}
