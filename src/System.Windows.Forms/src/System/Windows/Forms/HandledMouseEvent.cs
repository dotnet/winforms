// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public class HandledMouseEventArgs : MouseEventArgs
    {
        /// <devdoc>
        ///     Indicates, on return, whether or not the event was handled in the application's event handler.  
        ///     'true' means the application handled the event, 'false' means it didn't.
        /// </devdoc>
        private bool handled;

        public HandledMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta) : this(button, clicks, x, y, delta, false)
        {
        }

        public HandledMouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta, bool defaultHandledValue) : base(button, clicks, x, y, delta)
        {
            this.handled = defaultHandledValue;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value
        ///       indicating whether the event is handled.
        ///    </para>
        /// </devdoc>
        public bool Handled
        {
            get
            {
                return this.handled;
            }
            set
            {
                this.handled = value;
            }
        }
    }
}
