// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    [Flags]
    public enum ListViewHitTestLocations
    {
        None = (int)LVHT.NOWHERE,
        AboveClientArea = 0x0100,
        BelowClientArea = (int)LVHT.BELOW,
        LeftOfClientArea = (int)LVHT.TOLEFT,
        RightOfClientArea = (int)LVHT.TORIGHT,
        Image = (int)LVHT.ONITEMICON,
        StateImage = 0x0200,
        Label = (int)LVHT.ONITEMLABEL
    }
}
