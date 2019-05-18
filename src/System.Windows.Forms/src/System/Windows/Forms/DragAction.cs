// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Specifies how and if a drag-and-drop operation should continue.
    /// </devdoc>
    [ComVisible(true)]
    public enum DragAction
    {
        /// <summary>
        /// The operation will continue.
        /// </devdoc>
        Continue = 0,

        /// <summary>
        /// The operation will stop with a drop.
        /// </devdoc>
        Drop = 1,

        /// <summary>
        /// The operation is canceled with no drop message.
        /// </devdoc>
        Cancel = 2,
    }
}
