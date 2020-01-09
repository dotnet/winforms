// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    public partial class Control
    {
        private sealed class PrintPaintEventArgs : PaintEventArgs
        {
            private Message _m;

            internal PrintPaintEventArgs(Message m, IntPtr dc, Rectangle clipRect)
                : base(dc, clipRect)
            {
                _m = m;
            }

            internal Message Message
            {
                get
                {
                    return _m;
                }
            }
        }
    }
}
