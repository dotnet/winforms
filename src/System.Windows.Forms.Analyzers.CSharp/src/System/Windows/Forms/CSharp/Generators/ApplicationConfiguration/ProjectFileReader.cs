// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers;
using System.Windows.Forms.CSharp.Analyzers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static System.Windows.Forms.Analyzers.ApplicationConfig;

namespace System.Windows.Forms.CSharp.Generators.ApplicationConfiguration;

internal static partial class ProjectFileReader
{
    public static IncrementalValueProvider<(ApplicationConfig? ApplicationConfig, Diagnostic? Diagnostic)> ReadApplicationConfig(IncrementalValueProvider<AnalyzerConfigOptionsProvider> configOptionsProvider)
        => configOptionsProvider.Select(
            (analyzerConfigOptions, cancellationToken) =>
            {
                if (!TryReadBool(analyzerConfigOptions, PropertyNameCSharp.EnableVisualStyles,
                                 defaultValue: PropertyDefaultValue.EnableVisualStyles,
                                 out bool enableVisualStyles, out Diagnostic? diagnostic)
                    || !TryReadBool(analyzerConfigOptions, PropertyNameCSharp.UseCompatibleTextRendering,
                                 defaultValue: PropertyDefaultValue.UseCompatibleTextRendering,
                                 out bool useCompatibleTextRendering, out diagnostic)
                    || !TryReadFont(analyzerConfigOptions, out FontDescriptor? font, out diagnostic)
                    || !TryReadHighDpiMode(analyzerConfigOptions, out HighDpiMode highDpiMode, out diagnostic))
                {
                    return ((ApplicationConfig?)null, diagnostic);
                }

                ApplicationConfig projectConfig = new(
                    EnableVisualStyles: enableVisualStyles,
                    DefaultFont: font?.ToString(),
                    HighDpiMode: highDpiMode,
                    UseCompatibleTextRendering: useCompatibleTextRendering);

                return (projectConfig, null);
            });

    private static bool TryReadBool(AnalyzerConfigOptionsProvider configOptions, string propertyName, bool defaultValue, out bool value, out Diagnostic? diagnostic)
    {
        value = defaultValue;
        diagnostic = null;

        if (!configOptions.GetMSBuildProperty(propertyName, out string? rawValue) ||
            rawValue == string.Empty)
        {
            // The property is either not defined explicitly, or the value is "". All good, use the default.
            return true;
        }

        if (!bool.TryParse(rawValue, out value))
        {
            diagnostic = Diagnostic.Create(CSharpDiagnosticDescriptors.s_propertyCantBeSetToValue,
                                           Location.None,
                                           propertyName,
                                           rawValue);
            value = defaultValue;
            return false;
        }

        return true;
    }

    private static bool TryReadFont(AnalyzerConfigOptionsProvider configOptions, out FontDescriptor? font, out Diagnostic? diagnostic)
    {
        font = null;
        diagnostic = null;

        if (!configOptions.GetMSBuildProperty(PropertyNameCSharp.DefaultFont, out string? rawValue) ||
            rawValue == string.Empty)
        {
            // The property is either not defined explicitly, or the value is "". All good, use the default.
            return true;
        }

        try
        {
            // In .NET runtime the font is validated via GDI+ to see whether it can be mapped to a valid font family.
            // We don't have access to Font (though we can with some gymnastics) or FontConverter (at all), so our
            // font validation logic is not as exhaustive as it is in .NET runtime.
            // With that it is possible that the value is not a valid font (e.g. 'Style=Bold' or '11px'), and which
            // will lead to runtime failures when we execute SetDefaultFont(new FontFamily('Style=Bold')).
            font = FontConverter.ConvertFrom(rawValue!);
            return true;
        }
        catch (Exception ex)
        {
            diagnostic = Diagnostic.Create(CSharpDiagnosticDescriptors.s_propertyCantBeSetToValueWithReason,
                                           Location.None,
                                           PropertyNameCSharp.DefaultFont,
                                           rawValue,
                                           ex.Message);
        }

        return false;
    }

    private static bool TryReadHighDpiMode(AnalyzerConfigOptionsProvider configOptions, out HighDpiMode highDpiMode, out Diagnostic? diagnostic)
    {
        highDpiMode = PropertyDefaultValue.DpiMode;
        diagnostic = null;

        if (!configOptions.GetMSBuildProperty(PropertyNameCSharp.HighDpiMode, out string? rawValue) ||
            rawValue == string.Empty)
        {
            // The property is either not defined explicitly, or the value is "". All good, use the default.
            return true;
        }

        if (!Enum.TryParse(rawValue, true, out highDpiMode) ||
            !Enum.IsDefined(typeof(HighDpiMode), highDpiMode))
        {
            diagnostic = Diagnostic.Create(CSharpDiagnosticDescriptors.s_propertyCantBeSetToValue,
                                           Location.None,
                                           PropertyNameCSharp.HighDpiMode,
                                           rawValue);
            highDpiMode = PropertyDefaultValue.DpiMode;
            return false;
        }

        return true;
    }
}
