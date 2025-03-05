// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class Control
{
    /// <summary>
    ///  This record contains a control and associates it with a z-order.
    ///  This is used when sorting controls based on tab index first, z-order second.
    /// </summary>
    private readonly record struct ControlTabOrderHolder(int OriginalIndex, int TabIndex, Control? Control)
    {
    }
}
