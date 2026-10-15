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
    '''  Preserves the Designer's WithEvents/Handles event model without restricting local components.
    ''' </summary>
    <DiagnosticAnalyzer(LanguageNames.VisualBasic)>
    Public NotInheritable Class DesignerEventAnalyzer
        Inherits DiagnosticAnalyzer

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor)
            Get
                Return ImmutableArray.Create(
                    SharedDiagnosticDescriptors.s_designerMissingWithEvents,
                    SharedDiagnosticDescriptors.s_designerAddHandler)
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
            Dim methodBlock = DirectCast(context.Node, MethodBlockSyntax)
            If Not DesignerTypeFacts.IsDesignerFile(methodBlock.SyntaxTree) _
                OrElse Not String.Equals(methodBlock.SubOrFunctionStatement.Identifier.ValueText,
                    "InitializeComponent", StringComparison.OrdinalIgnoreCase) Then
                Return
            End If

            Dim method = TryCast(context.SemanticModel.GetDeclaredSymbol(
                methodBlock.SubOrFunctionStatement, context.CancellationToken), IMethodSymbol)
            If method Is Nothing _
                OrElse Not DesignerTypeFacts.IsInitializeComponent(method) _
                OrElse Not facts.IsDesignerDeclaration(method.ContainingType, methodBlock.SyntaxTree) Then
                Return
            End If

            Dim body As IOperation = context.SemanticModel.GetOperation(methodBlock, context.CancellationToken)
            If body Is Nothing Then
                Return
            End If

            Dim initializedMembers As New HashSet(Of ISymbol)(SymbolEqualityComparer.Default)
            For Each operation As IOperation In body.DescendantsAndSelf()
                context.CancellationToken.ThrowIfCancellationRequested()
                Dim assignment = TryCast(operation, ISimpleAssignmentOperation)
                If assignment Is Nothing Then
                    Continue For
                End If

                Dim member As ISymbol = facts.GetInitializedComponent(assignment, method.ContainingType)
                If member Is Nothing OrElse Not initializedMembers.Add(member) Then
                    Continue For
                End If

                If TypeOf member Is IFieldSymbol Then
                    Dim location As Location = member.Locations.FirstOrDefault(Function(item) item.IsInSource)
                    If location IsNot Nothing Then
                        context.ReportDiagnostic(Diagnostic.Create(
                            SharedDiagnosticDescriptors.s_designerMissingWithEvents, location, member.Name))
                    End If
                End If
            Next

            For Each operation As IOperation In body.DescendantsAndSelf()
                context.CancellationToken.ThrowIfCancellationRequested()
                Dim assignment = TryCast(operation, IEventAssignmentOperation)
                If assignment Is Nothing OrElse Not assignment.Adds _
                    OrElse DesignerTypeFacts.IsInNestedFunction(assignment) Then
                    Continue For
                End If

                Dim eventReference = TryCast(assignment.EventReference, IEventReferenceOperation)
                If eventReference?.Instance Is Nothing Then
                    Continue For
                End If

                Dim receiver As String = Nothing
                Dim instance = TryCast(eventReference.Instance, IInstanceReferenceOperation)
                If instance IsNot Nothing AndAlso instance.ReferenceKind = InstanceReferenceKind.ContainingTypeInstance Then
                    Select Case instance.Syntax.Kind()
                        Case SyntaxKind.MyBaseExpression
                            receiver = "MyBase"
                        Case SyntaxKind.MyClassExpression
                            receiver = "MyClass"
                        Case Else
                            receiver = "Me"
                    End Select
                Else
                    Dim member As ISymbol = DesignerTypeFacts.GetInstanceMember(eventReference.Instance)
                    Dim propertySymbol = TryCast(member, IPropertySymbol)
                    If member IsNot Nothing AndAlso
                        (initializedMembers.Contains(member) OrElse
                         (propertySymbol IsNot Nothing AndAlso propertySymbol.IsWithEvents _
                          AndAlso facts.IsComponent(propertySymbol.Type))) Then
                        receiver = member.Name
                    End If
                End If

                If receiver IsNot Nothing Then
                    context.ReportDiagnostic(Diagnostic.Create(
                        SharedDiagnosticDescriptors.s_designerAddHandler,
                        assignment.Syntax.GetFirstToken().GetLocation(),
                        $"{receiver}.{eventReference.Event.Name}"))
                End If
            Next
        End Sub
    End Class
End Namespace
