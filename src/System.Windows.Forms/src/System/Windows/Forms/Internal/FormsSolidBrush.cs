// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This brush is roughly equivalent to <see cref="SolidBrush"/> with less overhead. It will not hook
    ///  <see cref="SystemEvents"/> to get updated when the user changes system colors.
    /// </summary>
    internal class FormsSolidBrush : Brush
    {
        public unsafe FormsSolidBrush(Color color)
        {
            Color = color;
            IntPtr nativeBrush;
            GdiPlus.GdipCreateSolidFill(color.ToArgb(), &nativeBrush).ThrowIfFailed();
            SetNativeBrush(nativeBrush);
        }

        public Color Color { get; }

        public override object Clone()
        {
            // Don't want to support this unless we really need it as we'll carry another (duplicate) IntPtr.
            throw new InvalidOperationException();

            // Here is how you clone if we ever end up needing to:
            //
            // GdiPlus.GdipCloneBrush(_handle, out IntPtr clonedBrush).ThrowIfFailed();
            // return new FormsSolidColorBrush(clonedBrush);
        }
    }
}
