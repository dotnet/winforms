// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Resources;
using Microsoft.CodeAnalysis;

namespace System.Windows.Forms.Analyzers.Diagnostics;

internal static partial class SharedDiagnosticDescriptors
{
    internal static DiagnosticDescriptor CSharpMigrateHighDpiSettings { get; } =
        new(id: DiagnosticIDs.MigrateHighDpiSettings,
            title: new LocalizableResourceString(nameof(SR.WFCA010Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFCA010Message_CS), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.ApplicationConfiguration,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    internal static DiagnosticDescriptor VisualBasicMigrateHighDpiSettings { get; } =
        new(id: DiagnosticIDs.MigrateHighDpiSettings,
            title: new LocalizableResourceString(nameof(SR.WFCA010Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFCA010Message_VB), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.ApplicationConfiguration,
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true);
}
