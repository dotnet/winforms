// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API to simplify porting of applications from .NET Framework to .NET.
// These types will not work, but if they are not accessed, other features in the application will work.
[Obsolete(
    Obsoletions.MenuMergeMessage,
    error: false,
    DiagnosticId = Obsoletions.MenuMergeDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
public enum MenuMerge
{
    Add = 0,
    Replace = 1,
    MergeItems = 2,
    Remove = 3,
}
