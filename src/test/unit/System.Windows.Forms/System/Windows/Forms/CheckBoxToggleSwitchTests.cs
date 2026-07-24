// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms.Tests;

// Behavioral tests for the modern toggle-switch CheckBox (Appearance.ToggleSwitch + VisualStylesMode.Net11).
// These cover public behavior and the shared metrics without depending on pixel-level rendering details.
public class CheckBoxToggleSwitchTests
{
    private const int LeftMouseButtonKeyState = 0x0001;

    public static IEnumerable<object[]> RenderingMatrixData()
    {
        foreach (FlatStyle flatStyle in Enum.GetValues<FlatStyle>())
        {
            foreach (Appearance appearance in Enum.GetValues<Appearance>())
            {
                yield return [flatStyle, appearance, VisualStylesMode.Inherit];
                yield return [flatStyle, appearance, VisualStylesMode.Classic];
                yield return [flatStyle, appearance, VisualStylesMode.Disabled];
                yield return [flatStyle, appearance, VisualStylesMode.Net11];
                yield return [flatStyle, appearance, VisualStylesMode.Latest];
            }
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(RenderingMatrixData))]
    public void CheckBox_RenderingMatrix_DoesNotThrow(
        FlatStyle flatStyle,
        Appearance appearance,
        VisualStylesMode visualStylesMode)
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using CheckBox checkBox = new()
        {
            Appearance = appearance,
            FlatStyle = flatStyle,
            Size = new Size(140, 36),
            Text = "Matrix",
            VisualStylesMode = visualStylesMode
        };
        checkBox.CreateControl();
        using Bitmap bitmap = new(
            checkBox.Width,
            checkBox.Height);

        _ = checkBox.DownChangeRectangle;
        _ = checkBox.OverChangeRectangle;
        checkBox.DrawToBitmap(
            bitmap,
            new Rectangle(Point.Empty, checkBox.Size));
        PInvokeCore.SendMessage(
            checkBox,
            PInvokeCore.WM_LBUTTONDOWN,
            (WPARAM)LeftMouseButtonKeyState,
            LPARAM.MAKELPARAM(5, 5));
        PInvokeCore.SendMessage(
            checkBox,
            PInvokeCore.WM_LBUTTONUP,
            0,
            LPARAM.MAKELPARAM(5, 5));
    }

    [WinFormsTheory]
    [InlineData(8)]
    [InlineData(9)]
    [InlineData(10)]
    [InlineData(11)]
    public void CheckBox_ToggleSwitch_FontSize_DerivesPreferredGeometry(float fontSize)
    {
        using Font font = new(Control.DefaultFont.FontFamily, fontSize);
        using CheckBox checkBox = new()
        {
            VisualStylesMode = VisualStylesMode.Net11,
            Appearance = Appearance.ToggleSwitch,
            Font = font,
            Text = "Toggle",
            Padding = new Padding(2)
        };

        Rendering.CheckBox.ToggleSwitchMetrics metrics = Rendering.CheckBox.ToggleSwitchMetrics.Create(checkBox);
        int expectedHeight = Math.Max(
            checkBox.LogicalToDeviceUnits(13),
            (int)(font.Height * 0.9f));

        Assert.Equal(expectedHeight, metrics.SwitchHeight);
        Assert.Equal(2 * expectedHeight, metrics.SwitchWidth);
        Assert.True(metrics.HoverThumbDiameter > metrics.ThumbDiameter);
        Assert.Equal(metrics.GetPreferredSize(checkBox), checkBox.GetPreferredSize(Size.Empty));
    }

    [WinFormsFact]
    public void CheckBox_ToggleSwitch_LargeFont_ReservesTenPercentHoverGrowth()
    {
        using Font font = new(Control.DefaultFont.FontFamily, 72f);
        using CheckBox checkBox = new()
        {
            Appearance = Appearance.ToggleSwitch,
            Font = font,
            VisualStylesMode = VisualStylesMode.Net11
        };

        Rendering.CheckBox.ToggleSwitchMetrics metrics = Rendering.CheckBox.ToggleSwitchMetrics.Create(checkBox);
        float growth = metrics.HoverThumbDiameter / (float)metrics.ThumbDiameter;

        Assert.InRange(growth, 1.08f, 1.12f);
    }

    [WinFormsFact]
    public void CheckBox_Appearance_ToggleSwitch_RoundTrips()
    {
        using CheckBox checkBox = new() { Appearance = Appearance.ToggleSwitch };

        Assert.Equal(Appearance.ToggleSwitch, checkBox.Appearance);
    }

    [WinFormsTheory]
    [InlineData(VisualStylesMode.Net11)]
    [InlineData(VisualStylesMode.Latest)]
    public void CheckBox_ToggleSwitch_WithModernVisualStyles_HasPositivePreferredSize(VisualStylesMode visualStylesMode)
    {
        using CheckBox checkBox = new()
        {
            VisualStylesMode = visualStylesMode,
            Appearance = Appearance.ToggleSwitch,
            Text = "Toggle"
        };

        Size preferred = checkBox.GetPreferredSize(Size.Empty);

        Assert.True(preferred.Width > 0);
        Assert.True(preferred.Height > 0);
    }

    [WinFormsTheory]
    [InlineData(VisualStylesMode.Classic)]
    [InlineData(VisualStylesMode.Disabled)]
    public void CheckBox_ToggleSwitch_WithoutModernVisualStyles_UsesClassicPreferredSize(VisualStylesMode visualStylesMode)
    {
        using CheckBox classicCheckBox = new()
        {
            VisualStylesMode = visualStylesMode,
            Appearance = Appearance.Normal,
            Text = "Toggle"
        };

        using CheckBox toggleSwitchCheckBox = new()
        {
            VisualStylesMode = visualStylesMode,
            Appearance = Appearance.ToggleSwitch,
            Text = "Toggle"
        };

        Assert.Equal(
            classicCheckBox.GetPreferredSize(Size.Empty),
            toggleSwitchCheckBox.GetPreferredSize(Size.Empty));
    }

    [WinFormsFact]
    public void CheckBox_ToggleSwitch_CheckedRoundTrips()
    {
        using CheckBox checkBox = new()
        {
            VisualStylesMode = VisualStylesMode.Net11,
            Appearance = Appearance.ToggleSwitch
        };

        Assert.False(checkBox.Checked);

        checkBox.Checked = true;
        Assert.True(checkBox.Checked);
        Assert.Equal(CheckState.Checked, checkBox.CheckState);

        checkBox.Checked = false;
        Assert.False(checkBox.Checked);
        Assert.Equal(CheckState.Unchecked, checkBox.CheckState);
    }

    [WinFormsFact]
    public void CheckBox_ToggleSwitch_CheckStateChanged_Raised()
    {
        using CheckBox checkBox = new()
        {
            VisualStylesMode = VisualStylesMode.Net11,
            Appearance = Appearance.ToggleSwitch
        };

        int callCount = 0;
        checkBox.CheckStateChanged += (s, e) => callCount++;

        checkBox.CheckState = CheckState.Checked;
        Assert.Equal(1, callCount);

        // Setting the same value does not raise the event.
        checkBox.CheckState = CheckState.Checked;
        Assert.Equal(1, callCount);
    }

    [WinFormsFact]
    public void CheckBox_ToggleSwitch_ThreeState_PreservesClassicCheckStateBehavior()
    {
        using CheckBox checkBox = new()
        {
            VisualStylesMode = VisualStylesMode.Net11,
            Appearance = Appearance.ToggleSwitch,
            ThreeState = true
        };

        checkBox.CheckState = CheckState.Indeterminate;

        Assert.Equal(CheckState.Indeterminate, checkBox.CheckState);
        Assert.True(checkBox.GetPreferredSize(Size.Empty).Width > 0);
    }

    [WinFormsFact]
    public void CheckBox_VisualStylesMode_DefaultIsAmbient()
    {
        using CheckBox checkBox = new();

        Assert.Equal(VisualStylesMode.Inherit, checkBox.VisualStylesMode);
    }
}
