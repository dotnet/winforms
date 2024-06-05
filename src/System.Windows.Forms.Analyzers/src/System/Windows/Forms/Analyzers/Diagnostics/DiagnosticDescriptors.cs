// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Resources;
using Microsoft.CodeAnalysis;

namespace System.Windows.Forms.Analyzers.Diagnostics;

internal static partial class DiagnosticDescriptors
{
    private const string Category = "ApplicationConfiguration";

    private static readonly LocalizableString s_localizableWFAC010Title
        = new LocalizableResourceString(nameof(SR.WFAC010Title), SR.ResourceManager, typeof(SR));

    private static readonly LocalizableString s_localizableWFAC010Message_CS
        = new LocalizableResourceString(nameof(SR.WFAC010Message_CS), SR.ResourceManager, typeof(SR));

    private static readonly LocalizableString s_localizableWFAC010Message_VB
        = new LocalizableResourceString(nameof(SR.WFAC010Message_VB), SR.ResourceManager, typeof(SR));

    internal static readonly DiagnosticDescriptor s_migrateHighDpiSettings_CSharp
       = new(id: DiagnosticIDs.MigrateHighDpiSettings,
             title: s_localizableWFAC010Title,
             messageFormat: s_localizableWFAC010Message_CS,
             category: Category,
             defaultSeverity: DiagnosticSeverity.Warning,
             isEnabledByDefault: true);

    internal static readonly DiagnosticDescriptor s_migrateHighDpiSettings_VB
       = new(id: DiagnosticIDs.MigrateHighDpiSettings,
             title: s_localizableWFAC010Title,
             messageFormat: s_localizableWFAC010Message_VB,
             category: Category,
             defaultSeverity: DiagnosticSeverity.Warning,
             isEnabledByDefault: true);
}
