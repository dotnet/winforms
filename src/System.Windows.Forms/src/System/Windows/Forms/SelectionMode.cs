// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    [ComVisible(true)]
    public enum SelectionMode
    {
        /// <summary>
        /// Indicates that no items can be selected.
        /// </devdoc>
        None = 0,

        /// <summary>
        /// Indicates that only one item at a time can be selected.
        /// </devdoc>
        One = 1,

        /// <summary>
        /// Indicates that more than one item at a time can be selected.
        /// </devdoc>
        MultiSimple = 2,

        /// <summary>
        /// Indicates that more than one item at a time can be selected, and
        /// keyboard combinations, such as SHIFT and CTRL can be used to help
        /// in selection.
        /// </devdoc>
        MultiExtended = 3,
    }
}
