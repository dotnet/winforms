// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32;

internal static partial class PInvokeCore
{
    private static HBRUSH s_solidBrush;
    private static HBRUSH s_lastSolidBruch;

    /// <summary>
    /// Returns a system color brush for the given color.
    /// If using an alternative color set, manages solid brush creation and cleanup to prevent GDI leaks.
    /// </summary>
    /// <param name="systemColor">The system color.</param>
    /// <returns>An HBRUSH for the color.</returns>
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
            // Create a new solid brush for the alternative color set.
            s_solidBrush = CreateSolidBrush(systemColor);

            // Delete the previous solid brush to avoid GDI leaks.
            if (s_lastSolidBruch != s_solidBrush)
            {
                if (s_lastSolidBruch != default)
                {
                    DeleteObject(s_lastSolidBruch);
                }

                s_lastSolidBruch = s_solidBrush;
            }

            return s_solidBrush;
        }

        Debug.Assert(systemColor.IsSystemColor);

        // Clean up any previously created solid brushes to prevent GDI leaks.
        if (s_solidBrush != default || s_lastSolidBruch != default)
        {
            DeleteObject(s_solidBrush);
            DeleteObject(s_lastSolidBruch);
            s_solidBrush = default;
            s_lastSolidBruch = default;
        }

        // Return the system color brush for the specified system color.
        return GetSysColorBrush((SYS_COLOR_INDEX)(ColorTranslator.ToOle(systemColor) & 0xFF));
    }
}
