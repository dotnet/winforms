﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

#nullable disable

[Obsolete(
    Obsoletions.StatusBarMessage,
    error: false,
    DiagnosticId = Obsoletions.UnsupportedControlsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
[Browsable(false)]
public delegate void StatusBarPanelClickEventHandler(object sender, StatusBarPanelClickEventArgs e);
