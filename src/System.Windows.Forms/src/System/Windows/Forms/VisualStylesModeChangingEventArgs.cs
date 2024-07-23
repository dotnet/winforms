// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
///  Represents the event data for the changing of the visual styles mode.
/// </summary>
[Experimental("WFO9000")]
public class VisualStylesModeChangingEventArgs : CancelEventArgs
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="VisualStylesModeChangingEventArgs"/> class
    ///  with the specified visual styles mode.
    /// </summary>
    /// <param name="visualStylesMode">The visual styles mode.</param>
    public VisualStylesModeChangingEventArgs(VisualStylesMode visualStylesMode) : base(false)
    {
        VisualStylesMode = visualStylesMode;
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="VisualStylesModeChangingEventArgs"/> class
    ///  with the specified visual styles mode and cancel flag.
    /// </summary>
    /// <param name="visualStylesMode">The visual styles mode.</param>
    /// <param name="cancel">true to cancel the event; otherwise, false.</param>
    public VisualStylesModeChangingEventArgs(VisualStylesMode visualStylesMode, bool cancel) : base(cancel)
    {
        VisualStylesMode = visualStylesMode;
    }

    /// <summary>
    ///  Gets the visual styles mode.
    /// </summary>
    public VisualStylesMode VisualStylesMode { get; }
}
