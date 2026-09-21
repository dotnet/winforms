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
                Return ImmutableArray.Create(
                    SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode,
                    SharedDiagnosticDescriptors.s_unsupportedNameOfExpression,
                    SharedDiagnosticDescriptors.s_unsupportedConditionalExpression,
                    SharedDiagnosticDescriptors.s_unsupportedNullCoalescingExpression,
                    SharedDiagnosticDescriptors.s_unsupportedNullConditionalExpression,
                    SharedDiagnosticDescriptors.s_unsupportedInterpolatedString,
                    SharedDiagnosticDescriptors.s_visualBasicUnsupportedAnonymousFunction)
            End Get
        End Property

        ''' <inheritdoc/>
        Public Overrides Sub Initialize(context As AnalysisContext)
            context.EnableConcurrentExecution()
            context.ConfigureGeneratedCodeAnalysis(
                GeneratedCodeAnalysisFlags.Analyze Or GeneratedCodeAnalysisFlags.ReportDiagnostics)
            context.RegisterCompilationStartAction(
                Sub(startContext)
                    Dim facts As New DesignerTypeFacts(startContext.Compilation)
                    startContext.RegisterSyntaxNodeAction(
                        Sub(nodeContext) AnalyzeInitializeComponent(nodeContext, facts),
                        SyntaxKind.SubBlock)
                End Sub)
        End Sub

        Private Shared Sub AnalyzeInitializeComponent(context As SyntaxNodeAnalysisContext, facts As DesignerTypeFacts)
            Dim method = DirectCast(context.Node, MethodBlockSyntax)
            Dim statement As MethodStatementSyntax = method.SubOrFunctionStatement

            If Not DesignerTypeFacts.IsDesignerFile(method.SyntaxTree) _
                OrElse Not String.Equals(statement.Identifier.ValueText, "InitializeComponent", StringComparison.OrdinalIgnoreCase) Then
                Return
            End If

            Dim methodSymbol = TryCast(
                context.SemanticModel.GetDeclaredSymbol(
                    statement,
                    context.CancellationToken),
                IMethodSymbol)

            If methodSymbol Is Nothing _
                OrElse Not DesignerTypeFacts.IsInitializeComponent(methodSymbol) _
                OrElse Not facts.IsDesignerDeclaration(
                    methodSymbol.ContainingType,
                    method.SyntaxTree) Then
                Return
            End If

            For Each node As SyntaxNode In method.Statements.SelectMany(
                Function(item) item.DescendantNodesAndSelf(
                    descendIntoChildren:=Function(child) Not TypeOf child Is LambdaExpressionSyntax))
                context.CancellationToken.ThrowIfCancellationRequested()
                Dim token As SyntaxToken
                Dim descriptor As DiagnosticDescriptor = Nothing
                Dim construct As String = Nothing

                If TypeOf node Is ForBlockSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode
                    construct = "For loop"
                ElseIf TypeOf node Is ForEachBlockSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode
                    construct = "For Each loop"
                ElseIf TypeOf node Is WhileBlockSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode
                    construct = "While loop"
                ElseIf TypeOf node Is DoLoopBlockSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode
                    construct = "Do loop"
                ElseIf TypeOf node Is MultiLineIfBlockSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode
                    construct = "If statement"
                ElseIf TypeOf node Is SingleLineIfStatementSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode
                    construct = "If statement"
                ElseIf TypeOf node Is SelectBlockSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode
                    construct = "Select statement"
                ElseIf TypeOf node Is GoToStatementSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode
                    construct = "GoTo statement"
                ElseIf TypeOf node Is NameOfExpressionSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_unsupportedNameOfExpression
                ElseIf TypeOf node Is TernaryConditionalExpressionSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_unsupportedConditionalExpression
                ElseIf TypeOf node Is BinaryConditionalExpressionSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_unsupportedNullCoalescingExpression
                ElseIf TypeOf node Is ConditionalAccessExpressionSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_unsupportedNullConditionalExpression
                ElseIf TypeOf node Is InterpolatedStringExpressionSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_unsupportedInterpolatedString
                ElseIf TypeOf node Is LambdaExpressionSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_visualBasicUnsupportedAnonymousFunction
                ElseIf TypeOf node Is TryBlockSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode
                    construct = "Try statement"
                ElseIf TypeOf node Is SyncLockBlockSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode
                    construct = "SyncLock statement"
                ElseIf TypeOf node Is UsingBlockSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode
                    construct = "Using statement"
                ElseIf TypeOf node Is AwaitExpressionSyntax Then
                    token = node.GetFirstToken()
                    descriptor = SharedDiagnosticDescriptors.s_unsupportedInitializeComponentCode
                    construct = "Await expression"
                End If

                If descriptor IsNot Nothing Then
                    Dim diagnostic As Diagnostic = If(
                        construct Is Nothing,
                        Diagnostic.Create(descriptor, token.GetLocation()),
                        Diagnostic.Create(descriptor, token.GetLocation(), construct))

                    context.ReportDiagnostic(diagnostic)
                End If
            Next
        End Sub
    End Class
End Namespace
