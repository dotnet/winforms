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

    // WFO2002
    internal static readonly DiagnosticDescriptor s_unsupportedInitializeComponentCode =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnsupportedInitializeComponentCode,
            title: new LocalizableResourceString(nameof(SR.WFO2002Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO2002Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO2003
    internal static readonly DiagnosticDescriptor s_unexpectedDesignerMember =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnexpectedDesignerMember,
            title: new LocalizableResourceString(nameof(SR.WFO2003Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO2003Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO2004
    internal static readonly DiagnosticDescriptor s_designerFieldPlacement =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.DesignerFieldPlacement,
            title: new LocalizableResourceString(nameof(SR.WFO2004Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO2004Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO2005
    internal static readonly DiagnosticDescriptor s_designerEventOrDelegate =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.DesignerEventOrDelegate,
            title: new LocalizableResourceString(nameof(SR.WFO2005Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO2005Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO2006
    internal static readonly DiagnosticDescriptor s_designerCollectionExpression =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.DesignerCollectionExpression,
            title: new LocalizableResourceString(nameof(SR.WFO2006Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO2006Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Warning);
}
