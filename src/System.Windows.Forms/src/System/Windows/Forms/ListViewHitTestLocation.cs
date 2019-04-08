// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    [Flags]
    public enum ListViewHitTestLocations
    {
        None = NativeMethods.LVHT_NOWHERE,
        AboveClientArea = 0x0100,
        BelowClientArea = NativeMethods.LVHT_BELOW,
        LeftOfClientArea = NativeMethods.LVHT_LEFT,
        RightOfClientArea = NativeMethods.LVHT_RIGHT,
        Image = NativeMethods.LVHT_ONITEMICON,
        StateImage = 0x0200,
        Label = NativeMethods.LVHT_ONITEMLABEL
    }
}
