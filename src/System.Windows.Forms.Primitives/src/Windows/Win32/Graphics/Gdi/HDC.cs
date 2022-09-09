// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32.Graphics.Gdi
{
    internal readonly partial struct HDC
    {
        public static implicit operator HDC(CreatedHDC hdc) => new(hdc.Value);
        public static implicit operator CreatedHDC(HDC hdc) => new(hdc.Value);
    }
}
