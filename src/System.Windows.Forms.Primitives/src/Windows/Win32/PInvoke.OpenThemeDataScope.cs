// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32;

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
        public HTHEME HTheme { get; }

        /// <summary>
        ///  Opens the requested theme data using <see cref="OpenThemeData(HWND, string)"/>.
        /// </summary>
        public OpenThemeDataScope(HWND hwnd, string pszClassList)
        {
            HTheme = OpenThemeData(hwnd, pszClassList);
        }

        public static implicit operator HTHEME(in OpenThemeDataScope scope) => scope.HTheme;

        public bool IsNull => HTheme.IsNull;

        public void Dispose()
        {
            if (!HTheme.IsNull)
            {
                CloseThemeData(HTheme);
            }

            DisposalTracking.SuppressFinalize(this);
        }
    }
}
