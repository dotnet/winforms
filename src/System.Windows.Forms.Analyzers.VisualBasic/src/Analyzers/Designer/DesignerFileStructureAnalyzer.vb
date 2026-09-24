' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Immutable
Imports System.Windows.Forms.Analyzers
Imports System.Windows.Forms.Analyzers.Diagnostics
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Diagnostics
Imports Microsoft.CodeAnalysis.Operations
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax

Namespace Global.System.Windows.Forms.VisualBasic.Analyzers.Designer

    ''' <summary>
    '''  Enforces the structural boundary between generated Designer code and user code.
    ''' </summary>
    <DiagnosticAnalyzer(LanguageNames.VisualBasic)>
    Public NotInheritable Class DesignerFileStructureAnalyzer
        Inherits DiagnosticAnalyzer

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor)
            Get
                Return ImmutableArray.Create(
                    SharedDiagnosticDescriptors.s_unexpectedDesignerMember,
                    SharedDiagnosticDescriptors.s_designerFieldPlacement,
                    SharedDiagnosticDescriptors.s_designerEventOrDelegate)
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
                        Sub(nodeContext) AnalyzeType(nodeContext, facts), SyntaxKind.ClassBlock)
                End Sub)
        End Sub

        Private Shared Sub AnalyzeType(context As SyntaxNodeAnalysisContext, facts As DesignerTypeFacts)
            Dim typeBlock = DirectCast(context.Node, ClassBlockSyntax)
            If Not DesignerTypeFacts.IsDesignerFile(typeBlock.SyntaxTree) Then
                Return
            End If

            Dim type = TryCast(
                context.SemanticModel.GetDeclaredSymbol(
                    typeBlock.ClassStatement,
                    context.CancellationToken),
                INamedTypeSymbol)

            If type Is Nothing _
                OrElse Not facts.IsDesignerDeclaration(type, typeBlock.SyntaxTree) Then
                Return
            End If

            AnalyzeFieldPlacement(context, typeBlock, facts)

            For Each member As StatementSyntax In typeBlock.Members
                If TypeOf member Is FieldDeclarationSyntax _
                    OrElse TypeOf member Is ConstructorBlockSyntax Then
                    Continue For
                ElseIf TypeOf member Is MethodBlockSyntax Then
                    Dim method = DirectCast(member, MethodBlockSyntax)
                    Dim symbol = TryCast(context.SemanticModel.GetDeclaredSymbol(
                        method.SubOrFunctionStatement, context.CancellationToken), IMethodSymbol)
                    If symbol Is Nothing OrElse Not DesignerTypeFacts.IsAllowedMethod(symbol) Then
                        ReportUnexpectedMember(context, member)
                    ElseIf DesignerTypeFacts.IsInitializeComponent(symbol) Then
                        Dim operation As IOperation = context.SemanticModel.GetOperation(method, context.CancellationToken)
                        If operation IsNot Nothing Then
                            AnalyzeInitializers(context, symbol, operation, facts)
                        End If
                    End If
                ElseIf TypeOf member Is EventStatementSyntax _
                    OrElse TypeOf member Is EventBlockSyntax Then
                    ReportEventOrDelegate(context, member, "Event")
                ElseIf TypeOf member Is DelegateStatementSyntax Then
                    ReportEventOrDelegate(context, member, "Delegate")
                Else
                    ReportUnexpectedMember(context, member)
                End If
            Next
        End Sub

        Private Shared Sub AnalyzeInitializers(
            context As SyntaxNodeAnalysisContext,
            method As IMethodSymbol,
            body As IOperation,
            facts As DesignerTypeFacts)
            Dim reportedMembers As New HashSet(Of ISymbol)(SymbolEqualityComparer.Default)
            For Each operation As IOperation In body.DescendantsAndSelf()
                context.CancellationToken.ThrowIfCancellationRequested()
                Dim assignment = TryCast(operation, ISimpleAssignmentOperation)
                If assignment Is Nothing Then
                    Continue For
                End If

                Dim member As ISymbol = facts.GetInitializedComponent(assignment, method.ContainingType)
                If member Is Nothing _
                    OrElse member.DeclaringSyntaxReferences.Any(
                        Function(declaration) DesignerTypeFacts.IsDesignerFile(declaration.SyntaxTree)) _
                    OrElse Not reportedMembers.Add(member) Then
                    Continue For
                End If

                Dim location As Location = member.Locations.FirstOrDefault(Function(item) item.IsInSource)
                If location IsNot Nothing Then
                    context.ReportDiagnostic(Diagnostic.Create(
                        SharedDiagnosticDescriptors.s_designerFieldPlacement, location, member.Name))
                End If
            Next
        End Sub

        Private Shared Sub AnalyzeFieldPlacement(
            context As SyntaxNodeAnalysisContext,
            typeBlock As ClassBlockSyntax,
            facts As DesignerTypeFacts)
            Dim lastNonFieldIndex As Integer = -1
            For i As Integer = 0 To typeBlock.Members.Count - 1
                If Not TypeOf typeBlock.Members(i) Is FieldDeclarationSyntax Then
                    lastNonFieldIndex = i
                End If
            Next

            For i As Integer = 0 To lastNonFieldIndex - 1
                Dim field = TryCast(typeBlock.Members(i), FieldDeclarationSyntax)
                If field Is Nothing Then
                    Continue For
                End If

                For Each declarator As VariableDeclaratorSyntax In field.Declarators
                    For Each name As ModifiedIdentifierSyntax In declarator.Names
                        Dim member As ISymbol = context.SemanticModel.GetDeclaredSymbol(name, context.CancellationToken)

                        If member IsNot Nothing AndAlso Not facts.IsComponentsMember(member) Then
                            context.ReportDiagnostic(
                                Diagnostic.Create(
                                    SharedDiagnosticDescriptors.s_designerFieldPlacement,
                                    name.Identifier.GetLocation(),
                                    name.Identifier.ValueText))
                        End If
                    Next
                Next
            Next
        End Sub

        Private Shared Sub ReportUnexpectedMember(
            context As SyntaxNodeAnalysisContext,
            member As StatementSyntax)
            Dim symbol = context.SemanticModel.GetDeclaredSymbol(member, context.CancellationToken)
            Dim location As Location = If(
                symbol?.Locations.FirstOrDefault(Function(item) item.IsInSource),
                member.GetLocation())
            Dim name As String = If(symbol?.Name, member.Kind().ToString())

            context.ReportDiagnostic(
                Diagnostic.Create(
                    SharedDiagnosticDescriptors.s_unexpectedDesignerMember,
                    location,
                    name))
        End Sub

        Private Shared Sub ReportEventOrDelegate(
            context As SyntaxNodeAnalysisContext,
            member As StatementSyntax,
            kind As String)
            Dim symbol = context.SemanticModel.GetDeclaredSymbol(member, context.CancellationToken)
            Dim location As Location = If(
                symbol?.Locations.FirstOrDefault(Function(item) item.IsInSource),
                member.GetLocation())
            Dim name As String = If(symbol?.Name, member.Kind().ToString())

            context.ReportDiagnostic(
                Diagnostic.Create(
                    SharedDiagnosticDescriptors.s_designerEventOrDelegate,
                    location,
                    kind,
                    name))
        End Sub
    End Class
End Namespace
