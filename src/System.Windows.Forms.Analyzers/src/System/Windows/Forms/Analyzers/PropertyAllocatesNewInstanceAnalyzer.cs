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
///  Reports properties whose getter directly creates a new object on every access.
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
                ConcurrentDictionary<IPropertySymbol, byte> reportedProperties =
                    new(SymbolEqualityComparer.Default);

                startContext.RegisterOperationAction(
                    operationContext => AnalyzeReturn(operationContext, reportedProperties),
                    OperationKind.Return);
            });
    }

    private static void AnalyzeReturn(
        OperationAnalysisContext context,
        ConcurrentDictionary<IPropertySymbol, byte> reportedProperties)
    {
        var returnOperation = (IReturnOperation)context.Operation;
        if (context.ContainingSymbol is not IMethodSymbol method
            || method.MethodKind != MethodKind.PropertyGet
            || method.AssociatedSymbol is not IPropertySymbol property
            || property.IsIndexer
            || UnwrapConversion(returnOperation.ReturnedValue) is not IObjectCreationOperation
            || !reportedProperties.TryAdd(property, 0))
        {
            return;
        }

        Location? location = property.Locations.FirstOrDefault(item => item.IsInSource);
        if (location is not null)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    SharedDiagnosticDescriptors.s_propertyAllocatesNewInstance,
                    location,
                    property.Name));
        }
    }

    private static IOperation? UnwrapConversion(IOperation? operation)
    {
        while (operation is IConversionOperation conversion)
        {
            operation = conversion.Operand;
        }

        return operation;
    }
}
