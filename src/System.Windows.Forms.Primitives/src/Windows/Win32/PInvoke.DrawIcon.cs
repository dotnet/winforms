// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static BOOL DrawIcon<T>(HDC hDC, int x, int y, T hIcon)
            where T : IHandle<HICON>
        {
            BOOL result = DrawIcon(hDC, x, y, hIcon.Handle);
            GC.KeepAlive(hIcon.Wrapper);
            return result;
        }
    }
}
