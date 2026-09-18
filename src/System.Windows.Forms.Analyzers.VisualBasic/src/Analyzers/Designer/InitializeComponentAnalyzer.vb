' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Immutable
Imports System.Windows.Forms.Analyzers
Imports System.Windows.Forms.Analyzers.Diagnostics
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Diagnostics
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Global.System.Windows.Forms.VisualBasic.Analyzers.Designer

    ''' <summary>
    '''  Reports control flow that should not be emitted in <c>InitializeComponent</c>.
    ''' </summary>
    <DiagnosticAnalyzer(LanguageNames.VisualBasic)>
    Public NotInheritable Class InitializeComponentAnalyzer
        Inherits DiagnosticAnalyzer

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor)
            Get
                Return ImmutableArray.Create(SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode)
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides Sub Initialize(context As AnalysisContext)
            context.EnableConcurrentExecution()
            context.ConfigureGeneratedCodeAnalysis(
                GeneratedCodeAnalysisFlags.Analyze Or GeneratedCodeAnalysisFlags.ReportDiagnostics)
            context.RegisterSyntaxNodeAction(AddressOf AnalyzeInitializeComponent, SyntaxKind.SubBlock)
        End Sub

        Private Shared Sub AnalyzeInitializeComponent(context As SyntaxNodeAnalysisContext)
            Dim method = DirectCast(context.Node, MethodBlockSyntax)
            Dim statement As MethodStatementSyntax = method.SubOrFunctionStatement
            Dim methodSymbol = TryCast(
                context.SemanticModel.GetDeclaredSymbol(
                    statement,
                    context.CancellationToken),
                IMethodSymbol)

            If statement.Identifier.ValueText <> "InitializeComponent" _
                OrElse statement.ParameterList.Parameters.Count <> 0 _
                OrElse statement.Modifiers.Any(SyntaxKind.SharedKeyword) _
                OrElse methodSymbol Is Nothing _
                OrElse Not methodSymbol.ReturnsVoid _
                OrElse methodSymbol.ContainingType Is Nothing _
                OrElse Not DesignerTypeFacts.IsDesignerDeclaration(
                    methodSymbol.ContainingType,
                    method.SyntaxTree) Then
                Return
            End If

            For Each node As SyntaxNode In method.Statements.SelectMany(
                Function(item) item.DescendantNodesAndSelf())
                Dim token As SyntaxToken
                Dim construct As String = Nothing

                If TypeOf node Is ForBlockSyntax Then
                    token = node.GetFirstToken()
                    construct = "For loop"
                ElseIf TypeOf node Is ForEachBlockSyntax Then
                    token = node.GetFirstToken()
                    construct = "For Each loop"
                ElseIf TypeOf node Is WhileBlockSyntax Then
                    token = node.GetFirstToken()
                    construct = "While loop"
                ElseIf TypeOf node Is DoLoopBlockSyntax Then
                    token = node.GetFirstToken()
                    construct = "Do loop"
                ElseIf TypeOf node Is MultiLineIfBlockSyntax Then
                    token = node.GetFirstToken()
                    construct = "If statement"
                ElseIf TypeOf node Is SingleLineIfStatementSyntax Then
                    token = node.GetFirstToken()
                    construct = "If statement"
                ElseIf TypeOf node Is SelectBlockSyntax Then
                    token = node.GetFirstToken()
                    construct = "Select statement"
                ElseIf TypeOf node Is GoToStatementSyntax Then
                    token = node.GetFirstToken()
                    construct = "GoTo statement"
                ElseIf TypeOf node Is TryBlockSyntax Then
                    token = node.GetFirstToken()
                    construct = "Try statement"
                ElseIf TypeOf node Is SyncLockBlockSyntax Then
                    token = node.GetFirstToken()
                    construct = "SyncLock statement"
                End If

                If construct IsNot Nothing Then
                    context.ReportDiagnostic(
                        Diagnostic.Create(
                            SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode,
                            token.GetLocation(),
                            construct))
                End If
            Next
        End Sub
    End Class
End Namespace
