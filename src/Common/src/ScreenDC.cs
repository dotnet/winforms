// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Simple wrapper to create a screen HDC within a using statement.
    ///  This will return the DC for the primary monitor. (Not the entire desktop.)
    /// </summary>
    /// <remarks>
    ///   <see cref="Interop.Gdi32.CreateDC(string, string, string, IntPtr)" /> is
    ///   the API to get the DC for the entire desktop.
    /// </remarks>
    internal ref struct ScreenDC
    {
        private IntPtr _handle;

        public static ScreenDC Create()
            => new ScreenDC { _handle = Interop.User32.GetDC(IntPtr.Zero) };

        public static implicit operator IntPtr(ScreenDC screenDC) => screenDC._handle;

        public void Dispose()
        {
            if (_handle != IntPtr.Zero)
            {
                Interop.User32.ReleaseDC(IntPtr.Zero, _handle);
            }
        }
    }
}
