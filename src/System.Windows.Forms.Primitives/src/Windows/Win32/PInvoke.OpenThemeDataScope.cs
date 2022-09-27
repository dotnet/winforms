// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        /// <summary>
        ///  Helper to scope the lifetime of a an HTHEME.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   Use in a <see langword="using" /> statement. If you must pass this around, always pass by
        ///   <see langword="ref" /> to avoid duplicating the handle and risking a double close.
        ///  </para>
        /// </remarks>
#if DEBUG
        internal class OpenThemeDataScope : DisposalTracking.Tracker, IDisposable
#else
        internal readonly ref struct OpenThemeDataScope
#endif
        {
            public nint HTheme { get; }

            /// <summary>
            ///  Opens the requested theme data using <see cref="OpenThemeData(HWND, string)"/>.
            /// </summary>
            public OpenThemeDataScope(HWND hwnd, string pszClassList, bool throwOnError = false)
            {
                HTheme = OpenThemeData(hwnd, pszClassList);
            }

            public static implicit operator nint(in OpenThemeDataScope scope) => scope.HTheme;

            public bool IsNull => HTheme == 0;

            public void Dispose()
            {
                if (HTheme != 0)
                {
                    CloseThemeData(HTheme);
                }

                DisposalTracking.SuppressFinalize(this);
            }
        }
    }
}
