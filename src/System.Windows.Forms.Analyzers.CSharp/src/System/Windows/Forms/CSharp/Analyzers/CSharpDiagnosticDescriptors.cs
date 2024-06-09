// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis;

namespace System.Windows.Forms.CSharp.Analyzers;

internal static class CSharpDiagnosticDescriptors
{
    private const string Category = "ApplicationConfiguration";

    private static readonly LocalizableString s_localizableWFCA001Title
        = new LocalizableResourceString(nameof(SR.WFCA001Title), SR.ResourceManager, typeof(SR));

    private static readonly LocalizableString s_localizableWFAC001Message
        = new LocalizableResourceString(nameof(SR.WFCA001Message), SR.ResourceManager, typeof(SR));

    private static readonly LocalizableString s_localizableWFCA002Title
        = new LocalizableResourceString(nameof(SR.WFCA002Title), SR.ResourceManager, typeof(SR));

    private static readonly LocalizableString s_localizableWFAC002Message
        = new LocalizableResourceString(nameof(SR.WFCA002Message), SR.ResourceManager, typeof(SR));

    private static readonly LocalizableString s_localizableWFAC002MessageWithReason
        = new LocalizableResourceString(nameof(SR.WFCA002MessageWithReason), SR.ResourceManager, typeof(SR));

    public static readonly DiagnosticDescriptor s_errorUnsupportedProjectType
        = new(id: DiagnosticIDs.UnsupportedProjectType,
              title: s_localizableWFCA001Title,
              messageFormat: s_localizableWFAC001Message,
              category: Category,
              defaultSeverity: DiagnosticSeverity.Error,
              isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor s_propertyCantBeSetToValue
        = new(id: DiagnosticIDs.PropertyCantBeSetToValue,
              title: s_localizableWFCA002Title,
              messageFormat: s_localizableWFAC002Message,
              category: Category,
              defaultSeverity: DiagnosticSeverity.Error,
              isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor s_propertyCantBeSetToValueWithReason
        = new(id: DiagnosticIDs.PropertyCantBeSetToValue,
              title: s_localizableWFCA002Title,
              messageFormat: s_localizableWFAC002MessageWithReason,
              category: Category,
              defaultSeverity: DiagnosticSeverity.Error,
              isEnabledByDefault: true);
}
