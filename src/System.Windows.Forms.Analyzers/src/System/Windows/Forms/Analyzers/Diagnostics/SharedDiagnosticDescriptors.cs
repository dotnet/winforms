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

    // WFO3000
    internal static readonly DiagnosticDescriptor s_unsupportedInitializeComponentCode =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnsupportedInitializeComponentCode,
            title: new LocalizableResourceString(nameof(SR.WFO3000Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO3000Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Error);

    // WFO3001
    internal static readonly DiagnosticDescriptor s_unexpectedDesignerMember =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnexpectedDesignerMember,
            title: new LocalizableResourceString(nameof(SR.WFO3001Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO3001Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Error);

    // WFO3002
    internal static readonly DiagnosticDescriptor s_designerFieldPlacement =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.DesignerFieldPlacement,
            title: new LocalizableResourceString(nameof(SR.WFO3002Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO3002Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO3003
    internal static readonly DiagnosticDescriptor s_designerEventOrDelegate =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.DesignerEventOrDelegate,
            title: new LocalizableResourceString(nameof(SR.WFO3003Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO3003Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Error);

    // WFO3004
    internal static readonly DiagnosticDescriptor s_designerCollectionExpression =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.DesignerCollectionExpression,
            title: new LocalizableResourceString(nameof(SR.WFO3004Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO3004Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Error);

    // WFO3005
    internal static readonly DiagnosticDescriptor s_unsupportedNameOfExpression =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnsupportedNameOfExpression,
            title: new LocalizableResourceString(nameof(SR.WFO3005Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO3005Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO3006
    internal static readonly DiagnosticDescriptor s_unsupportedConditionalExpression =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnsupportedConditionalExpression,
            title: new LocalizableResourceString(nameof(SR.WFO3006Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO3006Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Error);

    // WFO3007
    internal static readonly DiagnosticDescriptor s_unsupportedNullCoalescingExpression =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnsupportedNullCoalescingExpression,
            title: new LocalizableResourceString(nameof(SR.WFO3007Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO3007Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Error);

    // WFO3008
    internal static readonly DiagnosticDescriptor s_unsupportedNullConditionalExpression =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnsupportedNullConditionalExpression,
            title: new LocalizableResourceString(nameof(SR.WFO3008Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO3008Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Error);

    // WFO3009
    internal static readonly DiagnosticDescriptor s_unsupportedInterpolatedString =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnsupportedInterpolatedString,
            title: new LocalizableResourceString(nameof(SR.WFO3009Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO3009Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO3010
    internal static readonly DiagnosticDescriptor s_unsupportedAnonymousFunction =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnsupportedAnonymousFunction,
            title: new LocalizableResourceString(nameof(SR.WFO3010Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO3010Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Error);

    // WFO2000
    internal static readonly DiagnosticDescriptor s_propertyAllocatesNewInstance =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.PropertyAllocatesNewInstance,
            title: new LocalizableResourceString(nameof(SR.WFO2000Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO2000Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsUsage,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO3010 (Visual Basic)
    internal static readonly DiagnosticDescriptor s_visualBasicUnsupportedAnonymousFunction =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnsupportedAnonymousFunction,
            title: new LocalizableResourceString(nameof(SR.WFO3010Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO3010Message_VB), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Error);

    // WFO3011 (Visual Basic)
    internal static readonly DiagnosticDescriptor s_designerMissingWithEvents =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.DesignerMissingWithEvents,
            title: new LocalizableResourceString(nameof(SR.WFO3011Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO3011Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Error);

    // WFO3012 (Visual Basic)
    internal static readonly DiagnosticDescriptor s_designerAddHandler =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.DesignerAddHandler,
            title: new LocalizableResourceString(nameof(SR.WFO3012Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO3012Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Error);
}
