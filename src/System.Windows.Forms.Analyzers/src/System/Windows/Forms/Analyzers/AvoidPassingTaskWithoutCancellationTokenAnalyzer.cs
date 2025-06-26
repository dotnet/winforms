// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Windows.Forms.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace System.Windows.Forms.Analyzers.AvoidPassingTaskWithoutCancellationToken;

[DiagnosticAnalyzer(LanguageNames.CSharp, LanguageNames.VisualBasic)]
public class AvoidPassingTaskWithoutCancellationTokenAnalyzer : DiagnosticAnalyzer
{
    private const string InvokeAsyncString = "InvokeAsync";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => [SharedDiagnosticDescriptors.s_avoidPassingFuncReturningTaskWithoutCancellationToken];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterCompilationStartAction(context =>
        {
            ImmutableArray<INamedTypeSymbol>.Builder builder = ImmutableArray.CreateBuilder<INamedTypeSymbol>(4);
            AddIfNotNull(context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task"));
            AddIfNotNull(context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.Task`1"));
            AddIfNotNull(context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask"));
            AddIfNotNull(context.Compilation.GetTypeByMetadataName("System.Threading.Tasks.ValueTask`1"));

            INamedTypeSymbol? controlNamedTypeSymbol = context.Compilation.GetTypeByMetadataName("System.Windows.Forms.Control");
            var invokeAsyncSymbols = controlNamedTypeSymbol?.GetMembers(InvokeAsyncString)
                .OfType<IMethodSymbol>()
                .Where(m => m.Parameters.Length == 2 &&
                        m.Parameters[0].Type is INamedTypeSymbol callbackParameter &&
                        callbackParameter.DelegateInvokeMethod is { } delegateInvokeMethod &&
                        delegateInvokeMethod.Parameters.Length == 0)
                .ToImmutableArray();
            if (!invokeAsyncSymbols.HasValue || invokeAsyncSymbols.Value.IsDefaultOrEmpty)
            {
                return;
            }

            context.RegisterOperationAction(context => AnalyzeInvocation(context, invokeAsyncSymbols.Value, builder.ToImmutable()), OperationKind.Invocation);

            void AddIfNotNull(INamedTypeSymbol? typeSymbol)
            {
                if (typeSymbol is not null)
                {
                    builder.Add(typeSymbol);
                }
            }
        });
    }

    private static void AnalyzeInvocation(OperationAnalysisContext context, ImmutableArray<IMethodSymbol> invokeAsyncSymbols, ImmutableArray<INamedTypeSymbol> taskSymbols)
    {
        var invocation = (IInvocationOperation)context.Operation;
        IMethodSymbol? methodSymbol = invocation.TargetMethod;

        if (!invokeAsyncSymbols.Contains(methodSymbol.OriginalDefinition, SymbolEqualityComparer.Default))
        {
            return;
        }

        var funcType = (INamedTypeSymbol)methodSymbol.Parameters[0].Type;

        // And finally, let's check if the return type is Task or ValueTask, because those
        // can become now fire-and-forgets.
        if (funcType.DelegateInvokeMethod?.ReturnType is INamedTypeSymbol returnType
            && taskSymbols.Contains(returnType.OriginalDefinition))
        {
            Diagnostic diagnostic = Diagnostic.Create(
                SharedDiagnosticDescriptors.s_avoidPassingFuncReturningTaskWithoutCancellationToken,
                invocation.Syntax.GetLocation());

            context.ReportDiagnostic(diagnostic);
        }
    }
}
