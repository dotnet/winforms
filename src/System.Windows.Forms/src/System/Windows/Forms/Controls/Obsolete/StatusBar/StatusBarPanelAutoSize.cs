// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API
[Obsolete(
    Obsoletions.StatusBarPanelAutoSizeMessage,
    error: false,
    DiagnosticId = Obsoletions.StatusBarPanelAutoSizeDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public enum StatusBarPanelAutoSize
{
    None = 1,
    Spring = 2,
    Contents = 3,
}
