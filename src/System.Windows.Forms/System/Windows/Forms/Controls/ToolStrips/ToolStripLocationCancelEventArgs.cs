// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

/// <summary>
///  Provides Arguments for the Cancelable LocationChanging Event.
/// </summary>
internal class ToolStripLocationCancelEventArgs : CancelEventArgs
{
    /// <summary>
    ///  Initializes a new instance of the ToolStripLocationCancelEventArgs with cancel value.
    /// </summary>
    public ToolStripLocationCancelEventArgs(Point newLocation, bool value) : base(value)
    {
        NewLocation = newLocation;
    }

    /// <summary>
    ///  Returns the New Location of the ToolStrip.
    /// </summary>
    public Point NewLocation { get; }
}
