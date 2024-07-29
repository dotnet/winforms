// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Composition;
using System.Windows.Forms.Analyzers.Diagnostics;
using System.Windows.Forms.Analyzers.CodeFixes.Resources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Formatting;

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
            Debug.Fail("The attribute should not be there.");

            return document;
        }

        // Generate the Attribute we need to put on the property
        AttributeSyntax designerSerializationVisibilityAttribute = SyntaxFactory.Attribute(
            SyntaxFactory.ParseName(DesignerSerializationVisibilityAttributeName),
            SyntaxFactory.ParseAttributeArgumentList("(DesignerSerializationVisibility.Hidden)"));

        // Make sure, we keep the white spaces before and after the property
        SyntaxTriviaList leadingTrivia = propertyDeclarationSyntax.GetLeadingTrivia();
        SyntaxTriviaList trailingTrivia = propertyDeclarationSyntax.GetTrailingTrivia();

        // Add the attribute to the property:
        PropertyDeclarationSyntax newProperty = propertyDeclarationSyntax
            .AddAttributeLists(
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SingletonSeparatedList(designerSerializationVisibilityAttribute)));

        // Let's format the property, so the attribute is on top of it:
        newProperty = newProperty.NormalizeWhitespace();

        // Let's restore the original trivia:
        newProperty = newProperty.WithLeadingTrivia(leadingTrivia);
        newProperty = newProperty.WithTrailingTrivia(trailingTrivia);
        newProperty = newProperty.WithAdditionalAnnotations(Formatter.Annotation);

        // Let's check, if we already have the using directive or if we need to add it:
        // (Remember: We can't throw here, as we are in a code fixer. But this also cannot be null.)
        SyntaxNode root = (await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false))!;

        // Produce a new root, which has the updated property with the attribute.
        root = root.ReplaceNode(propertyDeclarationSyntax, newProperty);

        // Let's check if we already have the using directive:
        if (!root.DescendantNodes()
                .OfType<UsingDirectiveSyntax>()
                .Any(u => u?.Name?.ToString() == SystemComponentModelName))
        {
            UsingDirectiveSyntax usingDirective = SyntaxFactory
                .UsingDirective(SyntaxFactory.ParseName(SystemComponentModelName));

            usingDirective = usingDirective
                .WithAdditionalAnnotations(Simplifier.Annotation)
                .WithAdditionalAnnotations(Formatter.Annotation);

            // We need to add the using directive:
            SyntaxNode firstNode = root.DescendantNodes().First();
            root = root.InsertNodesBefore(firstNode, [usingDirective]);
        }

        document = document.WithSyntaxRoot(root);

        // Generate the new document:
        return document;
    }
}
