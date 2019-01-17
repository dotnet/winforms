// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;

    /// <include file='doc\ScrollEvent.uex' path='docs/doc[@for="ScrollEventArgs"]/*' />
    /// <devdoc>
    /// <para>Provides data for the <see cref='System.Windows.Forms.ScrollBar.Scroll'/>
    /// event.</para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class ScrollEventArgs : EventArgs {

        readonly ScrollEventType type;
        int newValue;
        private ScrollOrientation scrollOrientation;
        int oldValue = -1;
        
        /// <include file='doc\ScrollEvent.uex' path='docs/doc[@for="ScrollEventArgs.ScrollEventArgs"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.ScrollEventArgs'/>class.
        ///       
        ///    </para>
        /// </devdoc>
        public ScrollEventArgs(ScrollEventType type, int newValue) {
            this.type = type;
            this.newValue = newValue;
        }

        /// <include file='doc\ScrollEvent.uex' path='docs/doc[@for="ScrollEventArgs.ScrollEventArgs1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.ScrollEventArgs'/>class.
        ///       
        ///    </para>
        /// </devdoc>
        public ScrollEventArgs(ScrollEventType type, int newValue, ScrollOrientation scroll) {
            this.type = type;
            this.newValue = newValue;
            this.scrollOrientation = scroll;
        }

        /// <include file='doc\ScrollEvent.uex' path='docs/doc[@for="ScrollEventArgs.ScrollEventArgs2"]/*' />
        public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue) {
            this.type = type;
            this.newValue = newValue;
            this.oldValue = oldValue;

        }


        /// <include file='doc\ScrollEvent.uex' path='docs/doc[@for="ScrollEventArgs.ScrollEventArgs3"]/*' />
        public ScrollEventArgs(ScrollEventType type, int oldValue,  int newValue, ScrollOrientation scroll) {
            this.type = type;
            this.newValue = newValue;
            this.scrollOrientation = scroll;
            this.oldValue = oldValue;
        }

        /// <include file='doc\ScrollEvent.uex' path='docs/doc[@for="ScrollEventArgs.Scroll"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies the type of scroll event that occurred.
        ///       
        ///    </para>
        /// </devdoc>
        public ScrollOrientation ScrollOrientation {
            get {
                return scrollOrientation;
            }
        }

        /// <include file='doc\ScrollEvent.uex' path='docs/doc[@for="ScrollEventArgs.Type"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies the type of scroll event that occurred.
        ///       
        ///    </para>
        /// </devdoc>
        public ScrollEventType Type {
            get {
                return type;
            }
        }

        /// <include file='doc\ScrollEvent.uex' path='docs/doc[@for="ScrollEventArgs.NewValue"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies the new location of the scroll box
        ///       within the
        ///       scroll bar.
        ///       
        ///    </para>
        /// </devdoc>
        public int NewValue {
            get {
                return newValue;
            }
            set {
                newValue = value;
            }
        }

       /// <include file='doc\ScrollEvent.uex' path='docs/doc[@for="ScrollEventArgs.OldValue"]/*' />
       /// <devdoc>
       /// <para>
       /// Specifies the last position 
       /// within the
       /// scroll bar.
       /// </para>
       /// </devdoc>
       public int OldValue {
           get {
               return oldValue;
           }
       }


    }
}
