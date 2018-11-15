// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;
 
    // this enum is tightly coupled to Orientation so you can determine quickly 
    // an orientation from a direction.  (direction & Orientation.Vertical == Orientation.Vertical)    
    /// <include file='doc\Direction.uex' path='docs/doc[@for="ArrowDirection"]/*' />    
    [SuppressMessage("Microsoft.Design", "CA1027:MarkEnumsWithFlags")]
    public enum ArrowDirection {
        /// <include file='doc\ArrowDirection.uex' path='docs/doc[@for="ArrowDirection.Up"]/*' />
        Up = 0x00   | (int)Orientation.Vertical,
        /// <include file='doc\ArrowDirection.uex' path='docs/doc[@for="ArrowDirection.Down"]/*' />
        Down = 0x10 | (int)Orientation.Vertical,
        /// <include file='doc\ArrowDirection.uex' path='docs/doc[@for="ArrowDirection.Left"]/*' />
        Left = 0x00 | (int)Orientation.Horizontal,
        /// <include file='doc\ArrowDirection.uex' path='docs/doc[@for="ArrowDirection.Right"]/*' />
        Right =0x10 | (int)Orientation.Horizontal,
    }
}
