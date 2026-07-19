// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

/// <summary>
///  Temporarily overrides system visual settings for deterministic renderer tests.
/// </summary>
internal sealed class SystemVisualSettingsTestScope : IDisposable
{
    private readonly SystemVisualSettings _previousSettings = SystemVisualSettingsTracker.CurrentSettings;

    public SystemVisualSettingsTestScope(
        bool clientAreaAnimationEnabled,
        bool? highContrastEnabled = null)
    {
        SystemVisualSettingsTracker.ResetForTesting(
            new SystemVisualSettings(
                _previousSettings.AccentColor,
                _previousSettings.TextScaleFactor,
                highContrastEnabled
                    ?? _previousSettings.HighContrastEnabled,
                clientAreaAnimationEnabled,
                _previousSettings.KeyboardCuesVisible,
                _previousSettings.FocusBorderMetrics));
    }

    public void Dispose()
        => SystemVisualSettingsTracker.ResetForTesting(_previousSettings);
}
