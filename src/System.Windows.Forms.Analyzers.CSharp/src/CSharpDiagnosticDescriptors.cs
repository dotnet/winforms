// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;

namespace System.Windows.Forms.Analyzers.CSharp;

internal static class CSharpDiagnosticDescriptors
{
    private const string Category = "ApplicationConfiguration";

    private static readonly LocalizableString s_localizableWFAC001Title
        = new LocalizableResourceString(nameof(SR.WFAC001Title), SR.ResourceManager, typeof(SR));
    private static readonly LocalizableString s_localizableWFAC001Message
        = new LocalizableResourceString(nameof(SR.WFAC001Message), SR.ResourceManager, typeof(SR));
    private static readonly LocalizableString s_localizableWFAC002Title
        = new LocalizableResourceString(nameof(SR.WFAC002Title), SR.ResourceManager, typeof(SR));
    private static readonly LocalizableString s_localizableWFAC002Message
        = new LocalizableResourceString(nameof(SR.WFAC002Message), SR.ResourceManager, typeof(SR));
    private static readonly LocalizableString s_localizableWFAC002MessageWithReason
        = new LocalizableResourceString(nameof(SR.WFAC002MessageWithReason), SR.ResourceManager, typeof(SR));

    public static readonly DiagnosticDescriptor s_errorUnsupportedProjectType
        = new(id: "WFAC001",
              title: s_localizableWFAC001Title,
              messageFormat: s_localizableWFAC001Message,
              category: Category,
              defaultSeverity: DiagnosticSeverity.Error,
              isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor s_propertyCantBeSetToValue
        = new(id: "WFAC002",
              title: s_localizableWFAC002Title,
              messageFormat: s_localizableWFAC002Message,
              category: Category,
              defaultSeverity: DiagnosticSeverity.Error,
              isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor s_propertyCantBeSetToValueWithReason
        = new(id: "WFAC002",
              title: s_localizableWFAC002Title,
#pragma warning disable RS1032 // Define diagnostic message correctly. Justification - exception messages end with a comma.
              messageFormat: s_localizableWFAC002MessageWithReason,
#pragma warning restore RS1032 // Define diagnostic message correctly
              category: Category,
              defaultSeverity: DiagnosticSeverity.Error,
              isEnabledByDefault: true);
}
