// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;

    /// <include file='doc\MeasureItemEvent.uex' path='docs/doc[@for="MeasureItemEventArgs"]/*' />
    /// <devdoc>
    ///     This event is sent by controls such as the ListBox or ComboBox that need users
    ///     to tell them how large a given item is to be.
    /// </devdoc>
    public class MeasureItemEventArgs : EventArgs {

        private int itemHeight;
        private int itemWidth;
        private int index;

        private readonly System.Drawing.Graphics graphics;


        /// <include file='doc\MeasureItemEvent.uex' path='docs/doc[@for="MeasureItemEventArgs.MeasureItemEventArgs"]/*' />
        public MeasureItemEventArgs(Graphics graphics, int index, int itemHeight) {
            this.graphics = graphics;
            this.index = index;
            this.itemHeight = itemHeight;
            this.itemWidth = 0;
        }

        /// <include file='doc\MeasureItemEvent.uex' path='docs/doc[@for="MeasureItemEventArgs.MeasureItemEventArgs1"]/*' />
        public MeasureItemEventArgs(Graphics graphics, int index) {
            this.graphics = graphics;
            this.index = index;
            this.itemHeight = 0;
            this.itemWidth = 0;
        }
        
        /// <include file='doc\MeasureItemEvent.uex' path='docs/doc[@for="MeasureItemEventArgs.Graphics"]/*' />
        /// <devdoc>
        ///     A Graphics object to measure relative to.
        /// </devdoc>
        public System.Drawing.Graphics Graphics {
            get {
                return graphics;
            }
        }

        /// <include file='doc\MeasureItemEvent.uex' path='docs/doc[@for="MeasureItemEventArgs.Index"]/*' />
        /// <devdoc>
        ///     The index of item for which the height/width is needed.
        /// </devdoc>
        public int Index {
            get {
                return index;
            }
        }

        /// <include file='doc\MeasureItemEvent.uex' path='docs/doc[@for="MeasureItemEventArgs.ItemHeight"]/*' />
        /// <devdoc>
        ///     Where the recipient of the event should put the height of the
        ///     item specified by the index.
        /// </devdoc>
        public int ItemHeight {
            get {
                return itemHeight;
            }
            set {
                itemHeight = value;
            }
        }

        /// <include file='doc\MeasureItemEvent.uex' path='docs/doc[@for="MeasureItemEventArgs.ItemWidth"]/*' />
        /// <devdoc>
        ///     Where the recipient of the event should put the width of the
        ///     item specified by the index.
        /// </devdoc>
        public int ItemWidth {
            get {
                return itemWidth;
            }
            set {
                itemWidth = value;
            }
        }
    }
}
