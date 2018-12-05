// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    /// <devdoc>
    ///    <para>
    ///       Specifies UI state.
    ///    </para>
    /// </devdoc>
    [Flags]
    public enum UICues {
    
        /// <devdoc>
        ///     Focus rectangles are shown after the change.
        /// </devdoc>
        ShowFocus = 0x01,
        
        /// <devdoc>
        ///     Keyboard cues are underlined after the change.
        /// </devdoc>
        ShowKeyboard = 0x02,
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Shown = ShowFocus | ShowKeyboard,
        
        /// <devdoc>
        ///     The state of the focus cues has changed.
        /// </devdoc>
        ChangeFocus = 0x04,
        
        /// <devdoc>
        ///     The state of the keyboard cues has changed.
        /// </devdoc>
        ChangeKeyboard = 0x08,
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        Changed = ChangeFocus | ChangeKeyboard,
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        None = 0x00,
    }

    /// <devdoc>
    ///    <para>
    ///       Provides data for the <see cref='System.Windows.Forms.Control.ChangeUICues'/> event.
    ///    </para>
    /// </devdoc>
    public class UICuesEventArgs : EventArgs {
        
        private readonly UICues uicues;
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public UICuesEventArgs(UICues uicues) {
            this.uicues = uicues;
        }
        
        /// <devdoc>
        ///     Focus rectangles are shown after the change.
        /// </devdoc>
        public bool ShowFocus {
            get {
                return (uicues & UICues.ShowFocus) != 0;
            }
        }
           
        /// <devdoc>
        ///     Keyboard cues are underlined after the change.
        /// </devdoc>
        public bool ShowKeyboard {
            get {
                return (uicues & UICues.ShowKeyboard) != 0;
            }
        }
        
        /// <devdoc>
        ///     The state of the focus cues has changed.
        /// </devdoc>
        public bool ChangeFocus {
            get {
                return (uicues & UICues.ChangeFocus) != 0;
            }
        }
        
        /// <devdoc>
        ///     The state of the keyboard cues has changed.
        /// </devdoc>
        public bool ChangeKeyboard {
            get {
                return (uicues & UICues.ChangeKeyboard) != 0;
            }
        }
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public UICues Changed {
            get {
                return (uicues & UICues.Changed);
            }
        }
    }
}
