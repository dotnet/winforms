// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms {
    /// <devdoc>
    ///
    /// </devdoc>
    [
        SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")  // Maps to native enum.
    ]
    public enum SearchDirectionHint {
        /// <devdoc>
        ///
        /// </devdoc>
        Up = NativeMethods.VK_UP,
        /// <devdoc>
        ///
        /// </devdoc>
        Down = NativeMethods.VK_DOWN,
        /// <devdoc>
        ///
        /// </devdoc>
        Left = NativeMethods.VK_LEFT,
        /// <devdoc>
        ///
        /// </devdoc>
        Right = NativeMethods.VK_RIGHT
    }
}
