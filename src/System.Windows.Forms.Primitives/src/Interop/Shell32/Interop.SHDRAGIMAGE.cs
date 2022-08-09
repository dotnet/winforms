// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Shell32
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct SHDRAGIMAGE
        {
            public Size sizeDragImage;
            public Point ptOffset;
            public HBITMAP hbmpDragImage;
            public COLORREF crColorKey;
        }
    }
}
