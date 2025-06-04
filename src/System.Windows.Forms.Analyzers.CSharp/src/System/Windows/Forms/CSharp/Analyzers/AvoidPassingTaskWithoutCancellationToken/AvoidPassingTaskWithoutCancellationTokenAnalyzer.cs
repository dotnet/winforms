// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Windows.Forms.CSharp.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace System.Windows.Forms.CSharp.Analyzers.AvoidPassingTaskWithoutCancellationToken;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AvoidPassingTaskWithoutCancellationTokenAnalyzer : DiagnosticAnalyzer
{
    private const string InvokeAsyncString = "InvokeAsync";
    private const string TaskString = "Task";
    private const string ValueTaskString = "ValueTask";

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => [CSharpDiagnosticDescriptors.s_avoidPassingFuncReturningTaskWithoutCancellationToken];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocationExpr = (InvocationExpressionSyntax)context.Node;
        IMethodSymbol? methodSymbol = null;

        // Handle both explicit member access (this.InvokeAsync) and implicit method calls (InvokeAsync)
        if (invocationExpr.Expression is MemberAccessExpressionSyntax memberAccessExpr)
        {
            methodSymbol = context.SemanticModel.GetSymbolInfo(memberAccessExpr).Symbol as IMethodSymbol;
        }
        else if (invocationExpr.Expression is IdentifierNameSyntax identifierNameSyntax)
        {
            methodSymbol = context.SemanticModel.GetSymbolInfo(identifierNameSyntax).Symbol as IMethodSymbol;
        }

        if (methodSymbol is null || methodSymbol.Name != InvokeAsyncString || methodSymbol.Parameters.Length != 2)
        {
            return;
        }

        IParameterSymbol funcParameter = methodSymbol.Parameters[0];
        INamedTypeSymbol? containingType = methodSymbol.ContainingType;

        // If the function delegate has a parameter (which makes then 2 type arguments),
        // we can safely assume it's a CancellationToken, otherwise the compiler would have
        // complained before, because this is the only overload type we're accepting in a
        // func as a passed parameter.
        if (funcParameter.Type is not INamedTypeSymbol funcType
            || funcType.TypeArguments.Length != 1
            || !funcType.ContainingNamespace.ToString().Equals("System"))
        {
            return;
        }

        // Let's make absolute clear, we're dealing with InvokeAsync of Control.
        // For implicit calls, we check the containing type of the method itself.
        if (containingType is null || !IsAncestorOrSelfOfType(containingType, "System.Windows.Forms.Control"))
        {
            // For explicit calls, we need to check the instance type (from before)
            if (invocationExpr.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                TypeInfo objectTypeInfo = context.SemanticModel.GetTypeInfo(memberAccess.Expression);
                if (objectTypeInfo.Type is not INamedTypeSymbol objectType
                    || !IsAncestorOrSelfOfType(objectType, "System.Windows.Forms.Control"))
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }

        // And finally, let's check if the return type is Task or ValueTask, because those
        // can become now fire-and-forgets.
        if (funcType.DelegateInvokeMethod?.ReturnType is INamedTypeSymbol returnType
            && returnType.Name is TaskString or ValueTaskString)
        {
            Diagnostic diagnostic = Diagnostic.Create(
                CSharpDiagnosticDescriptors.s_avoidPassingFuncReturningTaskWithoutCancellationToken,
                invocationExpr.GetLocation());

            context.ReportDiagnostic(diagnostic);
        }

        // Helper method to check if a type is of a certain type or a derived type.
        static bool IsAncestorOrSelfOfType(INamedTypeSymbol? type, string typeName) =>
            type is not null
            && (type.ToString().Equals(typeName)
            || IsAncestorOrSelfOfType(type.BaseType, typeName));
    }
}
