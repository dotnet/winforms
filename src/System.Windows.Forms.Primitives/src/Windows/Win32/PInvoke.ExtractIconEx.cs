// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        internal static unsafe Icon? ExtractIconEx(string? lpszFile)
        {
            if (string.IsNullOrWhiteSpace(lpszFile))
            {
                return null;
            }

            HICON iconSmall = default;
            uint readIconCount = 0;
            fixed (char* lpszFileLocal = lpszFile)
            {
                readIconCount = ExtractIconEx(lpszFile: lpszFileLocal, nIconIndex: 0, phiconSmall: &iconSmall, nIcons: 1);
            }

            return readIconCount > 0 && !iconSmall.IsNull ? (Icon)Icon.FromHandle(iconSmall).Clone() : null;
        }
    }
}
