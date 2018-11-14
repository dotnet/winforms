// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
namespace System.Windows.Forms {
    /// <include file='doc\CacheVirtualItemsEventArgs.uex' path='docs/doc[@for="CacheVirtualItemsEventArgs"]/*' />
    public class CacheVirtualItemsEventArgs : EventArgs {
        private int startIndex;
        private int endIndex;
        /// <include file='doc\CacheVirtualItemsEventArgs.uex' path='docs/doc[@for="CacheVirtualItemsEventArgs.CacheVirtualItemsEventArgs"]/*' />
        public CacheVirtualItemsEventArgs(int startIndex, int endIndex) {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }
        /// <include file='doc\CacheVirtualItemsEventArgs.uex' path='docs/doc[@for="CacheVirtualItemsEventArgs.StartIndex"]/*' />
        public int StartIndex {
            get {
                return startIndex;
            }
        }
        /// <include file='doc\CacheVirtualItemsEventArgs.uex' path='docs/doc[@for="CacheVirtualItemsEventArgs.EndIndex"]/*' />
        public int EndIndex {
            get {
                return endIndex;
            }
        }
    }
}
