// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.ComponentModel.Com2Interop.Tests;

/// <summary>
///  A <see cref="Com2PropertyDescriptor"/> whose target object is supplied directly by the test, so browsing
///  handlers can be exercised against a managed COM fixture without a real ActiveX control.
/// </summary>
internal sealed class FixturePropertyDescriptor : Com2PropertyDescriptor
{
    // The property manager only tracks its target weakly, so the descriptor keeps an anchor object alive for it.
    private readonly object _managerTarget = new();

    public FixturePropertyDescriptor(
        object? target,
        int dispid = 42,
        string name = "Value",
        Type? propertyType = null,
        Attribute[]? attributes = null,
        object? typeData = null)
        : base(dispid, name, attributes ?? [], readOnly: false, propertyType ?? typeof(string), typeData, hrHidden: false)
    {
        Target = target;
        PropertyManager = new Com2Properties(_managerTarget, [this], defaultIndex: -1);

        // Properties handed out by the manager are always marked for refresh, which is what makes the dynamic
        // attributes, converter and editor be queried.
        SetNeedsRefresh(Com2PropertyDescriptorRefresh.All, true);
    }

    public object? Target { get; set; }

    public override object? TargetObject => Target;

    // Handlers add their browsable decision to the base attributes; the final attribute collection can still
    // be overridden by the property type, so tests inspect the base attributes directly.
    public bool HandlerForcedBrowsable => BaseAttributes.Contains(BrowsableAttribute.Yes);
}
