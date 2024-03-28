// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32.UI.Accessibility
{
    internal partial struct UiaPoint
    {
        public static implicit operator Point(UiaPoint value) => new((int)value.x, (int)value.y);

        public static implicit operator UiaPoint(Point value) => new() { x = value.X, y = value.Y };
    }
}
