// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API
[Obsolete(
    Obsoletions.ToolBarButtonClickEventHandlerMessage,
    error: false,
    DiagnosticId = Obsoletions.ToolBarButtonClickEventHandlerDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public delegate void ToolBarButtonClickEventHandler(object sender, ToolBarButtonClickEventArgs e);
