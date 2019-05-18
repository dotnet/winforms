// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides data for the <see cref='System.Windows.Forms.Control.KeyPress'/> event.
    /// </devdoc>
    [ComVisible(true)]
    public class KeyPressEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.KeyPressEventArgs'/>
        /// class.
        /// </devdoc>
        public KeyPressEventArgs(char keyChar)
        {
            KeyChar = keyChar;
        }

        /// <summary>
        /// Gets the character corresponding to the key pressed.
        /// </devdoc>
        public char KeyChar { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref='System.Windows.Forms.Control.KeyPress'/>
        /// event was handled.
        /// </devdoc>
        public bool Handled { get; set; }
    }
}
