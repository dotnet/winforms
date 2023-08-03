// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Layout;

internal partial class DefaultLayout
{
    [Flags]
    private enum GrowthDirection
    {
        None = 0,
        Upward = 0x01,
        Downward = 0x02,
        Left = 0x04,
        Right = 0x08
    }
}
