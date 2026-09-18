// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
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
                ConcurrentDictionary<IFieldSymbol, byte> reportedFields =
                    new(SymbolEqualityComparer.Default);

                startContext.RegisterSyntaxNodeAction(
                    AnalyzeType,
                    SyntaxKind.ClassDeclaration);
                startContext.RegisterOperationAction(
                    operationContext => AnalyzeFieldReference(operationContext, reportedFields),
                    OperationKind.FieldReference);
            });
    }

    private static void AnalyzeType(SyntaxNodeAnalysisContext context)
    {
        var typeDeclaration = (ClassDeclarationSyntax)context.Node;
        if (context.SemanticModel.GetDeclaredSymbol(
            typeDeclaration,
            context.CancellationToken) is not INamedTypeSymbol type
            || !DesignerTypeFacts.IsDesignerDeclaration(type, typeDeclaration.SyntaxTree))
        {
            return;
        }

        AnalyzeFieldPlacement(context, typeDeclaration);
        AnalyzeCollectionExpressions(context, typeDeclaration);

        foreach (MemberDeclarationSyntax member in typeDeclaration.Members)
        {
            switch (member)
            {
                case FieldDeclarationSyntax:
                case ConstructorDeclarationSyntax:
                    break;

                case MethodDeclarationSyntax method when IsAllowedMethod(method):
                    break;

                case MethodDeclarationSyntax method when method.ExplicitInterfaceSpecifier is not null:
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

    private static void AnalyzeFieldReference(
        OperationAnalysisContext context,
        ConcurrentDictionary<IFieldSymbol, byte> reportedFields)
    {
        var fieldReference = (IFieldReferenceOperation)context.Operation;
        if (context.ContainingSymbol is not IMethodSymbol method
            || method.Name != "InitializeComponent"
            || method.IsStatic
            || !method.Parameters.IsEmpty
            || !method.ReturnsVoid
            || !DesignerTypeFacts.IsDesignerDeclaration(
                method.ContainingType,
                fieldReference.Syntax.SyntaxTree)
            || fieldReference.Field.DeclaringSyntaxReferences.IsEmpty
            || fieldReference.Field.DeclaringSyntaxReferences.Any(
                declaration => DesignerTypeFacts.IsDesignerFile(declaration.SyntaxTree))
            || !reportedFields.TryAdd(fieldReference.Field, 0))
        {
            return;
        }

        Location? declarationLocation = fieldReference.Field.Locations.FirstOrDefault(
            location => location.IsInSource);
        if (declarationLocation is not null)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    SharedDiagnosticDescriptors.s_designerFieldPlacement,
                    declarationLocation,
                    fieldReference.Field.Name));
        }
    }

    private static void AnalyzeCollectionExpressions(
        SyntaxNodeAnalysisContext context,
        ClassDeclarationSyntax typeDeclaration)
    {
        foreach (CollectionExpressionSyntax expression in typeDeclaration
            .DescendantNodes()
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
        ClassDeclarationSyntax typeDeclaration)
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
            if (typeDeclaration.Members[i] is not FieldDeclarationSyntax field
                || IsComponentsField(field, context.SemanticModel, context.CancellationToken))
            {
                continue;
            }

            foreach (VariableDeclaratorSyntax variable in field.Declaration.Variables)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        SharedDiagnosticDescriptors.s_designerFieldPlacement,
                        variable.Identifier.GetLocation(),
                        variable.Identifier.ValueText));
            }
        }
    }

    private static bool IsAllowedMethod(MethodDeclarationSyntax method)
    {
        if (method.ReturnType is not PredefinedTypeSyntax returnType
            || !returnType.Keyword.IsKind(SyntaxKind.VoidKeyword))
        {
            return false;
        }

        bool isInitializeComponent = method.Identifier.ValueText == "InitializeComponent"
            && method.ParameterList.Parameters.Count == 0;
        bool isDispose = method.Identifier.ValueText == "Dispose"
            && method.ParameterList.Parameters.Count == 1
            && method.ParameterList.Parameters[0].Type is PredefinedTypeSyntax parameterType
            && parameterType.Keyword.IsKind(SyntaxKind.BoolKeyword);

        return isInitializeComponent || isDispose;
    }

    private static bool IsComponentsField(
        FieldDeclarationSyntax field,
        SemanticModel semanticModel,
        CancellationToken cancellationToken)
    {
        if (field.Declaration.Variables.Count != 1)
        {
            return false;
        }

        VariableDeclaratorSyntax variable = field.Declaration.Variables[0];
        if (variable.Identifier.ValueText != "components")
        {
            return false;
        }

        return semanticModel.GetDeclaredSymbol(variable, cancellationToken) is IFieldSymbol
        {
            Type.Name: "IContainer",
            Type.ContainingNamespace.Name: "ComponentModel",
            Type.ContainingNamespace.ContainingNamespace.Name: "System"
        };
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
