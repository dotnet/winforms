// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies how an object or text in a control is horizontally aligned
    /// relative to an element of the control.
    /// </devdoc>
    [ComVisible(true)]
    public enum HorizontalAlignment
    {
        /// <summary>
        /// The object or text is aligned on the left of the control element.
        /// </devdoc>
        Left = 0,

        /// <summary>
        /// The object or text is aligned on the right of the control element.
        /// </devdoc>
        Right = 1,

        /// <summary>
        /// The object or text is aligned in the center of the control element.
        /// </devdoc>
        Center = 2,
    }
}
