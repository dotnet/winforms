﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        /// <summary>
        ///  Sets the thread DPI awareness context.
        /// </summary>
        /// <returns>
        ///  The old thread DPI awareness context if the API is available in this version of OS.
        ///  Otherwise, <see cref="DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT"/>.
        /// </returns>
        public static DPI_AWARENESS_CONTEXT SetThreadDpiAwarenessContextInternal(DPI_AWARENESS_CONTEXT dpiContext)
        {
            if (OsVersion.IsWindows10_1607OrGreater())
            {
                if (dpiContext == DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT)
                {
                    throw new ArgumentException(nameof(dpiContext), dpiContext.ToString());
                }

                return SetThreadDpiAwarenessContext(dpiContext);
            }

            // legacy OS that does not have this API available.
            return DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT;
        }
    }
}
