// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Analyzers.Diagnostics;

internal static class DiagnosticIDs
{
    // Application Configuration
    public const string UnsupportedProjectType = "WFAC001";
    public const string PropertyCantBeSetToValue = "WFAC002";
    public const string MigrateHighDpiSettings = "WFAC010";

    // Security
    public const string ControlPropertySerialization = "WFAC100";

    // Memory Management
    public const string DisposeModalDialog = "WFAC200";
}
