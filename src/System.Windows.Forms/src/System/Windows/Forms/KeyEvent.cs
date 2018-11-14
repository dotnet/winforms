// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using Microsoft.Win32;
    using System.Diagnostics.CodeAnalysis;
    
    /// <include file='doc\KeyEvent.uex' path='docs/doc[@for="KeyEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.Control.KeyDown'/> or <see cref='System.Windows.Forms.Control.KeyUp'/>
    ///       event.
    ///    </para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class KeyEventArgs : EventArgs {

        /// <include file='doc\KeyEvent.uex' path='docs/doc[@for="KeyEventArgs.keyData"]/*' />
        /// <devdoc>
        ///     Contains key data for KeyDown and KeyUp events.  This is a combination
        ///     of keycode and modifer flags.
        /// </devdoc>
        private readonly Keys keyData;

        /// <include file='doc\KeyEvent.uex' path='docs/doc[@for="KeyEventArgs.handled"]/*' />
        /// <devdoc>
        ///     Determines if this event has been handled by a handler.  If handled, the
        ///     key event will not be sent along to Windows.  If not handled, the event
        ///     will be sent to Windows for default processing.
        /// </devdoc>
        private bool handled = false;

        /// <include file='doc\KeyEvent.uex' path='docs/doc[@for="KeyEventArgs.suppressKeyPress"]/*' />
        /// <devdoc>
        /// </devdoc>
        private bool suppressKeyPress = false;

        /// <include file='doc\KeyEvent.uex' path='docs/doc[@for="KeyEventArgs.KeyEventArgs"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new
        ///       instance of the <see cref='System.Windows.Forms.KeyEventArgs'/> class.
        ///    </para>
        /// </devdoc>
        public KeyEventArgs(Keys keyData) {
            this.keyData = keyData;
        }

        /// <include file='doc\KeyEvent.uex' path='docs/doc[@for="KeyEventArgs.Alt"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the ALT key was pressed.
        ///    </para>
        /// </devdoc>
        public virtual bool Alt {
            get {
                return (keyData & Keys.Alt) == Keys.Alt;
            }
        }

        /// <include file='doc\KeyEvent.uex' path='docs/doc[@for="KeyEventArgs.Control"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the CTRL key was pressed.
        ///    </para>
        /// </devdoc>
        public bool Control {
            get {
                return (keyData & Keys.Control) == Keys.Control;
            }
        }

        /// <include file='doc\KeyEvent.uex' path='docs/doc[@for="KeyEventArgs.Handled"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value
        ///       indicating whether the event was handled.
        ///    </para>
        /// </devdoc>
        //
        public bool Handled {
            get {
                return handled;
            }
            set {
                handled = value;
            }
        }

        /// <include file='doc\KeyEvent.uex' path='docs/doc[@for="KeyEventArgs.KeyCode"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the keyboard code for a <see cref='System.Windows.Forms.Control.KeyDown'/> or <see cref='System.Windows.Forms.Control.KeyUp'/>
        ///       event.
        ///    </para>
        /// </devdoc>
        //subhag : changed the behaviour of the KeyCode as per the new requirements.
        public Keys KeyCode {
            [
                // Keys is discontiguous so we have to use Enum.IsDefined.
                SuppressMessage("Microsoft.Performance", "CA1803:AvoidCostlyCallsWherePossible")
            ]
            get {
                Keys keyGenerated =  keyData & Keys.KeyCode;

                // since Keys can be discontiguous, keeping Enum.IsDefined.
                if (!Enum.IsDefined(typeof(Keys),(int)keyGenerated))
                    return Keys.None;
                else
                    return keyGenerated;
            }
        }

        /// <include file='doc\KeyEvent.uex' path='docs/doc[@for="KeyEventArgs.KeyValue"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the keyboard value for a <see cref='System.Windows.Forms.Control.KeyDown'/> or <see cref='System.Windows.Forms.Control.KeyUp'/>
        ///       event.
        ///    </para>
        /// </devdoc>
        //subhag : added the KeyValue as per the new requirements.
        public int KeyValue {
            get {
                return (int)(keyData & Keys.KeyCode);
            }
        }
       
        /// <include file='doc\KeyEvent.uex' path='docs/doc[@for="KeyEventArgs.KeyData"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the key data for a <see cref='System.Windows.Forms.Control.KeyDown'/> or <see cref='System.Windows.Forms.Control.KeyUp'/>
        ///       event.
        ///    </para>
        /// </devdoc>
        public Keys KeyData {
            get {
                return keyData;
            }
        }

        /// <include file='doc\KeyEvent.uex' path='docs/doc[@for="KeyEventArgs.Modifiers"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the modifier flags for a <see cref='System.Windows.Forms.Control.KeyDown'/> or <see cref='System.Windows.Forms.Control.KeyUp'/> event.
        ///       This indicates which modifier keys (CTRL, SHIFT, and/or ALT) were pressed.
        ///    </para>
        /// </devdoc>
        public Keys Modifiers {
            get {
                return keyData & Keys.Modifiers;
            }
        }

        /// <include file='doc\KeyEvent.uex' path='docs/doc[@for="KeyEventArgs.Shift"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       a value indicating whether the SHIFT key was pressed.
        ///    </para>
        /// </devdoc>
        public virtual bool Shift {
            get {
                return (keyData & Keys.Shift) == Keys.Shift;
            }
        }

        /// <include file='doc\KeyEvent.uex' path='docs/doc[@for="KeyEventArgs.SuppressKeyPress"]/*' />
        /// <devdoc>
        /// </devdoc>
        //
        public bool SuppressKeyPress {
            get {
                return suppressKeyPress;
            }
            set {
                suppressKeyPress = value;
                handled = value;
            }
        }

    }
}
