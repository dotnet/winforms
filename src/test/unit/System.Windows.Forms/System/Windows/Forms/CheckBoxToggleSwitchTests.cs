// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;

namespace System.Windows.Forms.Tests;

// Behavioral tests for the modern toggle-switch CheckBox (Appearance.ToggleSwitch + VisualStylesMode.Net11).
// These use only public surface so they remain stable; the rendering/animation itself is verified through
// the WinformsControlsTest exploratory harness.
public class CheckBoxToggleSwitchTests
{
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
    public void CheckBox_VisualStylesMode_DefaultIsAmbient()
    {
        using CheckBox checkBox = new();

        Assert.Equal(Application.DefaultVisualStylesMode, checkBox.VisualStylesMode);
    }
}
