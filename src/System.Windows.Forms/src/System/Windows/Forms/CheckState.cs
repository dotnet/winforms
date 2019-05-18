﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies the state of a control,  such as a check box, that can be
    /// checked, unchecked, or set to an indeterminate state.
    /// </devdoc>
    public enum CheckState
    {
        /// <summary>
        /// The control is unchecked.
        /// </devdoc>
        Unchecked = 0,

        /// <summary>
        /// The control is checked.
        /// </devdoc>
        Checked = 1,

        /// <summary>
        /// The control is indeterminate. An indeterminate control generally has
        /// a shaded appearance.
        /// </devdoc>
        Indeterminate = 2,
    }
}
