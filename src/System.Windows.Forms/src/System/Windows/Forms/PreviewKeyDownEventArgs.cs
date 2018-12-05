// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System;

    /// <include file='doc\PreviewKeyDownEvent.uex' path='docs/doc[@for="PreviewKeyDownEventArgs"]/*' />
    /// <internalonly/>
    /// <devdoc>
    ///    <para>
    ///       Provides data for the PreviewKeyDownEvent
    ///    </para>
    /// </devdoc>
    public class PreviewKeyDownEventArgs : EventArgs {

        private readonly Keys _keyData;
        private bool _isInputKey;

        /// <include file='doc\PreviewKeyDownEvent.uex' path='docs/doc[@for="PreviewKeyDownEventArgs.PreviewKeyDownEventArgs"]/*' />
        public PreviewKeyDownEventArgs(Keys keyData) {
            _keyData = keyData;
        }

       /// <include file='doc\KeyEvent.uex' path='docs/doc[@for="KeyEventArgs.Alt"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the ALT key was pressed.
        ///    </para>
        /// </devdoc>
        public bool Alt {
            get {
                return (_keyData & Keys.Alt) == Keys.Alt;
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
                return (_keyData & Keys.Control) == Keys.Control;
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
                Keys keyGenerated =  _keyData & Keys.KeyCode;

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
                return (int)(_keyData & Keys.KeyCode);
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
                return _keyData;
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
                return _keyData & Keys.Modifiers;
            }
        }

        /// <include file='doc\KeyEvent.uex' path='docs/doc[@for="KeyEventArgs.Shift"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       a value indicating whether the SHIFT key was pressed.
        ///    </para>
        /// </devdoc>
        public bool Shift {
            get {
                return (_keyData & Keys.Shift) == Keys.Shift;
            }
        }

        public bool IsInputKey {
            get {
                return _isInputKey;
            }
            set {
                _isInputKey = value;
            }
        }
    }
}