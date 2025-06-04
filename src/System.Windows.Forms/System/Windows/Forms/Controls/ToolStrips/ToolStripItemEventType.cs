// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  These methods allow the ToolStrip to route events
///  to the ToolStrip item. Since a ToolStrip is not a ToolStripItem,
///  it cannot directly call OnPaint.
/// </summary>
internal enum ToolStripItemEventType
{
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
