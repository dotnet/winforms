// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

/// <summary>
///  Event handler for the DesignSurface event.
/// </summary>
public delegate void DesignSurfaceEventHandler(object sender, DesignSurfaceEventArgs e);

/// <summary>
///  Event args for the DesignSurface event.
/// </summary>
public class DesignSurfaceEventArgs : EventArgs
{
    /// <summary>
    ///  Creates a new DesignSurfaceEventArgs for the given design surface.
    /// </summary>
    public DesignSurfaceEventArgs(DesignSurface surface)
    {
        Surface = surface.OrThrowIfNull();
    }

    /// <summary>
    ///  The design surface passed into the constructor.
    /// </summary>
    public DesignSurface Surface { get; }
}
