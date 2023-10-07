﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Design;

internal class DataGridViewComponentPropertyGridSite : ISite
{
    private readonly IServiceProvider? _sp;
    private bool _inGetService;

    public DataGridViewComponentPropertyGridSite(IServiceProvider? sp, IComponent comp)
    {
        _sp = sp;
        Component = comp;
    }

    /// <summary>
    ///  When implemented by a class, gets the component associated with the <see cref="ISite"/>.
    /// </summary>
    public IComponent Component { get; }

    /// <summary>
    /// When implemented by a class, gets the container associated with the <see cref="ISite"/>.
    /// </summary>
    public IContainer? Container => null;

    /// <summary>
    ///  When implemented by a class, determines whether the component is in design mode.
    /// </summary>
    public bool DesignMode => false;

    /// <summary>
    ///  When implemented by a class, gets or sets the name of the component associated with the <see cref="ISite"/>.
    /// </summary>
    public string? Name { get; set; }

    public object? GetService(Type t)
    {
        if (_inGetService || _sp is null)
        {
            return null;
        }

        try
        {
            _inGetService = true;
            return _sp.GetService(t);
        }
        finally
        {
            _inGetService = false;
        }
    }
}
