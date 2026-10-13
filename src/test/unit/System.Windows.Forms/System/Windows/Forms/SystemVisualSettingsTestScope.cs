// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Tests;

/// <summary>
///  Temporarily overrides system visual settings for deterministic renderer tests.
/// </summary>
internal sealed class SystemVisualSettingsTestScope : IDisposable
{
    private readonly SystemVisualSettings _previousSettings = SystemVisualSettingsTracker.CurrentSettings;

    public SystemVisualSettingsTestScope(
        bool clientAreaAnimationEnabled,
        bool highContrastEnabled = false,
        Color? accentColor = null,
        float textScaleFactor = 1f,
        Size? focusBorderMetrics = null)
    {
        SystemVisualSettingsTracker.ResetForTesting(
            new SystemVisualSettings(
                accentColor ?? Color.DodgerBlue,
                textScaleFactor,
                highContrastEnabled,
                clientAreaAnimationEnabled,
                _previousSettings.KeyboardCuesVisible,
                focusBorderMetrics ?? new Size(1, 1)));
    }

    public void Dispose()
        => SystemVisualSettingsTracker.ResetForTesting(_previousSettings);
}
