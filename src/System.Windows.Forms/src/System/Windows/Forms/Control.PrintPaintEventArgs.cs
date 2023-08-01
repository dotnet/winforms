// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class Control
{
    private sealed class PrintPaintEventArgs : PaintEventArgs
    {
        internal Message Message { get; }

        internal PrintPaintEventArgs(Message m, HDC dc, Rectangle clipRect)
            : base(dc, clipRect, DrawingEventFlags.SaveState)
        {
            Message = m;
        }
    }
}
