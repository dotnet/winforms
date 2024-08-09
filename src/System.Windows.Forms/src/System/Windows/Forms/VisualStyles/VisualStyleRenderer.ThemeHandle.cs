// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.VisualStyles;

public sealed partial class VisualStyleRenderer
{
    // This wrapper class is needed for safely cleaning up TLS cache of handles.
    private class ThemeHandle : IDisposable, IHandle<HTHEME>
    {
        private ThemeHandle(HTHEME hTheme)
        {
            Handle = hTheme;
        }

        public HTHEME Handle { get; private set; }

        public static ThemeHandle? Create(string className, bool throwExceptionOnFail)
        {
            return Create(className, throwExceptionOnFail, HWND.Null);
        }

        internal static ThemeHandle? Create(string className, bool throwExceptionOnFail, HWND hWndRef)
        {
            // HThemes require an HWND when display scaling is different between monitors.
            HTHEME hTheme = PInvoke.OpenThemeData(hWndRef, className);

            return hTheme.IsNull
                ? throwExceptionOnFail ? throw new InvalidOperationException(SR.VisualStyleHandleCreationFailed) : null
                : new ThemeHandle(hTheme);
        }

        public void Dispose()
        {
            if (!Handle.IsNull)
            {
                PInvoke.CloseThemeData(Handle);
                Handle = HTHEME.Null;
            }

            GC.SuppressFinalize(this);
        }

        ~ThemeHandle() => Dispose();
    }
}
