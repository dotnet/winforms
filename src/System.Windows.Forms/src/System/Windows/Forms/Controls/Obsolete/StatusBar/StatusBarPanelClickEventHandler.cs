﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.StatusBarPanelClickEventHandlerMessage,
    error: false,
    DiagnosticId = Obsoletions.StatusBarPanelClickEventHandlerDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat),
    EditorBrowsable(EditorBrowsableState.Never)]
public delegate void StatusBarPanelClickEventHandler(object sender, StatusBarPanelClickEventArgs e);
