// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='ScrollBar.Scroll'/>
    ///  event.
    /// </summary>
    public class ScrollEventArgs : EventArgs
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref='ScrollEventArgs'/> class.
        /// </summary>
        public ScrollEventArgs(ScrollEventType type, int newValue)
        {
            Type = type;
            NewValue = newValue;
            OldValue = -1;
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='ScrollEventArgs'/> class.
        /// </summary>
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

        public ScrollEventArgs(ScrollEventType type, int oldValue, int newValue, ScrollOrientation scroll)
        {
            Type = type;
            NewValue = newValue;
            OldValue = oldValue;
            ScrollOrientation = scroll;
        }

        /// <summary>
        ///  Specifies the type of scroll event that occurred.
        /// </summary>
        public ScrollEventType Type { get; }

        /// <summary>
        ///  Specifies the new location of the scroll box within the scroll bar.
        /// </summary>
        public int NewValue { get; set; }

        /// <summary>
        ///  Specifies the last position  within the scroll bar.
        /// </summary>
        public int OldValue { get; }

        /// <summary>
        ///  Specifies the type of scroll event that occurred.
        /// </summary>
        public ScrollOrientation ScrollOrientation { get; }
    }
}
