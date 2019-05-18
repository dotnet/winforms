// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies whether any characters in the
    /// current selection have the style or attribute.
    ///
    /// </devdoc>
    public enum RichTextBoxSelectionAttribute
    {
        /// <summary>
        /// Some but not all characters.
        /// </devdoc>
        Mixed = -1,

        /// <summary>
        /// No characters.
        /// </devdoc>
        None = 0,

        /// <summary>
        /// All characters.
        /// </devdoc>
        All = 1,
    }
}
