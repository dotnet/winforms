// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class UiaCore
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct UiaRect
        {
            public double left;
            public double top;
            public double width;
            public double height;

            public UiaRect(Rectangle r)
            {
                left = r.Left;
                top = r.Top;
                width = r.Width;
                height = r.Height;
            }
        }
    }
}
