// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace Windows.Win32.Foundation;

internal partial struct RECT
{
    public RECT(Size size)
    {
        right = size.Width;
        bottom = size.Height;
    }

    public override readonly string ToString() => ((Rectangle)this).ToString();
}
