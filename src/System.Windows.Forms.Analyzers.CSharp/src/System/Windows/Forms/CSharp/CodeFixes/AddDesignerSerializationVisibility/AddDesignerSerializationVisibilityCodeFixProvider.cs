// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Immutable;
using System.Composition;
using System.Windows.Forms.Analyzers.Diagnostics;
using Microsoft.CodeAnalysis.Text;

namespace System.Windows.Forms.CSharp.CodeFixes.AddDesignerSerializationVisibility;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AddDesignerSerializationVisibilityCodeFixProvider)), Shared]
internal class AddDesignerSerializationVisibilityCodeFixProvider : CodeFixProvider
{
    private const string SystemComponentModelName = "System.ComponentModel";

    public sealed override ImmutableArray<string> FixableDiagnosticIds
        => ImmutableArray.Create(DiagnosticIDs.ControlPropertySerialization);

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

        // Generate the Attribute we need to put on the property
        AttributeSyntax designerSerializationVisibilityAttribute = SyntaxFactory.Attribute(
            SyntaxFactory.ParseName("DesignerSerializationVisibility"),
            SyntaxFactory.ParseAttributeArgumentList("(DesignerSerializationVisibility.Hidden)"));

        // Make sure, we keep the white spaces before and after the property
        SyntaxTriviaList leadingTrivia = propertyDeclarationSyntax.GetLeadingTrivia();
        SyntaxTriviaList trailingTrivia = propertyDeclarationSyntax.GetTrailingTrivia();

        // Add the attribute to the property:
        PropertyDeclarationSyntax newProperty = propertyDeclarationSyntax
            .AddAttributeLists(
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SingletonSeparatedList(designerSerializationVisibilityAttribute)));

        // Let's restore the trivia:
        newProperty = newProperty.WithLeadingTrivia(leadingTrivia);
        newProperty = newProperty.WithTrailingTrivia(trailingTrivia);

        var usingDirective = SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(SystemComponentModelName));

        // Let's check, if we already have the using directive or if we need to add it:
        // (Remember: We can't throw here, as we are in a code fixer. But this also cannot be null.)
        SyntaxNode root = (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false))!;

        // Let's check if we already have the using directive:
        if (!root.DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .Any(u => u?.Name?.ToString() == SystemComponentModelName))
        {
            // We need to add the using directive:
            SyntaxNode firstNode = root.DescendantNodes().First();
            root = root.InsertNodesBefore(firstNode, [usingDirective]);
        }

        // Produce a new root:
        SyntaxNode originalRoot = (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false))!;
        SyntaxNode newRoot = originalRoot.ReplaceNode(propertyDeclarationSyntax, newProperty);

        // Generate the new document:
        return document.WithSyntaxRoot(newRoot);
    }
}
