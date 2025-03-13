// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Resources;
using Microsoft.CodeAnalysis;

namespace System.Windows.Forms.Analyzers.Diagnostics;

internal static partial class SharedDiagnosticDescriptors
{
    // WFO0003
    internal static readonly DiagnosticDescriptor s_cSharpMigrateHighDpiSettings =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.MigrateHighDpiSettings,
            title: new LocalizableResourceString(nameof(SR.WFO0003Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO0003Message_CS), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.ApplicationConfiguration,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO0003
    internal static readonly DiagnosticDescriptor s_visualBasicMigrateHighDpiSettings =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.MigrateHighDpiSettings,
            title: new LocalizableResourceString(nameof(SR.WFO0003Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO0003Message_VB), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.ApplicationConfiguration,
            defaultSeverity: DiagnosticSeverity.Warning);
}
