// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Composition;
using System.Windows.Forms.Analyzers.CodeFixes.Resources;
using System.Windows.Forms.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.Text;

namespace System.Windows.Forms.CSharp.CodeFixes.AddDesignerSerializationVisibility;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddDesignerSerializationVisibilityCodeFixProvider)), Shared]
internal sealed class AddDesignerSerializationVisibilityCodeFixProvider : CodeFixProvider
{
    private const string SystemComponentModelName = "System.ComponentModel";
    private const string DesignerSerializationVisibilityAttributeName = "DesignerSerializationVisibility";

    public sealed override ImmutableArray<string> FixableDiagnosticIds
        => [DiagnosticIDs.MissingPropertySerializationConfiguration];

    public sealed override FixAllProvider GetFixAllProvider()
        => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        // Cannot be null - otherwise we wouldn't have a diagnostic of that ID.
        SyntaxNode root = (await context
            .Document
            .GetSyntaxRootAsync(context.CancellationToken)
            .ConfigureAwait(false))!;

        Diagnostic diagnostic = context.Diagnostics.First();
        TextSpan diagnosticSpan = diagnostic.Location.SourceSpan;

        // Find the type declaration identified by the diagnostic.
        PropertyDeclarationSyntax? propertyDeclaration = root.FindToken(diagnosticSpan.Start)
            .Parent!
            .AncestorsAndSelf()
            .OfType<PropertyDeclarationSyntax>()
            .First();

        // Register a code action that will invoke the fix.
        context.RegisterCodeFix(
            CodeAction.Create(
                title: SR.AddDesignerSerializationVisibilityCodeFixTitle,
                createChangedDocument: c => AddDesignerSerializationAttribute(context.Document, propertyDeclaration, c),
                equivalenceKey: nameof(SR.AddDesignerSerializationVisibilityCodeFixTitle)),
            diagnostic);
    }

    private static async Task<Document> AddDesignerSerializationAttribute(
        Document document,
        PropertyDeclarationSyntax propertyDeclarationSyntax,
        CancellationToken cancellationToken)
    {
        if (propertyDeclarationSyntax is null)
        {
            return document;
        }

        // Let's make sure, the attribute we want to add is not already there:
        if (propertyDeclarationSyntax.AttributeLists
            .SelectMany(al => al.Attributes)
            .Any(a => a.Name.ToString() == DesignerSerializationVisibilityAttributeName))
        {
            // Already there, nothing to do.
            return document;
        }

        // Let's check, if we already have the using directive or if we need to add it:
        // (Remember: We can't throw here, as we are in a code fixer. But this also cannot be null.)
        SyntaxNode root = (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false))!;

        // Generate the Attribute we need to put on the property
        AttributeSyntax designerSerializationVisibilityAttribute = SyntaxFactory.Attribute(
            SyntaxFactory.ParseName(DesignerSerializationVisibilityAttributeName),
            SyntaxFactory.ParseAttributeArgumentList("(DesignerSerializationVisibility.Hidden)"));

        SyntaxTriviaList leadingTrivia = propertyDeclarationSyntax.GetLeadingTrivia();
        PropertyDeclarationSyntax newProperty = propertyDeclarationSyntax.WithoutLeadingTrivia();

        // Add the attribute to the property:
        newProperty = newProperty
            .AddAttributeLists(
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SingletonSeparatedList(designerSerializationVisibilityAttribute)));

        // Add the leading trivia back:
        newProperty = newProperty.WithLeadingTrivia(leadingTrivia);

        // Produce a new root, which has the updated property with the attribute.
        root = root.ReplaceNode(propertyDeclarationSyntax, newProperty);

        // Let's check if we already have the using directive:
        if (root.DescendantNodes()
            .OfType<UsingDirectiveSyntax>()
            .Any(u => u?.Name?.ToString() == SystemComponentModelName))
        {
            document = document.WithSyntaxRoot(root);
            return document;
        }

        // Let's check if we have _a_ using directive:
        bool hasUsings = root.DescendantNodes()
            .OfType<UsingDirectiveSyntax>()
            .FirstOrDefault() is not null;

        // Get the compilation unit:
        CompilationUnitSyntax compilationUnit = root
            .DescendantNodesAndSelf()
            .OfType<CompilationUnitSyntax>()
            .First();

        CompilationUnitSyntax originalCompilationUnit = compilationUnit;

        if (!hasUsings)
        {
            // We need to add a new line before the namespace/file-scoped-namespace declaration:
            SyntaxTriviaList? firstNodesLeadingTrivia = compilationUnit
                .DescendantNodes()
                .FirstOrDefault()
                ?.GetLeadingTrivia();

            compilationUnit = firstNodesLeadingTrivia is null
                ? compilationUnit
                    .WithLeadingTrivia(SyntaxFactory.CarriageReturnLineFeed)
                : compilationUnit
                    .WithLeadingTrivia(firstNodesLeadingTrivia.Value
                        .Add(SyntaxFactory.CarriageReturnLineFeed));
        }

        UsingDirectiveSyntax usingDirective = SyntaxFactory
            .UsingDirective(SyntaxFactory.ParseName(SystemComponentModelName));

        usingDirective = usingDirective
            .NormalizeWhitespace()
            .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed)
            .WithAdditionalAnnotations(Formatter.Annotation);

        // Generate the new document:
        document = document.WithSyntaxRoot(
            root.ReplaceNode(
                originalCompilationUnit,
                compilationUnit.AddUsings(usingDirective)));

        return document;
    }
}
