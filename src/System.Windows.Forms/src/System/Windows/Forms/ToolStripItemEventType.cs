// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;


    /// <devdoc>
    /// These methods allow the ToolStrip to route events
    /// to the winbar item.  Since a ToolStrip is not a ToolStripItem,
    /// it cannot directly call OnPaint.
    /// </devdoc>
    internal enum ToolStripItemEventType {
        Paint,
        LocationChanged,
        MouseUp,
        MouseDown,
        MouseMove,
        MouseEnter,
        MouseLeave,
        MouseHover,
        Click,
        DoubleClick
    }
}
