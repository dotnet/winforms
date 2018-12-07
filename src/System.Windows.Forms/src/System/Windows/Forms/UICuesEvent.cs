// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    /// <include file='doc\UICuesEvent.uex' path='docs/doc[@for="UICues"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies UI state.
    ///    </para>
    /// </devdoc>
    [Flags]
    public enum UICues {
    
        /// <include file='doc\UICuesEvent.uex' path='docs/doc[@for="UICues.ShowFocus"]/*' />
        /// <devdoc>
        ///     Focus rectangles are shown after the change.
        /// </devdoc>
        ShowFocus = 0x01,
        
        /// <include file='doc\UICuesEvent.uex' path='docs/doc[@for="UICues.ShowKeyboard"]/*' />
        /// <devdoc>
        ///     Keyboard cues are underlined after the change.
        /// </devdoc>
        ShowKeyboard = 0x02,
        
        /// <include file='doc\UICuesEvent.uex' path='docs/doc[@for="UICues.Shown"]/*' />
        Shown = ShowFocus | ShowKeyboard,
        
        /// <include file='doc\UICuesEvent.uex' path='docs/doc[@for="UICues.ChangeFocus"]/*' />
        /// <devdoc>
        ///     The state of the focus cues has changed.
        /// </devdoc>
        ChangeFocus = 0x04,
        
        /// <include file='doc\UICuesEvent.uex' path='docs/doc[@for="UICues.ChangeKeyboard"]/*' />
        /// <devdoc>
        ///     The state of the keyboard cues has changed.
        /// </devdoc>
        ChangeKeyboard = 0x08,
        
        /// <include file='doc\UICuesEvent.uex' path='docs/doc[@for="UICues.Changed"]/*' />
        Changed = ChangeFocus | ChangeKeyboard,
        
        /// <include file='doc\UICuesEvent.uex' path='docs/doc[@for="UICues.None"]/*' />
        None = 0x00,
    }

    /// <include file='doc\UICuesEvent.uex' path='docs/doc[@for="UICuesEventArgs"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.Control.ChangeUICues'/> event.
    ///    </para>
    /// </devdoc>
    public class UICuesEventArgs : EventArgs {
        
        private readonly UICues uicues;
        
        /// <include file='doc\UICuesEvent.uex' path='docs/doc[@for="UICuesEventArgs.UICuesEventArgs"]/*' />
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public UICuesEventArgs(UICues uicues) {
            this.uicues = uicues;
        }
        
        /// <include file='doc\UICuesEvent.uex' path='docs/doc[@for="UICuesEventArgs.ShowFocus"]/*' />
        /// <devdoc>
        ///     Focus rectangles are shown after the change.
        /// </devdoc>
        public bool ShowFocus {
            get {
                return (uicues & UICues.ShowFocus) != 0;
            }
        }
           
        /// <include file='doc\UICuesEvent.uex' path='docs/doc[@for="UICuesEventArgs.ShowKeyboard"]/*' />
        /// <devdoc>
        ///     Keyboard cues are underlined after the change.
        /// </devdoc>
        public bool ShowKeyboard {
            get {
                return (uicues & UICues.ShowKeyboard) != 0;
            }
        }
        
        /// <include file='doc\UICuesEvent.uex' path='docs/doc[@for="UICuesEventArgs.ChangeFocus"]/*' />
        /// <devdoc>
        ///     The state of the focus cues has changed.
        /// </devdoc>
        public bool ChangeFocus {
            get {
                return (uicues & UICues.ChangeFocus) != 0;
            }
        }
        
        /// <include file='doc\UICuesEvent.uex' path='docs/doc[@for="UICuesEventArgs.ChangeKeyboard"]/*' />
        /// <devdoc>
        ///     The state of the keyboard cues has changed.
        /// </devdoc>
        public bool ChangeKeyboard {
            get {
                return (uicues & UICues.ChangeKeyboard) != 0;
            }
        }
        
        /// <include file='doc\UICuesEvent.uex' path='docs/doc[@for="UICuesEventArgs.Changed"]/*' />
        public UICues Changed {
            get {
                return (uicues & UICues.Changed);
            }
        }
    }
}
