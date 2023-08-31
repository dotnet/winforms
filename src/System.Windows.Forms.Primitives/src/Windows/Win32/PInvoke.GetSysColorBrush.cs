// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32;

internal static partial class PInvoke
{
    public static HBRUSH GetSysColorBrush(Color systemColor)
    {
        Debug.Assert(systemColor.IsSystemColor);

        // Extract the COLOR value
        return PInvoke.GetSysColorBrush((SYS_COLOR_INDEX)(ColorTranslator.ToOle(systemColor) & 0xFF));
    }
}
