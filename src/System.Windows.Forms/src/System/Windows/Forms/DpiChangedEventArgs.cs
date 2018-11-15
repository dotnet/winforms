// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using Diagnostics;
    using Drawing;
    using System;
    using System.ComponentModel;

    /// <include file='doc\DpiChangedEventArgs.uex' path='docs/doc[@for="DpiChangedEventArgs"]/*' />
    /// <devdoc>
    ///     Provides information about a DpiChanged event.
    /// </devdoc>
    public sealed class DpiChangedEventArgs : CancelEventArgs {
        // Parameter units are pixels(dots) per inch. 
        internal DpiChangedEventArgs(int old, Message m) {
            DeviceDpiOld = old;
            DeviceDpiNew = NativeMethods.Util.SignedLOWORD(m.WParam);
            Debug.Assert(NativeMethods.Util.SignedHIWORD(m.WParam) == DeviceDpiNew, "Non-square pixels!");
            NativeMethods.RECT suggestedRect = (NativeMethods.RECT)UnsafeNativeMethods.PtrToStructure(m.LParam, typeof(NativeMethods.RECT));
            SuggestedRectangle = Rectangle.FromLTRB(suggestedRect.left, suggestedRect.top, suggestedRect.right, suggestedRect.bottom);
        }

        /// <include file='doc\DpiChangedEventArgs.uex' path='docs/doc[@for="DpiChangedEventArgs.DeviceDpiXOld"]/*' />
        /// <devdoc>
        /// </devdoc>
        public int DeviceDpiOld {
            get;
            private set;
        }

        /// <include file='doc\DpiChangedEventArgs.uex' path='docs/doc[@for="DpiChangedEventArgs.DeviceDpiXNew"]/*' />
        /// <devdoc>
        /// </devdoc>
        public int DeviceDpiNew {
            get;
            private set;
        }

        /// <include file='doc\DpiChangedEventArgs.uex' path='docs/doc[@for="DpiChangedEventArgs.SuggestedRectangle"]/*' />
        /// <devdoc>
        /// </devdoc>
        public Rectangle SuggestedRectangle {
            get;
            private set;
        }

        public override string ToString() {
            return $"was: {DeviceDpiOld}, now: {DeviceDpiNew}";
        }
    }
}
