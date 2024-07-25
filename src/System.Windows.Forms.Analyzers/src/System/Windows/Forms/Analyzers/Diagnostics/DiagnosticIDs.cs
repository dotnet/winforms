// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Analyzers.Diagnostics;

internal static class DiagnosticIDs
{
    // Application Configuration, number group 0001+
    public const string UnsupportedProjectType = "WFO0001";
    public const string PropertyCantBeSetToValue = "WFO0002";
    public const string MigrateHighDpiSettings = "WFO0003";

    // Security, number group 1000+
    public const string MissingPropertySerializationConfiguration = "WFO1000";

    // WinForms best practize, number group 2000+
    public const string DisposeModalDialog = "WFO2000";

    // Experimental, number group 9000+
    public const string ExperimentalVisualStyles = "WFO9000";
    public const string ExperimentalDarkMode = "WFO9001";
}
