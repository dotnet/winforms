// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class Control
    {
        private sealed class PrintPaintEventArgs : PaintEventArgs
        {
            internal Message Message { get; }

            internal PrintPaintEventArgs(Message m, Gdi32.HDC dc, Rectangle clipRect)
                : base(dc, clipRect, DrawingEventFlags.SaveState)
            {
                Message = m;
            }
        }
    }
}
