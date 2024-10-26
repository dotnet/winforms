// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    /// <inheritdoc cref="GetSysColorBrush(SYS_COLOR_INDEX)"/>
    public static HBRUSH GetSysColorBrush(Color systemColor)
    {
#if NET9_0_OR_GREATER
#pragma warning disable SYSLIB5002 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        bool useSolidBrush = SystemColors.UseAlternativeColorSet;
#pragma warning restore SYSLIB5002
#else
        bool useSolidBrush = false;
#endif

        if (useSolidBrush)
        {
            // We don't have a real system color, so we'll just create a solid brush.
            return CreateSolidBrush(systemColor);
        }

        Debug.Assert(systemColor.IsSystemColor);

        // Extract the COLOR value
        return GetSysColorBrush((SYS_COLOR_INDEX)(ColorTranslator.ToOle(systemColor) & 0xFF));
    }
}
