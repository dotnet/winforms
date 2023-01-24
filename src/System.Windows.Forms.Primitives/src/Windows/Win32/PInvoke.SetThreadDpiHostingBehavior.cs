// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        /// <summary>
        ///  Sets the thread DPI hosting behavior.
        /// </summary>
        /// <returns>
        ///  The old thread DPI hosting behavior if the API is available in this version of OS.
        ///  Otherwise, <see cref="DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_DEFAULT"/>.
        /// </returns>
        public static DPI_HOSTING_BEHAVIOR SetThreadDpiHostingBehaviorInternal(DPI_HOSTING_BEHAVIOR dpiHostingBehavior)
        {
            if (dpiHostingBehavior == DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_MIXED)
            {
                // Winforms only affects the behavior of thread hosting in mixed DPI mode situations, and does not apply to any other scenarios.
                throw new ArgumentException(nameof(dpiHostingBehavior), dpiHostingBehavior.ToString());
            }

            if (OsVersion.IsWindows10_18030rGreater())
            {
                if (dpiHostingBehavior == DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_MIXED)
                {
                    return SetThreadDpiHostingBehavior(dpiHostingBehavior);
                }
            }

            return DPI_HOSTING_BEHAVIOR.DPI_HOSTING_BEHAVIOR_INVALID;
        }
    }
}
