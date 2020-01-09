// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='Forms.Control.KeyDown'/> or
    /// <see cref='Forms.Control.KeyUp'/> event.
    /// </summary>
    [ComVisible(true)]
    public class KeyEventArgs : EventArgs
    {
        private bool _suppressKeyPress = false;

        /// <summary>
        ///  Initializes a new instance of the <see cref='KeyEventArgs'/> class.
        /// </summary>
        public KeyEventArgs(Keys keyData)
        {
            KeyData = keyData;
        }

        /// <summary>
        ///  Gets a value indicating whether the ALT key was pressed.
        /// </summary>
        public virtual bool Alt => (KeyData & Keys.Alt) == Keys.Alt;

        /// <summary>
        ///  Gets a value indicating whether the CTRL key was pressed.
        /// </summary>
        public bool Control => (KeyData & Keys.Control) == Keys.Control;

        /// <summary>
        ///  Gets or sets a value indicating whether the event was handled.
        /// </summary>
        public bool Handled { get; set; }

        /// <summary>
        ///  Gets the keyboard code for a <see cref='Forms.Control.KeyDown'/> or
        /// <see cref='Forms.Control.KeyUp'/> event.
        /// </summary>
        public Keys KeyCode
        {
            get
            {
                Keys keyGenerated = KeyData & Keys.KeyCode;

                // since Keys can be discontiguous, keeping Enum.IsDefined.
                if (!Enum.IsDefined(typeof(Keys), (int)keyGenerated))
                {
                    return Keys.None;
                }
                return keyGenerated;
            }
        }

        /// <summary>
        ///  Gets the keyboard value for a <see cref='Forms.Control.KeyDown'/> or
        /// <see cref='Forms.Control.KeyUp'/> event.
        /// </summary>
        public int KeyValue => (int)(KeyData & Keys.KeyCode);

        /// <summary>
        ///  Gets the key data for a <see cref='Forms.Control.KeyDown'/> or
        /// <see cref='Forms.Control.KeyUp'/> event.
        /// </summary>
        public Keys KeyData { get; }

        /// <summary>
        ///  Gets the modifier flags for a <see cref='Forms.Control.KeyDown'/> or
        /// <see cref='Forms.Control.KeyUp'/> event.
        ///  This indicates which modifier keys (CTRL, SHIFT, and/or ALT) were pressed.
        /// </summary>
        public Keys Modifiers => KeyData & Keys.Modifiers;

        /// <summary>
        ///  Gets a value indicating whether the SHIFT key was pressed.
        /// </summary>
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
