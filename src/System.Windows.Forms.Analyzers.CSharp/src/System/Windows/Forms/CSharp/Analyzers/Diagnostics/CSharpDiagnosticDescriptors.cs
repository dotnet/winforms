// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.CSharp.Resources;
using System.Windows.Forms.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis;

namespace System.Windows.Forms.CSharp.Analyzers.Diagnostics;

internal static class CSharpDiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor s_errorUnsupportedProjectType =
        new(id: DiagnosticIDs.UnsupportedProjectType,
            title: new LocalizableResourceString(nameof(SR.WFO0001Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO0001Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.ApplicationConfiguration,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor s_propertyCantBeSetToValue =
        new(id: DiagnosticIDs.PropertyCantBeSetToValue,
            title: new LocalizableResourceString(nameof(SR.WFO0002Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO0002Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.ApplicationConfiguration,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor s_propertyCantBeSetToValueWithReason =
        new(id: DiagnosticIDs.PropertyCantBeSetToValue,
            title: new LocalizableResourceString(nameof(SR.WFO0002Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO0002MessageWithReason), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.ApplicationConfiguration,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor s_missingPropertySerializationConfiguration =
        new(id: DiagnosticIDs.MissingPropertySerializationConfiguration,
            title: new LocalizableResourceString(nameof(SR.WFO1000AnalyzerTitle), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO1000AnalyzerMessageFormat), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsSecurity,
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true,
            description: new LocalizableResourceString(nameof(SR.WFO1000AnalyzerDescription), SR.ResourceManager, typeof(SR)));

    private static readonly LocalizableString s_title =
        "Task or ValueTask passed to InvokeAsync without passing a cancellation token";

    private static readonly LocalizableString s_messageFormat =
        "Do not pass a Task or ValueTask as the return type of the Func<T> to InvokeAsync without a cancellation token";

    private static readonly LocalizableString s_description =
        "Task or ValueTask should not be passed to the InvokeAsync(Func<T>) method overload, " +
        "since it is not meant to return a type which implicates an async operation. " +
        "Use only overloads which also take a cancellation token for that purpose, " +
        "so that the passed function delegates can be awaited.";

    public static readonly DiagnosticDescriptor s_considerNotPassingFuncReturningTaskWithoutCancellationToken =
        new(DiagnosticIDs.ConsiderNotPassingFuncReturningTaskWithoutCancellationToken,
            s_title,
            s_messageFormat,
            DiagnosticCategories.WinFormsUsage,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true,
            description: s_description);
}
