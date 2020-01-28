// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides data for the <see cref='Control.ChangeUICues'/> event.
    /// </summary>
    public class UICuesEventArgs : EventArgs
    {
        private readonly UICues _uicues;

        public UICuesEventArgs(UICues uicues)
        {
            _uicues = uicues;
        }

        /// <summary>
        ///  Focus rectangles are shown after the change.
        /// </summary>
        public bool ShowFocus => (_uicues & UICues.ShowFocus) != 0;

        /// <summary>
        ///  Keyboard cues are underlined after the change.
        /// </summary>
        public bool ShowKeyboard => (_uicues & UICues.ShowKeyboard) != 0;

        /// <summary>
        ///  The state of the focus cues has changed.
        /// </summary>
        public bool ChangeFocus => (_uicues & UICues.ChangeFocus) != 0;

        /// <summary>
        ///  The state of the keyboard cues has changed.
        /// </summary>
        public bool ChangeKeyboard => (_uicues & UICues.ChangeKeyboard) != 0;

        public UICues Changed => (_uicues & UICues.Changed);
    }
}
