// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Private.Windows.Graphics;

namespace System.Private.Windows.Ole;

/// <summary>
///  Runtime agnostic interface for the GiveFeedback event.
/// </summary>
internal interface IGiveFeedbackEvent
{
    /// <summary>
    ///  Gets the drag image bitmap.
    /// </summary>
    IBitmap? DragImage { get; }

    /// <summary>
    ///  Gets the drag image cursor offset.
    /// </summary>
    Point CursorOffset { get; set; }

    /// <summary>
    ///  Gets a value indicating whether a layered window drag image is used.
    /// </summary>
    bool UseDefaultDragImage { get; set; }
}
