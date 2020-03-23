// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public class HandledMouseEventArgs : MouseEventArgs
    {
        public HandledMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta) : this(button, clicks, x, y, delta, false)
        {
        }

        public HandledMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta, bool defaultHandledValue) : base(button, clicks, x, y, delta)
        {
            Handled = defaultHandledValue;
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the event is handled.
        /// </summary>
        public bool Handled { get; set; }
    }
}
