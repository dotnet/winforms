// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.CSharp.Resources;
using System.Windows.Forms.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis;

namespace System.Windows.Forms.CSharp.Analyzers.Diagnostics;

internal static class CSharpDiagnosticDescriptors
{
    // WFO0001
    public static readonly DiagnosticDescriptor s_errorUnsupportedProjectType =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnsupportedProjectType,
            title: new LocalizableResourceString(nameof(SR.WFO0001Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO0001Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.ApplicationConfiguration,
            defaultSeverity: DiagnosticSeverity.Error);

    // WFO0002
    public static readonly DiagnosticDescriptor s_propertyCantBeSetToValue =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.PropertyCantBeSetToValue,
            title: new LocalizableResourceString(nameof(SR.WFO0002Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO0002Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.ApplicationConfiguration,
            defaultSeverity: DiagnosticSeverity.Error);

    // WFO0002
    public static readonly DiagnosticDescriptor s_propertyCantBeSetToValueWithReason =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.PropertyCantBeSetToValue,
            title: new LocalizableResourceString(nameof(SR.WFO0002Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO0002MessageWithReason), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.ApplicationConfiguration,
            defaultSeverity: DiagnosticSeverity.Error);

    // WFO1000
    public static readonly DiagnosticDescriptor s_missingPropertySerializationConfiguration =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.MissingPropertySerializationConfiguration,
            title: new LocalizableResourceString(nameof(SR.WFO1000AnalyzerTitle), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO1000AnalyzerMessageFormat), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsSecurity,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(SR.WFO1000AnalyzerDescription), SR.ResourceManager, typeof(SR)));

    // WFO1001
    public static readonly DiagnosticDescriptor s_implementITypedDataObjectInAdditionToIDataObject =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.ImplementITypedDataObject,
            title: new LocalizableResourceString(nameof(SR.WFO1001AnalyzerTitle), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO1001AnalyzerMessageFormat), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsSecurity,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(SR.WFO1001AnalyzerDescription), SR.ResourceManager, typeof(SR)));

    // WFO20001
    public static readonly DiagnosticDescriptor s_avoidPassingFuncReturningTaskWithoutCancellationToken =
        DiagnosticDescriptorHelper.Create(
            DiagnosticIDs.AvoidPassingFuncReturningTaskWithoutCancellationToken,
            title: new LocalizableResourceString(nameof(SR.WFO2001AnalyzerTitle), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO2001AnalyzerMessageFormat), SR.ResourceManager, typeof(SR)),
            DiagnosticCategories.WinFormsUsage,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(SR.WFO2001AnalyzerDescription), SR.ResourceManager, typeof(SR)));
}
