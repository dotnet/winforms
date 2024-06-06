// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

public interface IBindableComponent : IComponent
{
    internal const string ComponentModelTrimIncompatibilityMessage = "Binding is not supported with trimming";
    ControlBindingsCollection DataBindings { get; }
    BindingContext? BindingContext
    {
        get;
        set;
    }
}
