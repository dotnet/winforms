// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms;

[Obsolete(
    Obsoletions.MenuMessage,
    error: false,
    DiagnosticId = Obsoletions.MenuDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
[EditorBrowsable(EditorBrowsableState.Never)]
public enum MenuMerge
{
    Add = 0,
    Replace = 1,
    MergeItems = 2,
    Remove = 3,
}
