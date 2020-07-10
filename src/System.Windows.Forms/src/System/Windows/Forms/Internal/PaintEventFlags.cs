// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    [Flags]
    internal enum PaintEventFlags : ushort
    {
        /// <summary>
        ///  If true, consider the <see cref="Graphics"/> object to potentially have a clip or transform applied.
        /// </summary>
        GraphicsStateUnclean        = 0b0000_0001,
        SaveState                   = 0b0000_0010,
        CheckState                  = 0b0000_0100
    }
}
