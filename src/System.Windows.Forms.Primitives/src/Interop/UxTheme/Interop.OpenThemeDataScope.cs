// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    public static partial class UxTheme
    {
        /// <summary>
        ///  Helper to scope the lifetime of a an HTHEME.
        /// </summary>
        /// <remarks>
        ///  Use in a <see langword="using" /> statement. If you must pass this around, always pass by
        ///  <see langword="ref" /> to avoid duplicating the handle and risking a double close.
        /// </remarks>
#if DEBUG
        internal class OpenThemeDataScope : DisposalTracking.Tracker, IDisposable
#else
        internal readonly ref struct OpenThemeDataScope
#endif
        {
            public IntPtr HTheme { get; }

            /// <summary>
            ///  Opens the requested theme data using <see cref="OpenThemeData(IntPtr, string)"/>.
            /// </summary>
            public OpenThemeDataScope(IntPtr hwnd, string pszClassList)
            {
                HTheme = OpenThemeData(hwnd, pszClassList);
            }

            public static implicit operator IntPtr(in OpenThemeDataScope scope) => scope.HTheme;

            public bool IsNull => HTheme == IntPtr.Zero;

            public void Dispose()
            {
                if (HTheme != IntPtr.Zero)
                {
                    CloseThemeData(HTheme);
                }

                DisposalTracking.SuppressFinalize(this);
            }
        }
    }
}
