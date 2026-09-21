// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Windows.Forms.Analyzers;
using System.Windows.Forms.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace System.Windows.Forms.CSharp.Analyzers.Designer;

/// <summary>
///  Reports control flow that should not be emitted in <c>InitializeComponent</c>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InitializeComponentAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        =>
        [
            SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode,
            SharedDiagnosticDescriptors.s_unsupportedNameOfExpression,
            SharedDiagnosticDescriptors.s_unsupportedConditionalExpression,
            SharedDiagnosticDescriptors.s_unsupportedNullCoalescingExpression,
            SharedDiagnosticDescriptors.s_unsupportedNullConditionalExpression,
            SharedDiagnosticDescriptors.s_unsupportedInterpolatedString,
            SharedDiagnosticDescriptors.s_unsupportedAnonymousFunction
        ];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(
            GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterCompilationStartAction(startContext =>
        {
            DesignerTypeFacts facts = new(startContext.Compilation);

            startContext.RegisterSyntaxNodeAction(
                context => AnalyzeInitializeComponent(context, facts),
                SyntaxKind.MethodDeclaration);
        });
    }

    private static void AnalyzeInitializeComponent(SyntaxNodeAnalysisContext context, DesignerTypeFacts facts)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (!DesignerTypeFacts.IsDesignerFile(method.SyntaxTree)
            || method.Identifier.ValueText != "InitializeComponent"
            || method.ParameterList.Parameters.Count != 0
            || method.ReturnType is not PredefinedTypeSyntax returnType
            || !returnType.Keyword.IsKind(SyntaxKind.VoidKeyword)
            || method.Modifiers.Any(SyntaxKind.StaticKeyword)
            || context.SemanticModel.GetDeclaredSymbol(
                method,
                context.CancellationToken) is not IMethodSymbol symbol
            || !DesignerTypeFacts.IsInitializeComponent(symbol)
            || !facts.IsDesignerDeclaration(symbol.ContainingType, method.SyntaxTree))
        {
            return;
        }

        if (method.ExpressionBody is { } expressionBody)
        {
            context.ReportDiagnostic(Diagnostic.Create(
                SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode,
                expressionBody.ArrowToken.GetLocation(),
                "expression-bodied method"));
        }

        SyntaxNode? body = (SyntaxNode?)method.Body ?? method.ExpressionBody;

        if (body is null)
        {
            return;
        }

        foreach (SyntaxNode node in body.DescendantNodes(descendIntoChildren: node => node is not AnonymousFunctionExpressionSyntax and not LocalFunctionStatementSyntax))
        {
            context.CancellationToken.ThrowIfCancellationRequested();

            if (TryGetUnsupportedConstruct(
                node,
                out SyntaxToken token,
                out DiagnosticDescriptor descriptor,
                out string? construct))
            {
                if (descriptor == SharedDiagnosticDescriptors.s_unsupportedNameOfExpression
                    && context.SemanticModel.GetOperation(node, context.CancellationToken) is not INameOfOperation)
                {
                    continue;
                }

                Diagnostic diagnostic = construct is null
                    ? Diagnostic.Create(descriptor, token.GetLocation())
                    : Diagnostic.Create(descriptor, token.GetLocation(), construct);

                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private static bool TryGetUnsupportedConstruct(
        SyntaxNode node,
        out SyntaxToken token,
        out DiagnosticDescriptor descriptor,
        out string? construct)
    {
        (token, descriptor, construct) = node switch
        {
            ForStatementSyntax statement
                => (statement.ForKeyword, SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode, "for loop"),

            ForEachStatementSyntax statement
                => (statement.ForEachKeyword, SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode, "foreach loop"),

            ForEachVariableStatementSyntax statement
                => (statement.ForEachKeyword, SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode, "foreach loop"),

            WhileStatementSyntax statement
                => (statement.WhileKeyword, SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode, "while loop"),

            DoStatementSyntax statement
                => (statement.DoKeyword, SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode, "do loop"),

            IfStatementSyntax statement
                => (statement.IfKeyword, SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode, "if statement"),

            SwitchStatementSyntax statement
                => (statement.SwitchKeyword, SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode, "switch statement"),

            SwitchExpressionSyntax expression
                => (expression.SwitchKeyword, SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode, "switch expression"),

            LocalFunctionStatementSyntax function
                => (function.Identifier, SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode, "local function"),

            GotoStatementSyntax statement
                => (statement.GotoKeyword, SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode, "goto statement"),

            InvocationExpressionSyntax invocation
                when invocation.Expression is IdentifierNameSyntax identifier
                    && identifier.Identifier.Text == "nameof"
                => (identifier.Identifier, SharedDiagnosticDescriptors.s_unsupportedNameOfExpression, null),

            ConditionalExpressionSyntax expression
                => (expression.QuestionToken, SharedDiagnosticDescriptors.s_unsupportedConditionalExpression, null),

            BinaryExpressionSyntax expression when expression.IsKind(SyntaxKind.CoalesceExpression)
                => (expression.OperatorToken, SharedDiagnosticDescriptors.s_unsupportedNullCoalescingExpression, null),

            AssignmentExpressionSyntax expression when expression.IsKind(SyntaxKind.CoalesceAssignmentExpression)
                => (expression.OperatorToken, SharedDiagnosticDescriptors.s_unsupportedNullCoalescingExpression, null),

            ConditionalAccessExpressionSyntax expression
                => (expression.OperatorToken, SharedDiagnosticDescriptors.s_unsupportedNullConditionalExpression, null),

            InterpolatedStringExpressionSyntax expression
                => (expression.StringStartToken, SharedDiagnosticDescriptors.s_unsupportedInterpolatedString, null),

            LambdaExpressionSyntax expression
                => (expression.ArrowToken, SharedDiagnosticDescriptors.s_unsupportedAnonymousFunction, null),

            AnonymousMethodExpressionSyntax expression
                => (expression.DelegateKeyword, SharedDiagnosticDescriptors.s_unsupportedAnonymousFunction, null),

            TryStatementSyntax statement
                => (statement.TryKeyword, SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode, "try statement"),

            LockStatementSyntax statement
                => (statement.LockKeyword, SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode, "lock statement"),

            UsingStatementSyntax statement
                => (statement.UsingKeyword, SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode, "using statement"),

            LocalDeclarationStatementSyntax statement when !statement.UsingKeyword.IsKind(SyntaxKind.None)
                => (statement.UsingKeyword, SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode, "using declaration"),

            AwaitExpressionSyntax expression
                => (expression.AwaitKeyword, SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode, "await expression"),
            _ => (default, null!, null)
        };

        return descriptor is not null;
    }
}
