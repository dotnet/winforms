// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.Control.KeyDown'/> or
    /// <see cref='System.Windows.Forms.Control.KeyUp'/> event.
    /// </devdoc>
    [ComVisible(true)]
    public class KeyEventArgs : EventArgs
    {
        private bool _suppressKeyPress = false;

        /// <devdoc>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.KeyEventArgs'/> class.
        /// </devdoc>
        public KeyEventArgs(Keys keyData)
        {
            KeyData = keyData;
        }

        /// <devdoc>
        /// Gets a value indicating whether the ALT key was pressed.
        /// </devdoc>
        public virtual bool Alt => (KeyData & Keys.Alt) == Keys.Alt;

        /// <devdoc>
        /// Gets a value indicating whether the CTRL key was pressed.
        /// </devdoc>
        public bool Control => (KeyData & Keys.Control) == Keys.Control;

        /// <devdoc>
        /// Gets or sets a value indicating whether the event was handled.
        /// </devdoc>
        public bool Handled { get; set; }

        /// <devdoc>
        /// Gets the keyboard code for a <see cref='System.Windows.Forms.Control.KeyDown'/> or
        /// <see cref='System.Windows.Forms.Control.KeyUp'/> event.
        /// </devdoc>
        public Keys KeyCode
        {
            [SuppressMessage("Microsoft.Performance", "CA1803:AvoidCostlyCallsWherePossible")]
            get
            {
                Keys keyGenerated =  KeyData & Keys.KeyCode;

                // since Keys can be discontiguous, keeping Enum.IsDefined.
                if (!Enum.IsDefined(typeof(Keys),(int)keyGenerated))
                {
                    return Keys.None;
                }
                return keyGenerated;
            }
        }

        /// <devdoc>
        /// Gets the keyboard value for a <see cref='System.Windows.Forms.Control.KeyDown'/> or
        /// <see cref='System.Windows.Forms.Control.KeyUp'/> event.
        /// </devdoc>
        public int KeyValue => (int)(KeyData & Keys.KeyCode);

        /// <devdoc>
        /// Gets the key data for a <see cref='System.Windows.Forms.Control.KeyDown'/> or
        /// <see cref='System.Windows.Forms.Control.KeyUp'/> event.
        /// </devdoc>
        public Keys KeyData { get; }

        /// <devdoc>
        /// Gets the modifier flags for a <see cref='System.Windows.Forms.Control.KeyDown'/> or
        /// <see cref='System.Windows.Forms.Control.KeyUp'/> event.
        /// This indicates which modifier keys (CTRL, SHIFT, and/or ALT) were pressed.
        /// </devdoc>
        public Keys Modifiers => KeyData & Keys.Modifiers;

        /// <devdoc>
        /// Gets a value indicating whether the SHIFT key was pressed.
        /// </devdoc>
        public virtual bool Shift => (KeyData & Keys.Shift) == Keys.Shift;

        public bool SuppressKeyPress
        {
            get => _suppressKeyPress;
            set
            {
                _suppressKeyPress = value;
                Handled = value;
            }
        }
    }
}
