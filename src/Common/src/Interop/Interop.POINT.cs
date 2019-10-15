// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    /// <summary>
    /// Represetns the point structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        internal int x;
        internal int y;

        internal POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        static public explicit operator POINT(Point pt)
        {
            return checked(new POINT((int)pt.X, (int)pt.Y));
        }
    }
}
