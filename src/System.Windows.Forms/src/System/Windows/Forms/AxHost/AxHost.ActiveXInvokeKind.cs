// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public abstract partial class AxHost
{
    public enum ActiveXInvokeKind
    {
        MethodInvoke,
        PropertyGet,
        PropertySet
    }
}
