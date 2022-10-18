// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    [Flags]
    public enum ListViewHitTestLocations
    {
        None = (int)LVHITTESTINFO_FLAGS.LVHT_NOWHERE,
        AboveClientArea = 0x0100,
        BelowClientArea = (int)LVHITTESTINFO_FLAGS.LVHT_BELOW,
        LeftOfClientArea = (int)LVHITTESTINFO_FLAGS.LVHT_TOLEFT,
        RightOfClientArea = (int)LVHITTESTINFO_FLAGS.LVHT_TORIGHT,
        Image = (int)LVHITTESTINFO_FLAGS.LVHT_ONITEMICON,
        StateImage = 0x0200,
        Label = (int)LVHITTESTINFO_FLAGS.LVHT_ONITEMLABEL
    }
}
