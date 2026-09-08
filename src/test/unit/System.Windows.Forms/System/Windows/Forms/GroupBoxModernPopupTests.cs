// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Rendering.Button;

namespace System.Windows.Forms.Tests;

public class GroupBoxModernPopupTests
{
    [WinFormsFact]
    public void GroupBox_ModernStandard_WithCaptionTextScaleChangeRemeasuresParent()
    {
        SystemVisualSettings previous = SystemVisualSettingsTracker.CurrentSettings;
        SystemVisualSettings initial = new(
            previous.AccentColor,
            1f,
            highContrastEnabled: false,
            previous.ClientAreaAnimationEnabled,
            previous.KeyboardCuesVisible,
            previous.FocusBorderMetrics);
        SystemVisualSettings scaled = new(
            previous.AccentColor,
            1.5f,
            highContrastEnabled: false,
            previous.ClientAreaAnimationEnabled,
            previous.KeyboardCuesVisible,
            previous.FocusBorderMetrics);

        try
        {
            SystemVisualSettingsTracker.ResetForTesting(initial);
            using Panel parent = new();
            using DpiGroupBox control = new()
            {
                FlatStyle = FlatStyle.Standard,
                Text = "Header",
                VisualStylesMode = VisualStylesMode.Net11
            };
            parent.Controls.Add(control);
            int originalTop = control.DisplayRectangle.Top;
            int layoutCallCount = 0;
            parent.Layout += (sender, e) => layoutCallCount++;

            SystemVisualSettingsTracker.ResetForTesting(scaled);
            control.RaiseSystemVisualSettingsChanged(initial, scaled);

            Assert.True(control.DisplayRectangle.Top > originalTop);
            Assert.True(layoutCallCount > 0);
        }
        finally
        {
            SystemVisualSettingsTracker.ResetForTesting(previous);
        }
    }

    [WinFormsFact]
    public void GroupBox_ModernFlat_CaptionMetricsUseFontPixelHeightAtHighDpi()
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        using DpiGroupBox control = new()
        {
            FlatStyle = FlatStyle.Flat,
            Text = "Header",
            VisualStylesMode = VisualStylesMode.Net11
        };
        control.SetTestDeviceDpi(192);
        (int ascent, int descent) = control.ModernCaptionMetrics;

        Assert.InRange(ascent, 1, control.ModernCaptionHeight);
        Assert.InRange(ascent + descent, 1, control.ModernCaptionHeight + 1);
    }

    [WinFormsFact]
    public void GroupBox_ModernPopup_TransparentBackColorPaintsHeaderOverParent()
    {
        using SystemVisualSettingsTestScope settingsScope = new(
            clientAreaAnimationEnabled: false,
            highContrastEnabled: false);
        Color parentBackColor = Color.CornflowerBlue;
        using Panel parent = new()
        {
            BackColor = parentBackColor,
            Size = new Size(100, 70)
        };
        using GroupBox control = new()
        {
            BackColor = Color.Transparent,
            FlatStyle = FlatStyle.Popup,
            Size = parent.Size,
            Text = string.Empty,
            VisualStylesMode = VisualStylesMode.Net11
        };
        parent.Controls.Add(control);
        parent.CreateControl();
        control.CreateControl();
        using Bitmap actual = new(control.Width, control.Height);

        control.DrawToBitmap(
            actual,
            new Rectangle(Point.Empty, control.Size));

        Color expectedHeaderColor = PopupButtonColorMath.Blend(
            parentBackColor,
            Application.SystemVisualSettings.AccentColor,
            0.12f);
        Color actualHeaderColor = actual.GetPixel(
            actual.Width / 2,
            control.Font.Height / 2);

        Assert.Equal(expectedHeaderColor.ToArgb(), actualHeaderColor.ToArgb());
        Assert.NotEqual(parentBackColor.ToArgb(), actualHeaderColor.ToArgb());
    }

    private sealed class DpiGroupBox : GroupBox
    {
        public int ModernCaptionHeight
            => ((Font)this.TestAccessor.Dynamic.ModernCaptionFont).Height;

        public (int Ascent, int Descent) ModernCaptionMetrics
            => ((int, int))this.TestAccessor.Dynamic.GetModernCaptionMetrics();

        public void SetTestDeviceDpi(int deviceDpi)
            => DeviceDpiInternal = deviceDpi;

        public void RaiseSystemVisualSettingsChanged(
            SystemVisualSettings oldSettings,
            SystemVisualSettings newSettings)
            => OnSystemVisualSettingsChanged(
                new SystemVisualSettingsChangedEventArgs(
                    oldSettings,
                    newSettings,
                    SystemVisualSettingsCategories.TextScale));
    }
}
