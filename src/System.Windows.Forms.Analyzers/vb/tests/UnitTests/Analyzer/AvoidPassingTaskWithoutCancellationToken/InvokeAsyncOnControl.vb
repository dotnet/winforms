' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms.Analyzers.Diagnostics
Imports System.Windows.Forms.VisualBasic.Analyzers.AvoidPassingTaskWithoutCancellationToken
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Testing
Imports Microsoft.CodeAnalysis.VisualBasic.Testing
Imports Xunit

Public Class InvokeAsyncOnControl
    ' Currently, we do not have Control.InvokeAsync in the .NET 9.0 Windows reference assemblies.
    ' That's why we need to add this Async Control. Once it's there, this test will fail.
    ' We can then remove the AsyncControl and the test will pass, replace AsyncControl with
    ' Control, and the test will pass.
    Private Const AsyncControl As String = "
Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows.Forms

Namespace System.Windows.Forms
    Public Class AsyncControl
        Inherits Control

        ' BEGIN ASYNC API
        Public Function InvokeAsync(callback As Action, Optional cancellationToken As CancellationToken = Nothing) As Task
            Dim tcs As New TaskCompletionSource()

            ' Note: Code is INCORRECT, it's just here to satisfy the compiler!
            Using cancellationToken.Register(Sub() tcs.TrySetCanceled())
                MyBase.BeginInvoke(callback)
            End Using

            Return tcs.Task
        End Function

        Public Function InvokeAsync(Of T)(callback As Func(Of T), Optional cancellationToken As CancellationToken = Nothing) As Task(Of T)
            Dim tcs As New TaskCompletionSource(Of T)()

            ' Note: Code is INCORRECT, it's just here to satisfy the compiler!
            Using cancellationToken.Register(Sub() tcs.TrySetCanceled())
                MyBase.BeginInvoke(callback)
            End Using

            Return tcs.Task
        End Function

        Public Function InvokeAsync(callback As Func(Of CancellationToken, ValueTask), Optional cancellationToken As CancellationToken = Nothing) As Task
            Dim tcs As New TaskCompletionSource()

            ' Note: Code is INCORRECT, it's just here to satisfy the compiler!
            Using cancellationToken.Register(Sub() tcs.TrySetCanceled())
                MyBase.BeginInvoke(callback)
            End Using

            Return tcs.Task
        End Function

        Public Function InvokeAsync(Of T)(callback As Func(Of CancellationToken, ValueTask(Of T)), Optional cancellationToken As CancellationToken = Nothing) As Task(Of T)
            Dim tcs As New TaskCompletionSource(Of T)()

            ' Note: Code is INCORRECT, it's just here to satisfy the compiler!
            Using cancellationToken.Register(Sub() tcs.TrySetCanceled())
                MyBase.BeginInvoke(callback)
            End Using

            Return tcs.Task
        End Function
        ' END ASYNC API
    End Class
End Namespace
"

    Private Const TestCode As String = "
Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows.Forms

Namespace VisualBasicControls

    Public Module Program

        Public Sub Main()

            Dim control As New AsyncControl()

            ' A sync Action delegate is always fine.
            Dim okAction As New Action(Sub() control.Text = ""Hello, World!"")

            ' A sync Func delegate is also fine.
            Dim okFunc As New Func(Of Integer)(Function() 42)

            ' Just a Task we will get in trouble since it's handled as a fire and forget.
            Dim notOkAsyncFunc As New Func(Of Task)(Function() 
                control.Text = ""Hello, World!""
                Return Task.CompletedTask
            End Function)

            ' A Task returning a value will also get us in trouble since it's handled as a fire and forget.
            Dim notOkAsyncFunc2 As New Func(Of Task(Of Integer))(Function() 
                control.Text = ""Hello, World!""
                Return Task.FromResult(42)
            End Function)

            ' OK.
            Dim task1 = control.InvokeAsync(okAction)

            ' Also OK.
            Dim task2 = control.InvokeAsync(okFunc)

            ' Concerning. - Most likely fire and forget by accident. We should warn about this.
            Dim task3 = control.InvokeAsync(notOkAsyncFunc, System.Threading.CancellationToken.None)

            ' Again: Concerning. - Most likely fire and forget by accident. We should warn about this.
            Dim task4 = control.InvokeAsync(notOkAsyncFunc, System.Threading.CancellationToken.None)

            ' And again concerning. - We should warn about this, too.
            Dim task5 = control.InvokeAsync(notOkAsyncFunc2, System.Threading.CancellationToken.None)

            ' This is OK, since we're passing a cancellation token.
            Dim okAsyncFunc = New Func(Of CancellationToken, ValueTask)(Function(cancellation) 
                control.Text = ""Hello, World!""
                Return ValueTask.CompletedTask
            End Function)

            ' This is also OK, again, because we're passing a cancellation token.
            Dim okAsyncFunc2 = New Func(Of CancellationToken, ValueTask(Of Integer))(Function(cancellation) 
                control.Text = ""Hello, World!""
                Return ValueTask.FromResult(42)
            End Function)

            ' And let's test that, too:
            Dim task6 = control.InvokeAsync(okAsyncFunc, System.Threading.CancellationToken.None)

            ' And that, too:
            Dim task7 = control.InvokeAsync(okAsyncFunc2, System.Threading.CancellationToken.None)

        End Sub

    End Module

End Namespace

"

    Public Shared Iterator Function GetReferenceAssemblies() As IEnumerable(Of Object())
        Yield {ReferenceAssemblies.Net.Net90Windows}
    End Function

    <Theory>
    <MemberData(NameOf(GetReferenceAssemblies))>
    Public Async Function VB_AvoidPassingFuncReturningTaskWithoutCancellationAnalyzer(referenceAssemblies As ReferenceAssemblies) As Task
        ' If the API does not exist, we need to add it to the test.
        Dim customControlSource As String = AsyncControl
        Dim diagnosticId As String = DiagnosticIDs.AvoidPassingFuncReturningTaskWithoutCancellationToken

        Dim context As New VisualBasicAnalyzerTest(Of AvoidPassingTaskWithoutCancellationTokenAnalyzer, DefaultVerifier) With
            {
                .TestCode = TestCode,
                .ReferenceAssemblies = referenceAssemblies
            }

        context.TestState.OutputKind = OutputKind.WindowsApplication
        context.TestState.Sources.Add(customControlSource)
        context.TestState.ExpectedDiagnostics.AddRange(
                {
                    DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(40, 25, 40, 101),
                    DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(43, 25, 43, 101),
                    DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(46, 25, 46, 102)
                })

        Await context.RunAsync().ConfigureAwait(continueOnCapturedContext:=True)
    End Function

End Class
