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
///  Enforces the structural boundary between generated Designer code and user code.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DesignerFileStructureAnalyzer : DiagnosticAnalyzer
{
    /// <inheritdoc/>
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        =>
        [
            SharedDiagnosticDescriptors.s_unexpectedDesignerMember,
            SharedDiagnosticDescriptors.s_designerFieldPlacement,
            SharedDiagnosticDescriptors.s_designerEventOrDelegate,
            SharedDiagnosticDescriptors.s_designerCollectionExpression
        ];

    /// <inheritdoc/>
    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(
            GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
        context.RegisterCompilationStartAction(
            startContext =>
            {
                DesignerTypeFacts facts = new(startContext.Compilation);

                startContext.RegisterSyntaxNodeAction(
                    context => AnalyzeType(context, facts),
                    SyntaxKind.ClassDeclaration);
            });
    }

    private static void AnalyzeType(SyntaxNodeAnalysisContext context, DesignerTypeFacts facts)
    {
        var typeDeclaration = (ClassDeclarationSyntax)context.Node;
        if (!DesignerTypeFacts.IsDesignerFile(typeDeclaration.SyntaxTree)
            || context.SemanticModel.GetDeclaredSymbol(
            typeDeclaration,
            context.CancellationToken) is not INamedTypeSymbol type
            || !facts.IsDesignerDeclaration(type, typeDeclaration.SyntaxTree))
        {
            return;
        }

        AnalyzeFieldPlacement(context, typeDeclaration, facts);
        AnalyzeCollectionExpressions(context, typeDeclaration);

        foreach (MemberDeclarationSyntax member in typeDeclaration.Members)
        {
            switch (member)
            {
                case FieldDeclarationSyntax:
                case ConstructorDeclarationSyntax:
                    break;

                case MethodDeclarationSyntax method when context.SemanticModel.GetDeclaredSymbol(
                    method, context.CancellationToken) is IMethodSymbol symbol
                    && DesignerTypeFacts.IsAllowedMethod(symbol):
                    if (DesignerTypeFacts.IsInitializeComponent(symbol)
                        && context.SemanticModel.GetOperation(method, context.CancellationToken) is IOperation operation)
                    {
                        AnalyzeInitializers(context, symbol, operation, facts);
                    }

                    break;

                case EventFieldDeclarationSyntax eventField:
                    foreach (VariableDeclaratorSyntax variable in eventField.Declaration.Variables)
                    {
                        ReportEventOrDelegate(context, variable.Identifier, "Event");
                    }

                    break;

                case EventDeclarationSyntax eventDeclaration:
                    ReportEventOrDelegate(context, eventDeclaration.Identifier, "Event");
                    break;

                case DelegateDeclarationSyntax delegateDeclaration:
                    ReportEventOrDelegate(context, delegateDeclaration.Identifier, "Delegate");
                    break;

                default:
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            SharedDiagnosticDescriptors.s_unexpectedDesignerMember,
                            GetMemberLocation(member),
                            GetMemberName(member)));
                    break;
            }
        }
    }

    private static void AnalyzeInitializers(
        SyntaxNodeAnalysisContext context,
        IMethodSymbol method,
        IOperation body,
        DesignerTypeFacts facts)
    {
        HashSet<ISymbol> reportedMembers = new(SymbolEqualityComparer.Default);
        foreach (IOperation operation in body.DescendantsAndSelf())
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            if (operation is not ISimpleAssignmentOperation assignment
                || facts.GetInitializedComponent(assignment, method.ContainingType) is not ISymbol member
                || member.DeclaringSyntaxReferences.Any(
                    declaration => DesignerTypeFacts.IsDesignerFile(declaration.SyntaxTree))
                || !reportedMembers.Add(member))
            {
                continue;
            }

            Location? location = member.Locations.FirstOrDefault(location => location.IsInSource);

            if (location is not null)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    SharedDiagnosticDescriptors.s_designerFieldPlacement, location, member.Name));
            }
        }
    }

    private static void AnalyzeCollectionExpressions(
        SyntaxNodeAnalysisContext context,
        ClassDeclarationSyntax typeDeclaration)
    {
        foreach (CollectionExpressionSyntax expression in typeDeclaration
            .DescendantNodes(node => node == typeDeclaration || node is not TypeDeclarationSyntax)
            .OfType<CollectionExpressionSyntax>())
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    SharedDiagnosticDescriptors.s_designerCollectionExpression,
                    expression.OpenBracketToken.GetLocation()));
        }
    }

    private static void AnalyzeFieldPlacement(
        SyntaxNodeAnalysisContext context,
        ClassDeclarationSyntax typeDeclaration,
        DesignerTypeFacts facts)
    {
        int lastNonFieldIndex = -1;

        for (int i = 0; i < typeDeclaration.Members.Count; i++)
        {
            if (typeDeclaration.Members[i] is not FieldDeclarationSyntax)
            {
                lastNonFieldIndex = i;
            }
        }

        for (int i = 0; i < lastNonFieldIndex; i++)
        {
            if (typeDeclaration.Members[i] is not FieldDeclarationSyntax field)
            {
                continue;
            }

            foreach (VariableDeclaratorSyntax variable in field.Declaration.Variables)
            {
                if (context.SemanticModel.GetDeclaredSymbol(variable, context.CancellationToken) is not IFieldSymbol symbol
                    || facts.IsComponentsMember(symbol))
                {
                    continue;
                }

                context.ReportDiagnostic(
                    Diagnostic.Create(
                        SharedDiagnosticDescriptors.s_designerFieldPlacement,
                        variable.Identifier.GetLocation(),
                        variable.Identifier.ValueText));
            }
        }
    }

    private static string GetMemberName(MemberDeclarationSyntax member)
        => member switch
        {
            PropertyDeclarationSyntax property => property.Identifier.ValueText,
            IndexerDeclarationSyntax => "this",
            MethodDeclarationSyntax method => method.Identifier.ValueText,
            BaseTypeDeclarationSyntax type => type.Identifier.ValueText,
            _ => member.Kind().ToString()
        };

    private static Location GetMemberLocation(MemberDeclarationSyntax member)
        => member switch
        {
            PropertyDeclarationSyntax property => property.Identifier.GetLocation(),
            MethodDeclarationSyntax method => method.Identifier.GetLocation(),
            BaseTypeDeclarationSyntax type => type.Identifier.GetLocation(),
            _ => member.GetLocation()
        };

    private static void ReportEventOrDelegate(
        SyntaxNodeAnalysisContext context,
        SyntaxToken identifier,
        string kind)
        => context.ReportDiagnostic(
            Diagnostic.Create(
                SharedDiagnosticDescriptors.s_designerEventOrDelegate,
                identifier.GetLocation(),
                kind,
                identifier.ValueText));
}
