// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='Control.KeyPress'/> event.
    /// </summary>
    public class KeyPressEventArgs : EventArgs
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref='KeyPressEventArgs'/>
        ///  class.
        /// </summary>
        public KeyPressEventArgs(char keyChar)
        {
            KeyChar = keyChar;
        }

        /// <summary>
        ///  Gets the character corresponding to the key pressed.
        /// </summary>
        public char KeyChar { get; set; }

        /// <summary>
        ///  Gets or sets a value indicating whether the <see cref='Control.KeyPress'/>
        ///  event was handled.
        /// </summary>
        public bool Handled { get; set; }
    }
}
