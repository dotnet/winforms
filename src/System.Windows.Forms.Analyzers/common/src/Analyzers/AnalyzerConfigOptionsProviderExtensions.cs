// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis.Diagnostics;

namespace System.Windows.Forms.Analyzers;

internal static class AnalyzerConfigOptionsProviderExtensions
{
    /// <summary>
    /// Attempts to read a value for the requested MSBuild property.
    /// </summary>
    /// <param name="analyzerConfigOptions">The global options.</param>
    /// <param name="name">The name of the property to read the value for.</param>
    /// <param name="value">The property's value.</param>
    /// <returns><see langword="true"/> if the property is present; otherwise <see langword="true"/>.</returns>
    public static bool GetMSBuildProperty(this AnalyzerConfigOptionsProvider analyzerConfigOptions, string name, out string? value)
    {
        return analyzerConfigOptions.GlobalOptions.TryGetValue($"build_property.{name}", out value);
    }
}
