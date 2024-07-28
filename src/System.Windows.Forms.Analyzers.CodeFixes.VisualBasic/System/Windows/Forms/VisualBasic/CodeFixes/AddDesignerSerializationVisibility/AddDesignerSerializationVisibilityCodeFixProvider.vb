' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Immutable
Imports System.Composition
Imports System.Threading
Imports System.Windows.Forms.Analyzers.CodeFixes.Resources
Imports System.Windows.Forms.Analyzers.Diagnostics
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.CodeActions
Imports Microsoft.CodeAnalysis.CodeFixes
Imports Microsoft.CodeAnalysis.Formatting
Imports Microsoft.CodeAnalysis.Simplification
Imports Microsoft.CodeAnalysis.Text
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Global.System.Windows.Forms.VisualBasic.CodeFixes.AddDesignerSerializationVisibility

    <ExportCodeFixProvider(LanguageNames.VisualBasic, Name:=NameOf(AddDesignerSerializationVisibilityCodeFixProvider)), [Shared]>
    Friend NotInheritable Class AddDesignerSerializationVisibilityCodeFixProvider
        Inherits CodeFixProvider

        Private Const SystemComponentModelName As String = "System.ComponentModel"
        Private Const DesignerSerializationVisibilityAttributeName As String = "DesignerSerializationVisibility"

        Public Overrides ReadOnly Property FixableDiagnosticIds As ImmutableArray(Of String)
            Get
                Return ImmutableArray.Create(DiagnosticIDs.MissingPropertySerializationConfiguration)
            End Get
        End Property

        Public Overrides Function GetFixAllProvider() As FixAllProvider
            Return WellKnownFixAllProviders.BatchFixer
        End Function

        Public Overrides Async Function RegisterCodeFixesAsync(context As CodeFixContext) As Task
            ' Cannot be null - otherwise we wouldn't have a diagnostic of that ID.
            Dim root As SyntaxNode = Await context.
                Document.
                GetSyntaxRootAsync(context.CancellationToken).
                ConfigureAwait(False)

            Dim diagnostic As Diagnostic = context.Diagnostics.First()
            Dim diagnosticSpan As TextSpan = diagnostic.Location.SourceSpan

            ' Find the type declaration identified by the diagnostic.
            Dim propertyDeclaration As PropertyStatementSyntax = root.
                FindToken(diagnosticSpan.Start).
                Parent.AncestorsAndSelf().
                OfType(Of PropertyStatementSyntax)().
                First()

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

        Private Shared Async Function AddDesignerSerializationAttribute(
            document As Document,
            propertyDeclarationSyntax As PropertyStatementSyntax,
            cancellationToken As CancellationToken) As Task(Of Document)

            If propertyDeclarationSyntax Is Nothing Then
                Return document
            End If

            ' Let's make sure, the attribute we want to add is not already there:
            If propertyDeclarationSyntax.AttributeLists.SelectMany(
                    Function(al) al.Attributes).Any(
                        Function(a) a.Name.ToString() = DesignerSerializationVisibilityAttributeName) Then

                Debug.Assert(False, "The attribute should not be there.")
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
            Dim newProperty As PropertyStatementSyntax = propertyDeclarationSyntax.AddAttributeLists(
                SyntaxFactory.AttributeList(
                    SyntaxFactory.SingletonSeparatedList(designerSerializationVisibilityAttribute)))

            ' Let's format the property, so the attribute is on top of it:
            newProperty = newProperty.NormalizeWhitespace()

            ' Let's restore the original trivia:
            newProperty = newProperty.
                WithLeadingTrivia(leadingTrivia).
                WithTrailingTrivia(trailingTrivia).
                WithAdditionalAnnotations(Formatter.Annotation)

            ' Let's check, if we already have the imports statement or if we need to add it:
            ' (Remember: We can't throw here, as we are in a code fixer. But this also cannot be null.)
            Dim root As SyntaxNode = Await document.
                GetSyntaxRootAsync(cancellationToken).
                ConfigureAwait(False)

            ' Produce a new root, which has the updated property with the attribute.
            root = root.ReplaceNode(propertyDeclarationSyntax, newProperty)

            ' Let's check if we already have the imports statement:
            If Not root.DescendantNodes().OfType(Of ImportsStatementSyntax)().
                    Any(Function(u) u.ImportsClauses.OfType(Of SimpleImportsClauseSyntax)().
                        Any(Function(i) i.Name.ToString() = SystemComponentModelName)) Then

                Dim importsStatement As ImportsStatementSyntax = SyntaxFactory.
                    ImportsStatement(SyntaxFactory.SingletonSeparatedList(Of ImportsClauseSyntax)(
                        SyntaxFactory.SimpleImportsClause(
                            SyntaxFactory.ParseName(SystemComponentModelName)))).
                    WithAdditionalAnnotations(Simplifier.Annotation, Formatter.Annotation)

                ' We need to add the imports statement:
                Dim firstNode As SyntaxNode = root.DescendantNodes().First()
                root = root.InsertNodesBefore(firstNode, {importsStatement})
            End If

            document = document.WithSyntaxRoot(root)

            ' Generate the new document:
            Return document
        End Function
    End Class
End Namespace
