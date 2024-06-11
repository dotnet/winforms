' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Immutable
Imports System.Composition
Imports System.Threading
Imports System.Windows.Forms.Analyzers.Diagnostics
Imports System.Windows.Forms.Analyzers.CodeFixes.Resources
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeActions
Imports Microsoft.CodeAnalysis.CodeFixes
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Global.System.Windows.Forms.VisualBasic.CodeFixes.AddDesignerSerializationVisibility

    <ExportCodeFixProvider(
        LanguageNames.VisualBasic,
        Name:=NameOf(AddDesignerSerializationVisibilityCodeFixProvider))>
    <[Shared]>
    Friend NotInheritable Class AddDesignerSerializationVisibilityCodeFixProvider
        Inherits CodeFixProvider

        Private Const SystemComponentModelName As String = "System.ComponentModel"

        Public Overrides ReadOnly Property FixableDiagnosticIds As ImmutableArray(Of String)
            Get
                Return ImmutableArray.Create(DiagnosticIDs.ControlPropertySerialization)
            End Get
        End Property

        Public Overrides Function GetFixAllProvider() As FixAllProvider
            Return WellKnownFixAllProviders.BatchFixer
        End Function

        Public Overrides Async Function RegisterCodeFixesAsync(context As CodeFixContext) As Task

            ' Cannot be null - otherwise we wouldn't have a diagnostic of that ID.
            Dim root As SyntaxNode = Await context _
                .Document _
                .GetSyntaxRootAsync(context.CancellationToken) _
                .ConfigureAwait(False)

            Dim diagnostic As Diagnostic = context.Diagnostics.First()
            Dim diagnosticSpan As TextSpan = diagnostic.Location.SourceSpan

            ' Find the type declaration identified by the diagnostic.
            Dim propertyDeclaration As PropertyStatementSyntax = root _
                .FindToken(diagnosticSpan.Start) _
                .Parent.AncestorsAndSelf() _
                .OfType(Of PropertyStatementSyntax)() _
                .First()

            ' Register a code action that will invoke the fix.
            context.RegisterCodeFix(
                CodeAction.Create(
                    title:=SR.AddDesignerSerializationVisibilityCodeFixTitle,
                    createChangedDocument:=Function(c) AddDesignerSerializationAttribute(
                        document:=context.Document,
                        propertyDeclarationSyntax:=propertyDeclaration,
                        cancellationToken:=c),
                    equivalenceKey:=NameOf(SR.AddDesignerSerializationVisibilityCodeFixTitle)),
                diagnostic)

        End Function

        Private Shared Async Function AddDesignerSerializationAttribute(document As Document, propertyDeclarationSyntax As PropertyStatementSyntax, cancellationToken As CancellationToken) As Task(Of Document)
            If propertyDeclarationSyntax Is Nothing Then
                Return document
            End If

            ' Generate the Attribute we need to put on the property
            Dim designerSerializationVisibilityAttribute As AttributeSyntax =
                SyntaxFactory.Attribute(
                    Nothing,
                    SyntaxFactory.ParseTypeName("DesignerSerializationVisibility"),
                    SyntaxFactory.ParseArgumentList("(DesignerSerializationVisibility.Hidden)"))

            ' Make sure, we keep the white spaces before and after the property
            Dim leadingTrivia As SyntaxTriviaList = propertyDeclarationSyntax.GetLeadingTrivia()
            Dim trailingTrivia As SyntaxTriviaList = propertyDeclarationSyntax.GetTrailingTrivia()

            ' Add the attribute to the property:
            Dim newProperty As PropertyStatementSyntax = propertyDeclarationSyntax.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(designerSerializationVisibilityAttribute)))

            ' Let's restore the trivia:
            newProperty = newProperty.WithLeadingTrivia(leadingTrivia).WithTrailingTrivia(trailingTrivia)

            ' Let's check, if we already have the using directive or if we need to add it:
            ' (Remember: We can't throw here, as we are in a code fixer. But this also cannot be null.)
            Dim root As SyntaxNode = Await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(False)

            ' Let's check if we already have the using directive:
            If Not root.DescendantNodes().OfType(Of ImportsStatementSyntax)() _
                .Any(Function(u) u?.TryGetInferredMemberName() = SystemComponentModelName) Then

                ' We need to add the using directive:
                Dim firstNode As SyntaxNode = root.DescendantNodes().First()

                ' Add the imports directive:
                Dim systemComponentModelImportsClauseSyntax As ImportsClauseSyntax =
                SyntaxFactory.SimpleImportsClause(SyntaxFactory.IdentifierName(SystemComponentModelName))

                Dim importsList As SeparatedSyntaxList(Of ImportsClauseSyntax) =
                SyntaxFactory.SeparatedList(Of ImportsClauseSyntax)().Add(systemComponentModelImportsClauseSyntax)

                Dim usingDirective As ImportsStatementSyntax = SyntaxFactory.ImportsStatement(importsList)

                root = root.InsertNodesBefore(firstNode, {usingDirective})
            End If

            ' Produce a new root:
            Dim originalRoot As SyntaxNode = Await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(False)
            Dim newRoot As SyntaxNode = originalRoot.ReplaceNode(propertyDeclarationSyntax, newProperty)

            ' Generate the new document:
            Return document.WithSyntaxRoot(newRoot)

        End Function
    End Class
End Namespace
