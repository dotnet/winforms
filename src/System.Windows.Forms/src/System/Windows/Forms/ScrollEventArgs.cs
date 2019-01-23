// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.ScrollBar.Scroll'/>
    /// event.
    /// </devdoc>
    [ComVisible(true)]
    public class ScrollEventArgs : EventArgs
    {
        /// <devdoc>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.ScrollEventArgs'/>class.
        /// </devdoc>
        public ScrollEventArgs(ScrollEventType type, int newValue)
        {
            Type = type;
            NewValue = newValue;
            OldValue = -1;
        }

        /// <devdoc>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.ScrollEventArgs'/>class.
        /// </devdoc>
        public ScrollEventArgs(ScrollEventType type, int newValue, ScrollOrientation scroll)
        {
            Type = type;
            NewValue = newValue;
            ScrollOrientation = scroll;
            OldValue = -1;
        }

        public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue)
        {
            Type = type;
            NewValue = newValue;
            OldValue = oldValue;
        }


        /// <include file='doc\ScrollEvent.uex' path='docs/doc[@for="ScrollEventArgs.ScrollEventArgs3"]/*' />
        public ScrollEventArgs(ScrollEventType type, int oldValue,  int newValue, ScrollOrientation scroll) {
            Type = type;
            NewValue = newValue;
            OldValue = oldValue;
            ScrollOrientation = scroll;
        }

        /// <devdoc>
        /// Specifies the type of scroll event that occurred.
        /// </devdoc>
        public ScrollEventType Type { get; }

        /// <devdoc>
        /// Specifies the new location of the scroll box within the scroll bar.
        /// </devdoc>
        public int NewValue { get; set; }

       /// <devdoc>
       /// Specifies the last position  within the scroll bar.
       /// </devdoc>
       public int OldValue { get; }

        /// <devdoc>
        /// Specifies the type of scroll event that occurred.
        /// </devdoc>
        public ScrollOrientation ScrollOrientation { get; }
    }
}
