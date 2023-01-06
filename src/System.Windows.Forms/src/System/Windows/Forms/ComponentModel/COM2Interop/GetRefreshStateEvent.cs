// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal delegate void Com2EventHandler(Com2PropertyDescriptor sender, EventArgs e);

internal class GetRefreshStateEvent : GetBoolValueEvent
{
    public GetRefreshStateEvent(bool defaultValue) : base(defaultValue) { }
}
