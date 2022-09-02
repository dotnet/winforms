﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        /// <summary>
        ///  Tries to set thread dpi awareness context
        /// </summary>
        /// <returns>
        /// Returns old thread dpi awareness context if API is available in this version of OS.
        /// Otherwise, return <see cref="DPI_AWARENESS_CONTEXT.UNSPECIFIED_DPI_AWARENESS_CONTEXT"/>.
        /// </returns>
        public static DPI_AWARENESS_CONTEXT SetThreadDpiAwarenessContextInternal(DPI_AWARENESS_CONTEXT dpiContext)
        {
            if (OsVersion.IsWindows10_1607OrGreater)
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
