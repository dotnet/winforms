// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal delegate void GetBoolValueEventHandler(Com2PropertyDescriptor sender, GetBoolValueEvent e);

internal class GetBoolValueEvent : EventArgs
{
    public GetBoolValueEvent(bool defaultValue) => Value = defaultValue;
    public bool Value { get; set; }
}
