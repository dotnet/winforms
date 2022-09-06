// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        /// <summary>
        ///  Tries to compare two DPI awareness context values. Returns true if they are equal.
        ///  Returns false if they are not equal or the underlying OS does not support this API.
        /// </summary>
        /// <returns>
        /// This method returns true if they are equal.
        /// Otherwise, it returns false if they are not equal or the underlying OS does not support this API.
        /// </returns>
        public static bool AreDpiAwarenessContextsEqualInternal(DPI_AWARENESS_CONTEXT dpiContextA, DPI_AWARENESS_CONTEXT dpiContextB)
        {
            if (dpiContextA == DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT && dpiContextB == DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT)
            {
                return true;
            }

            if (OsVersion.IsWindows10_1607OrGreater)
            {
                return AreDpiAwarenessContextsEqual(dpiContextA, dpiContextB);
            }

            return false;
        }
    }
}
