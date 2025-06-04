' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Immutable
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.VisualBasic
Imports Microsoft.CodeAnalysis.VisualBasic.Syntax
Imports Microsoft.CodeAnalysis.Diagnostics

Namespace Global.System.Windows.Forms.VisualBasic.Analyzers.AvoidPassingTaskWithoutCancellationToken

    <DiagnosticAnalyzer(LanguageNames.VisualBasic)>
    Public Class AvoidPassingTaskWithoutCancellationTokenAnalyzer
        Inherits DiagnosticAnalyzer

        Private Const InvokeAsyncString As String = "InvokeAsync"
        Private Const TaskString As String = "Task"
        Private Const ValueTaskString As String = "ValueTask"

        Public Overrides ReadOnly Property SupportedDiagnostics As ImmutableArray(Of DiagnosticDescriptor)
            Get
                Return ImmutableArray.Create(s_avoidFuncReturningTaskWithoutCancellationToken)
            End Get
        End Property

        Public Overrides Sub Initialize(context As AnalysisContext)
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None)
            context.EnableConcurrentExecution()
            context.RegisterSyntaxNodeAction(AddressOf AnalyzeInvocation, SyntaxKind.InvocationExpression)
        End Sub

        Private Sub AnalyzeInvocation(context As SyntaxNodeAnalysisContext)
            Dim invocationExpr = DirectCast(context.Node, InvocationExpressionSyntax)
            Dim methodSymbol As IMethodSymbol = Nothing

            ' Handle both explicit member access (Me.InvokeAsync) and implicit method calls (InvokeAsync)
            If TypeOf invocationExpr.Expression Is MemberAccessExpressionSyntax Then
                Dim memberAccessExpr = DirectCast(invocationExpr.Expression, MemberAccessExpressionSyntax)
                methodSymbol = TryCast(context.SemanticModel.GetSymbolInfo(memberAccessExpr).Symbol, IMethodSymbol)
            ElseIf TypeOf invocationExpr.Expression Is IdentifierNameSyntax Then
                Dim identifierNameSyntax = DirectCast(invocationExpr.Expression, IdentifierNameSyntax)
                methodSymbol = TryCast(context.SemanticModel.GetSymbolInfo(identifierNameSyntax).Symbol, IMethodSymbol)
            End If

            If methodSymbol Is Nothing OrElse methodSymbol.Name <> InvokeAsyncString OrElse methodSymbol.Parameters.Length <> 2 Then
                Return
            End If

            Dim funcParameter As IParameterSymbol = methodSymbol.Parameters(0)
            Dim containingType As INamedTypeSymbol = methodSymbol.ContainingType

            ' If the function delegate has a parameter (which makes then 2 type arguments),
            ' we can safely assume it's a CancellationToken, otherwise the compiler would have
            ' complained before, because this is the only overload type we're accepting in a
            ' func as a passed parameter.
            If Not (TypeOf funcParameter.Type Is INamedTypeSymbol) Then
                Return
            End If

            Dim funcType = DirectCast(funcParameter.Type, INamedTypeSymbol)

            If funcType.TypeArguments.Length <> 1 OrElse funcType.ContainingNamespace.ToString() <> "System" Then
                Return
            End If

            ' Let's make absolute clear, we're dealing with InvokeAsync of Control.
            ' For implicit calls, we check the containing type of the method itself.
            If containingType Is Nothing OrElse Not IsAncestorOrSelfOfType(containingType, "System.Windows.Forms.Control") Then
                ' For explicit calls, we need to check the instance type (from before)
                If TypeOf invocationExpr.Expression Is MemberAccessExpressionSyntax Then
                    Dim memberAccess = DirectCast(invocationExpr.Expression, MemberAccessExpressionSyntax)
                    Dim objectTypeInfo As TypeInfo = context.SemanticModel.GetTypeInfo(memberAccess.Expression)

                    If Not (TypeOf objectTypeInfo.Type Is INamedTypeSymbol) Then
                        Return
                    End If

                    Dim objectType = DirectCast(objectTypeInfo.Type, INamedTypeSymbol)

                    If Not IsAncestorOrSelfOfType(objectType, "System.Windows.Forms.Control") Then
                        Return
                    End If
                Else
                    Return
                End If
            End If

            ' And finally, let's check if the return type is Task or ValueTask, because those
            ' can become now fire-and-forgets.
            If funcType.DelegateInvokeMethod?.ReturnType IsNot Nothing Then
                Dim returnType = TryCast(funcType.DelegateInvokeMethod.ReturnType, INamedTypeSymbol)

                If returnType IsNot Nothing AndAlso (returnType.Name = TaskString OrElse returnType.Name = ValueTaskString) Then
                    Dim diagnostic As Diagnostic = Diagnostic.Create(
                        s_avoidFuncReturningTaskWithoutCancellationToken,
                        invocationExpr.GetLocation())

                    context.ReportDiagnostic(diagnostic)
                End If
            End If
        End Sub

        ' Helper method to check if a type is of a certain type or a derived type.
        Private Shared Function IsAncestorOrSelfOfType(type As INamedTypeSymbol, typeName As String) As Boolean
            Return type IsNot Nothing AndAlso
                (type.ToString() = typeName OrElse
                IsAncestorOrSelfOfType(type.BaseType, typeName))
        End Function
    End Class
End Namespace
