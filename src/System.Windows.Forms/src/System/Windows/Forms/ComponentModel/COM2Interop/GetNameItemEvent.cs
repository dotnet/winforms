// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal delegate void GetNameItemEventHandler(Com2PropertyDescriptor sender, GetNameItemEvent e);

internal class GetNameItemEvent : EventArgs
{
    public GetNameItemEvent(object? defaultName) => Name = defaultName;

    public object? Name { get; set; }

    public string NameString => Name?.ToString() ?? string.Empty;
}
