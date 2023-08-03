// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Drawing.Design;

/// <summary>
///  Provides data for the 'ToolboxComponentsCreatedEventArgs' event that occurs
///  when components are added to the toolbox.
/// </summary>
public class ToolboxComponentsCreatedEventArgs : EventArgs
{
    private readonly IComponent[]? _components;

    /// <summary>
    ///  Initializes a new instance of the <see cref="ToolboxComponentsCreatedEventArgs"/>
    /// </summary>
    public ToolboxComponentsCreatedEventArgs(IComponent[]? components)
    {
        _components = components;
    }

    /// <summary>
    ///  An array storing the toolbox components.
    /// </summary>
    public IComponent[]? Components => (IComponent[]?)_components?.Clone();
}
