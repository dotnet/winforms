// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies UI state.
    /// </devdoc>
    [Flags]
    public enum UICues
    {
        /// <summary>
        /// Focus rectangles are shown after the change.
        /// </devdoc>
        ShowFocus = 0x01,
        
        /// <summary>
        /// Keyboard cues are underlined after the change.
        /// </devdoc>
        ShowKeyboard = 0x02,
        
        Shown = ShowFocus | ShowKeyboard,
        
        /// <summary>
        /// The state of the focus cues has changed.
        /// </devdoc>
        ChangeFocus = 0x04,
        
        /// <summary>
        /// The state of the keyboard cues has changed.
        /// </devdoc>
        ChangeKeyboard = 0x08,
        
        Changed = ChangeFocus | ChangeKeyboard,
        
        None = 0x00,
    }
}
