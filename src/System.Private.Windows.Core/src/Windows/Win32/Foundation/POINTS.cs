// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32.Foundation;

internal partial struct POINTS
{
    public static implicit operator Point(POINTS point) => new(point.x, point.y);
}
