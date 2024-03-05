// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API
[Obsolete("StatusBarPanelBorderStyle is obsolete.  Use the BorderStyle property of the StatusBarPanel class to change the border style of the StatusBarPanel.")]
public enum StatusBarPanelBorderStyle
{
    None = 1,

    Raised = 2,

    Sunken = 3,
}
