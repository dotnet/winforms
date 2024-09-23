// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.StatusBarDrawItemEventHandlerMessage,
    error: false,
    DiagnosticId = Obsoletions.StatusBarDrawItemEventHandlerDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
public delegate void StatusBarDrawItemEventHandler(object sender, StatusBarDrawItemEventArgs subevents);
