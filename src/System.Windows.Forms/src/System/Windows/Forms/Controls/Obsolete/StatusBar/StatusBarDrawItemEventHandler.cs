// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#pragma warning disable RS0016
[Obsolete(
    Obsoletions.StatusBarDrawItemEventHandlerMessage,
    error: false,
    DiagnosticId = Obsoletions.StatusBarDrawItemEventHandlerDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public delegate void StatusBarDrawItemEventHandler(object sender, StatusBarDrawItemEventArgs sbdevent);
