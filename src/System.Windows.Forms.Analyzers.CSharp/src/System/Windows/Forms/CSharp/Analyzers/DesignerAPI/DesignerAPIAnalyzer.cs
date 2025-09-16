// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace System.Windows.Forms.CSharp.Analyzers.DesignerAPI;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DesignerAPIAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "PARENTDESIGNER001";
    private const string Title = "Use updated Winform Designer API";
    private const string MessageFormat =
        "Type '{0}' is not recommended; use '{1}' instead. Before using new API male sure 'Microsoft.WinForms.Designer.SDK' nuget package is installed.";
    private const string Category = "Usage";
    private static readonly Dictionary<string, string> s_classReplacements = new Dictionary<string, string>();

    static DesignerAPIAnalyzer()
    {
        s_classReplacements.Add("System.Windows.Forms.Design.ParentControlDesigner", "Microsoft.DotNet.DesignTools.Designers.ParentControlDesigner");
        s_classReplacements.Add("System.Windows.Forms.Design.ControlDesigner", "Microsoft.DotNet.DesignTools.Designers.ControlDesigner");

    }

    private static readonly DiagnosticDescriptor s_rule =
        new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            Category,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => [s_rule];

    public override void Initialize(AnalysisContext context)
    {
        // Register for identifier references (typeof, casts, attributes, nameof)
        context.RegisterSyntaxNodeAction(
            AnalyzeIdentifierName,
            SyntaxKind.IdentifierName);
    }

    private static void AnalyzeIdentifierName(SyntaxNodeAnalysisContext context)
    {
        var idNode = (IdentifierNameSyntax)context.Node;

        if (context.SemanticModel.GetSymbolInfo(idNode).Symbol is not INamedTypeSymbol symbol)
        {
            return;
        }

        foreach (var pair in s_classReplacements)
        {
            if (symbol.ToDisplayString() == pair.Key)
            {
                var diagnostic = Diagnostic.Create(
                    s_rule,
                    idNode.GetLocation(),
                    symbol.Name, pair.Value);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}
