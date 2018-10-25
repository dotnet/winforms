// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    [
    Flags
    ]
    /// <include file='doc\ListViewHitTestLocations.uex' path='docs/doc[@for="ListViewHitTestLocations"]/*' />
    public enum ListViewHitTestLocations {
        /// <include file='doc\ListViewHitTestLocations.uex' path='docs/doc[@for="ListViewHitTestLocations.None"]/*' />
        None = NativeMethods.LVHT_NOWHERE,
        /// <include file='doc\ListViewHitTestLocations.uex' path='docs/doc[@for="ListViewHitTestLocations.AboveClientArea"]/*' />
        AboveClientArea = 0x0100,
        /// <include file='doc\ListViewHitTestLocations.uex' path='docs/doc[@for="ListViewHitTestLocations.BelowClientArea"]/*' />
        BelowClientArea = NativeMethods.LVHT_BELOW,
        /// <include file='doc\ListViewHitTestLocations.uex' path='docs/doc[@for="ListViewHitTestLocations.LeftOfClientArea"]/*' />
        LeftOfClientArea = NativeMethods.LVHT_LEFT,
        /// <include file='doc\ListViewHitTestLocations.uex' path='docs/doc[@for="ListViewHitTestLocations.RightOfClientArea"]/*' />
        RightOfClientArea = NativeMethods.LVHT_RIGHT,
        /// <include file='doc\ListViewHitTestLocations.uex' path='docs/doc[@for="ListViewHitTestLocations.Image"]/*' />
        Image = NativeMethods.LVHT_ONITEMICON,
        /// <include file='doc\ListViewHitTestLocations.uex' path='docs/doc[@for="ListViewHitTestLocations.StateImage"]/*' />
        StateImage = 0x0200, 
        /// <include file='doc\ListViewHitTestLocations.uex' path='docs/doc[@for="ListViewHitTestLocations.Label"]/*' />
        Label = NativeMethods.LVHT_ONITEMLABEL
    }
}
