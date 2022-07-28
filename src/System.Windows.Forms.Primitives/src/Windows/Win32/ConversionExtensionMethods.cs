// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Windows.Win32.Foundation;

namespace Windows.Win32
{
    internal static class ConversionExtensionMethods
    {
        public static Rectangle ToRectangle(this in RECT rect) => Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);

        public static int Width(this in RECT rect) => rect.right - rect.left;

        public static int Height(this in RECT rect) => rect.bottom - rect.top;

        public static RECT ToRect(this in Size size) => new() { right = size.Width, bottom = size.Height };

        public static Size Size(this in RECT rect) => new(rect.Width(), rect.Height());
    }
}
