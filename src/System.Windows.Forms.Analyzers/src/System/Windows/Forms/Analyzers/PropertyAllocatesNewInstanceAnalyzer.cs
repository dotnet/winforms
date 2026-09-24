// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Windows.Forms.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace System.Windows.Forms.Analyzers;

/// <summary>
///  Reports getter return paths that directly construct a reference instance.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
public sealed class PropertyAllocatesNewInstanceAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => [SharedDiagnosticDescriptors.s_propertyAllocatesNewInstance];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.RegisterCompilationStartAction(
            startContext =>
            {
                ConcurrentDictionary<IPropertySymbol, byte> reportedProperties = new(SymbolEqualityComparer.Default);
                startContext.RegisterOperationAction(
                    context => AnalyzeCreation(context, reportedProperties),
                    OperationKind.ObjectCreation,
                    OperationKind.ArrayCreation,
                    OperationKind.AnonymousObjectCreation);
            });
    }

    private static void AnalyzeCreation(
        OperationAnalysisContext context,
        ConcurrentDictionary<IPropertySymbol, byte> reportedProperties)
    {
        if (context.ContainingSymbol is not IMethodSymbol method
            || method.MethodKind != MethodKind.PropertyGet
            || method.AssociatedSymbol is not IPropertySymbol { IsIndexer: false } property
            || context.Operation.Type is not { IsReferenceType: true }
            || DesignerTypeFacts.IsInNestedFunction(context.Operation))
        {
            return;
        }

        IOperation value = context.Operation;
        while (value.Parent is IConversionOperation { OperatorMethod: null } or IParenthesizedOperation)
        {
            value = value.Parent;
        }

        if (value.Parent is not IReturnOperation || !reportedProperties.TryAdd(property, 0))
        {
            return;
        }

        Location? location = property.Locations.FirstOrDefault(item => item.IsInSource);
        if (location is not null)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                SharedDiagnosticDescriptors.s_propertyAllocatesNewInstance,
                location,
                property.Name));
        }
    }
}
