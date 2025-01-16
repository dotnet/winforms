// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace System;

#pragma warning disable WFO5001
/// <summary>
///  Scope for setting the default color mode (dark mode) for the application. Use in a <see langword="using"/> statement.
/// </summary>
public readonly ref struct ApplicationColorModeScope
{
    private readonly SystemColorMode _originalColorMode;

    public ApplicationColorModeScope(SystemColorMode colorMode)
    {
        _originalColorMode = Application.ColorMode;

        if (_originalColorMode != colorMode)
        {
            Application.SetColorMode(colorMode);
        }
    }

    public void Dispose()
    {
        if (Application.ColorMode != _originalColorMode)
        {
            Application.SetColorMode(_originalColorMode);
        }
    }
}
#pragma warning restore WFO5001
