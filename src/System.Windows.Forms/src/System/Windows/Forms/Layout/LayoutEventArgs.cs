// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public sealed class LayoutEventArgs : EventArgs
{
    private readonly WeakReference<IComponent>? _affectedComponent;

    public LayoutEventArgs(IComponent? affectedComponent, string? affectedProperty)
    {
        _affectedComponent = affectedComponent is not null ? new(affectedComponent) : null;
        AffectedProperty = affectedProperty;
    }

    public LayoutEventArgs(Control? affectedControl, string? affectedProperty)
        : this((IComponent?)affectedControl, affectedProperty)
    {
    }

    public IComponent? AffectedComponent
    {
        get
        {
            IComponent? target = null;
            _affectedComponent?.TryGetTarget(out target);
            return target;
        }
    }

    public Control? AffectedControl => AffectedComponent as Control;

    public string? AffectedProperty { get; }
}
