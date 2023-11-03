// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;

namespace System.Drawing.Design;

/// <summary>
///  Provides data for the 'ToolboxComponentsCreatingEventArgs' event that occurs
///  when components are added to the toolbox.
/// </summary>
public class ToolboxComponentsCreatingEventArgs : EventArgs
{
    /// <summary>
    ///  Initializes a new instance of the <see cref="ToolboxComponentsCreatingEventArgs"/> object.
    /// </summary>
    public ToolboxComponentsCreatingEventArgs(IDesignerHost? host)
    {
        DesignerHost = host;
    }

    /// <summary>
    ///  An instance of IDesignerHost that has made the creat request.
    ///  This can be null if no designer host was provided to the toolbox item.
    /// </summary>
    public IDesignerHost? DesignerHost { get; }
}
