// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    [Flags]
    internal enum DrawingEventFlags : ushort
    {
        /// <summary>
        ///  If true, consider the <see cref="Graphics"/> object to potentially have a clip or transform applied.
        /// </summary>
        GraphicsStateUnclean        = 0b0000_0001,

        /// <summary>
        ///  Only used in <see cref="PaintEventArgs"/>. Saves the state of the <see cref="Graphics"/> at construction
        ///  so it can be restored via <see cref="PaintEventArgs.ResetGraphics"/>.
        /// </summary>
        SaveState                   = 0b0000_0010,

        /// <summary>
        ///  If true, will validate the state stays clean of transforms and clipping in debug builds. This is the
        ///  default when constructing via an <see cref="Gdi32.HDC"/> as it, by definition, has no <see cref="Graphics"/>
        ///  state.
        /// </summary>
        CheckState                  = 0b0000_0100
    }
}
