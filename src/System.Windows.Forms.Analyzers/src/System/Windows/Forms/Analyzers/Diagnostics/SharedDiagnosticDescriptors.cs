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

    // WFO2007
    internal static readonly DiagnosticDescriptor s_unsupportedNameOfExpression =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnsupportedNameOfExpression,
            title: new LocalizableResourceString(nameof(SR.WFO2007Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO2007Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO2008
    internal static readonly DiagnosticDescriptor s_unsupportedConditionalExpression =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnsupportedConditionalExpression,
            title: new LocalizableResourceString(nameof(SR.WFO2008Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO2008Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO2009
    internal static readonly DiagnosticDescriptor s_unsupportedNullCoalescingExpression =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnsupportedNullCoalescingExpression,
            title: new LocalizableResourceString(nameof(SR.WFO2009Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO2009Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO2010
    internal static readonly DiagnosticDescriptor s_unsupportedNullConditionalExpression =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnsupportedNullConditionalExpression,
            title: new LocalizableResourceString(nameof(SR.WFO2010Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO2010Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO2011
    internal static readonly DiagnosticDescriptor s_unsupportedInterpolatedString =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnsupportedInterpolatedString,
            title: new LocalizableResourceString(nameof(SR.WFO2011Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO2011Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO2012
    internal static readonly DiagnosticDescriptor s_unsupportedAnonymousFunction =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnsupportedAnonymousFunction,
            title: new LocalizableResourceString(nameof(SR.WFO2012Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO2012Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO2013
    internal static readonly DiagnosticDescriptor s_propertyAllocatesNewInstance =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.PropertyAllocatesNewInstance,
            title: new LocalizableResourceString(nameof(SR.WFO2013Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO2013Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsUsage,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO2012 (Visual Basic)
    internal static readonly DiagnosticDescriptor s_visualBasicUnsupportedAnonymousFunction =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.UnsupportedAnonymousFunction,
            title: new LocalizableResourceString(nameof(SR.WFO2012Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO2012Message_VB), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO2014 (Visual Basic)
    internal static readonly DiagnosticDescriptor s_designerMissingWithEvents =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.DesignerMissingWithEvents,
            title: new LocalizableResourceString(nameof(SR.WFO2014Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO2014Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Warning);

    // WFO2015 (Visual Basic)
    internal static readonly DiagnosticDescriptor s_designerAddHandler =
        DiagnosticDescriptorHelper.Create(
            id: DiagnosticIDs.DesignerAddHandler,
            title: new LocalizableResourceString(nameof(SR.WFO2015Title), SR.ResourceManager, typeof(SR)),
            messageFormat: new LocalizableResourceString(nameof(SR.WFO2015Message), SR.ResourceManager, typeof(SR)),
            category: DiagnosticCategories.WinFormsDesigner,
            defaultSeverity: DiagnosticSeverity.Warning);
}
