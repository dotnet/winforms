// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Windows.Forms.Analyzers;
using System.Windows.Forms.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace System.Windows.Forms.CSharp.Analyzers.Designer;

/// <summary>
///  Reports control flow that should not be emitted in <c>InitializeComponent</c>.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class InitializeComponentAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => [SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(
            GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterSyntaxNodeAction(
            AnalyzeInitializeComponent,
            SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeInitializeComponent(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (method.Identifier.ValueText != "InitializeComponent"
            || method.ParameterList.Parameters.Count != 0
            || method.ReturnType is not PredefinedTypeSyntax returnType
            || !returnType.Keyword.IsKind(SyntaxKind.VoidKeyword)
            || method.Modifiers.Any(SyntaxKind.StaticKeyword)
            || method.Body is null
            || method.Parent is not TypeDeclarationSyntax typeDeclaration
            || context.SemanticModel.GetDeclaredSymbol(
                typeDeclaration,
                context.CancellationToken) is not INamedTypeSymbol type
            || !DesignerTypeFacts.IsDesignerDeclaration(type, method.SyntaxTree))
        {
            return;
        }

        foreach (SyntaxNode node in method.Body.DescendantNodes())
        {
            if (TryGetUnsupportedConstruct(node, out SyntaxToken token, out string? construct))
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode,
                        token.GetLocation(),
                        construct));
            }
        }
    }

    private static bool TryGetUnsupportedConstruct(
        SyntaxNode node,
        out SyntaxToken token,
        out string? construct)
    {
        (token, construct) = node switch
        {
            ForStatementSyntax statement => (statement.ForKeyword, "for loop"),
            ForEachStatementSyntax statement => (statement.ForEachKeyword, "foreach loop"),
            WhileStatementSyntax statement => (statement.WhileKeyword, "while loop"),
            DoStatementSyntax statement => (statement.DoKeyword, "do loop"),
            IfStatementSyntax statement => (statement.IfKeyword, "if statement"),
            SwitchStatementSyntax statement => (statement.SwitchKeyword, "switch statement"),
            SwitchExpressionSyntax expression => (expression.SwitchKeyword, "switch expression"),
            LocalFunctionStatementSyntax function => (function.Identifier, "local function"),
            GotoStatementSyntax statement => (statement.GotoKeyword, "goto statement"),
            TryStatementSyntax statement => (statement.TryKeyword, "try statement"),
            LockStatementSyntax statement => (statement.LockKeyword, "lock statement"),
            _ => (default, null)
        };

        return construct is not null;
    }
}
