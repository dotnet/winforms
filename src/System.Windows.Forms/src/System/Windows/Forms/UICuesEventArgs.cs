// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the <see cref='System.Windows.Forms.Control.ChangeUICues'/> event.
    /// </devdoc>
    public class UICuesEventArgs : EventArgs
    {
        private readonly UICues _uicues;

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        public UICuesEventArgs(UICues uicues)
        {
            _uicues = uicues;
        }
        
        /// <devdoc>
        /// Focus rectangles are shown after the change.
        /// </devdoc>
        public bool ShowFocus => (_uicues & UICues.ShowFocus) != 0;
           
        /// <devdoc>
        /// Keyboard cues are underlined after the change.
        /// </devdoc>
        public bool ShowKeyboard => (_uicues & UICues.ShowKeyboard) != 0;
        
        /// <devdoc>
        /// The state of the focus cues has changed.
        /// </devdoc>
        public bool ChangeFocus => (_uicues & UICues.ChangeFocus) != 0;
        
        /// <devdoc>
        /// The state of the keyboard cues has changed.
        /// </devdoc>
        public bool ChangeKeyboard => (_uicues & UICues.ChangeKeyboard) != 0;
        
        public UICues Changed => (_uicues & UICues.Changed);
    }
}
