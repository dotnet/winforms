// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

public abstract class LayoutSettings
{
    protected LayoutSettings()
    {
    }

    internal LayoutSettings(IArrangedElement? owner)
    {
        Owner = owner;
    }

    public virtual LayoutEngine? LayoutEngine => null;

    internal IArrangedElement? Owner { get; }
}
