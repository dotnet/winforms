// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {   
        public static BOOL GetTextExtentPoint32W<T>(T hdc, string lpString, int c, out Size size) where T : IHandle<HDC>
        {
            BOOL result = GetTextExtentPoint32W(hdc.Handle, lpString, c, out size);
            GC.KeepAlive(hdc.Wrapper);
            return result;
        }
    }
}
