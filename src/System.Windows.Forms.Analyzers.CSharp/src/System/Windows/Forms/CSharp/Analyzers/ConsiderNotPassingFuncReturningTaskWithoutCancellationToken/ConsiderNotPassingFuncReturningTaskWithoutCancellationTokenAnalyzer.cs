// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Windows.Forms.CSharp.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace System.Windows.Forms.CSharp.Analyzers.ConsiderNotPassingATaskWithoutCancellationToken;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ConsiderNotPassingFuncReturningTaskWithoutCancellationTokenAnalyzer : DiagnosticAnalyzer
{
    private const string InvokeAsyncString = "InvokeAsync";
    private const string TaskString = "Task";
    private const string ValueTaskString = "ValueTask";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => [CSharpDiagnosticDescriptors.s_considerNotPassingFuncReturningTaskWithoutCancellationToken];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocationExpr = (InvocationExpressionSyntax)context.Node;

        if (invocationExpr.Expression is not MemberAccessExpressionSyntax memberAccessExpr
            || context.SemanticModel.GetSymbolInfo(memberAccessExpr).Symbol is not IMethodSymbol methodSymbol
            || methodSymbol.Name != InvokeAsyncString || methodSymbol.Parameters.Length != 2)
        {
            return;
        }

        IParameterSymbol funcParameter = methodSymbol.Parameters[0];

        if (funcParameter.Type is not INamedTypeSymbol funcType
            || !funcType.ContainingNamespace.ToString().Equals("System"))
        {
            return;
        }

        if (funcType.DelegateInvokeMethod?.ReturnType is INamedTypeSymbol returnType)
        {
            if (returnType.Name is TaskString or ValueTaskString)
            {
                Diagnostic diagnostic = Diagnostic.Create(
                    CSharpDiagnosticDescriptors.s_considerNotPassingFuncReturningTaskWithoutCancellationToken,
                    invocationExpr.GetLocation());

                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
