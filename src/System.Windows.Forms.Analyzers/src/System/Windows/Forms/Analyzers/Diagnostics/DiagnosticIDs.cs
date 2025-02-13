// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Analyzers.Diagnostics;

internal static class DiagnosticIDs
{
    public const string UrlFormat = "https://aka.ms/winforms-warnings/{0}";

    // Application Configuration, number group 0001+
    public const string UnsupportedProjectType = "WFO0001";
    public const string PropertyCantBeSetToValue = "WFO0002";
    public const string MigrateHighDpiSettings = "WFO0003";

    // WinForms Security, number group 1000+
    public const string MissingPropertySerializationConfiguration = "WFO1000";
    public const string ImplementITypedDataObject = "WFO1001";

    // WinForms best practice, number group 2000+
    public const string DisposeModalDialog = "WFO2000";
    public const string AvoidPassingFuncReturningTaskWithoutCancellationToken = "WFO2001";

    // Experimental, number group 5000+
    public const string ExperimentalDarkMode = "WFO5001";
    public const string ExperimentalAsync = "WFO5002";
}
