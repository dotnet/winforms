// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;

    /// <devdoc>
    ///     This event is sent by controls such as the ListBox or ComboBox that need users
    ///     to tell them how large a given item is to be.
    /// </devdoc>
    public class MeasureItemEventArgs : EventArgs {

        private int itemHeight;
        private int itemWidth;
        private int index;

        private readonly System.Drawing.Graphics graphics;


        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public MeasureItemEventArgs(Graphics graphics, int index, int itemHeight) {
            this.graphics = graphics;
            this.index = index;
            this.itemHeight = itemHeight;
            this.itemWidth = 0;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public MeasureItemEventArgs(Graphics graphics, int index) {
            this.graphics = graphics;
            this.index = index;
            this.itemHeight = 0;
            this.itemWidth = 0;
        }
        
        /// <devdoc>
        ///     A Graphics object to measure relative to.
        /// </devdoc>
        public System.Drawing.Graphics Graphics {
            get {
                return graphics;
            }
        }

        /// <devdoc>
        ///     The index of item for which the height/width is needed.
        /// </devdoc>
        public int Index {
            get {
                return index;
            }
        }

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
