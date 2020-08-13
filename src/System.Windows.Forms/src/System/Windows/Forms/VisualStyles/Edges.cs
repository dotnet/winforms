// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms.VisualStyles
{
    [Flags]
    public enum Edges
    {
        Left = (int)User32.BF.LEFT,
        Top = (int)User32.BF.TOP,
        Right = (int)User32.BF.RIGHT,
        Bottom = (int)User32.BF.BOTTOM,
        Diagonal = (int)User32.BF.DIAGONAL,
    }
}
