' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Concurrent
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
                    Dim reportedFields As New ConcurrentDictionary(Of IFieldSymbol, Byte)(
                        SymbolEqualityComparer.Default)

                    startContext.RegisterSyntaxNodeAction(AddressOf AnalyzeType, SyntaxKind.ClassBlock)
                    startContext.RegisterOperationAction(
                        Sub(operationContext) AnalyzeFieldReference(operationContext, reportedFields),
                        OperationKind.FieldReference)
                End Sub)
        End Sub

        Private Shared Sub AnalyzeType(context As SyntaxNodeAnalysisContext)
            Dim typeBlock = DirectCast(context.Node, ClassBlockSyntax)
            Dim type = TryCast(
                context.SemanticModel.GetDeclaredSymbol(
                    typeBlock.ClassStatement,
                    context.CancellationToken),
                INamedTypeSymbol)

            If type Is Nothing _
                OrElse Not DesignerTypeFacts.IsDesignerDeclaration(type, typeBlock.SyntaxTree) Then
                Return
            End If

            AnalyzeFieldPlacement(context, typeBlock)

            For Each member As StatementSyntax In typeBlock.Members
                If TypeOf member Is FieldDeclarationSyntax _
                    OrElse TypeOf member Is ConstructorBlockSyntax Then
                    Continue For
                ElseIf TypeOf member Is MethodBlockSyntax Then
                    Dim method = DirectCast(member, MethodBlockSyntax)
                    If Not IsAllowedMethod(method) Then
                        ReportUnexpectedMember(context, member)
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

        Private Shared Sub AnalyzeFieldReference(
            context As OperationAnalysisContext,
            reportedFields As ConcurrentDictionary(Of IFieldSymbol, Byte))
            Dim fieldReference = DirectCast(context.Operation, IFieldReferenceOperation)
            Dim method = TryCast(context.ContainingSymbol, IMethodSymbol)

            If method Is Nothing _
                OrElse method.Name <> "InitializeComponent" _
                OrElse method.IsStatic _
                OrElse Not method.Parameters.IsEmpty _
                OrElse Not method.ReturnsVoid _
                OrElse Not DesignerTypeFacts.IsDesignerDeclaration(
                    method.ContainingType,
                    fieldReference.Syntax.SyntaxTree) _
                OrElse fieldReference.Field.DeclaringSyntaxReferences.IsEmpty _
                OrElse fieldReference.Field.DeclaringSyntaxReferences.Any(
                    Function(declaration) DesignerTypeFacts.IsDesignerFile(declaration.SyntaxTree)) _
                OrElse Not reportedFields.TryAdd(fieldReference.Field, 0) Then
                Return
            End If

            Dim declarationLocation As Location = fieldReference.Field.Locations.FirstOrDefault(
                Function(location) location.IsInSource)
            If declarationLocation IsNot Nothing Then
                context.ReportDiagnostic(
                    Diagnostic.Create(
                        SharedDiagnosticDescriptors.s_designerFieldPlacement,
                        declarationLocation,
                        fieldReference.Field.Name))
            End If
        End Sub

        Private Shared Sub AnalyzeFieldPlacement(
            context As SyntaxNodeAnalysisContext,
            typeBlock As ClassBlockSyntax)
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
                        Dim fieldSymbol = TryCast(
                            context.SemanticModel.GetDeclaredSymbol(name, context.CancellationToken),
                            IFieldSymbol)

                        If fieldSymbol IsNot Nothing AndAlso Not IsComponentsField(fieldSymbol) Then
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

        Private Shared Function IsAllowedMethod(method As MethodBlockSyntax) As Boolean
            Dim statement As MethodStatementSyntax = method.SubOrFunctionStatement
            If statement.ImplementsClause IsNot Nothing Then
                Return True
            End If

            Dim isInitializeComponent As Boolean =
                statement.Identifier.ValueText = "InitializeComponent" _
                AndAlso statement.ParameterList.Parameters.Count = 0 _
                AndAlso statement.Kind() = SyntaxKind.SubStatement
            Dim isDispose As Boolean =
                statement.Identifier.ValueText = "Dispose" _
                AndAlso statement.ParameterList.Parameters.Count = 1 _
                AndAlso IsBooleanParameter(statement.ParameterList.Parameters(0)) _
                AndAlso statement.Kind() = SyntaxKind.SubStatement

            Return isInitializeComponent OrElse isDispose
        End Function

        Private Shared Function IsBooleanParameter(parameter As ParameterSyntax) As Boolean
            Dim simpleAsClause = TryCast(parameter.AsClause, SimpleAsClauseSyntax)

            Return simpleAsClause IsNot Nothing _
                AndAlso TypeOf simpleAsClause.Type Is PredefinedTypeSyntax _
                AndAlso DirectCast(simpleAsClause.Type, PredefinedTypeSyntax).
                    Keyword.IsKind(SyntaxKind.BooleanKeyword)
        End Function

        Private Shared Function IsComponentsField(field As IFieldSymbol) As Boolean
            Return field.Name = "components" _
                AndAlso field.Type.Name = "IContainer" _
                AndAlso field.Type.ContainingNamespace?.ToDisplayString() = "System.ComponentModel"
        End Function

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
