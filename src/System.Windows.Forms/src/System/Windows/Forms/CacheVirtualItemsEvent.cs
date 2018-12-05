// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


using System;
namespace System.Windows.Forms {
    public class CacheVirtualItemsEventArgs : EventArgs {
        private int startIndex;
        private int endIndex;
        public CacheVirtualItemsEventArgs(int startIndex, int endIndex) {
            this.startIndex = startIndex;
            this.endIndex = endIndex;
        }
        public int StartIndex {
            get {
                return startIndex;
            }
        }
        public int EndIndex {
            get {
                return endIndex;
            }
        }
    }
}
