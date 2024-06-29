// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Analyzers.Diagnostics;

internal static class DiagnosticIDs
{
    // Application Configuration
    public const string UnsupportedProjectType = "WFCA001";
    public const string PropertyCantBeSetToValue = "WFCA002";
    public const string MigrateHighDpiSettings = "WFCA010";

    // Security
    public const string MissingPropertySerializationConfiguration = "WFCA100";

    // WinForms Usage
    public const string DisposeModalDialog = "WFCA500";
}
