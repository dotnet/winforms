// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static int FillRect<T>(T hDC, ref RECT lprc, HBRUSH hbr)
            where T : IHandle<HDC>
        {
            int result = FillRect(hDC.Handle, ref lprc, hbr);
            GC.KeepAlive(hDC.Wrapper);
            return result;
        }
    }
}
