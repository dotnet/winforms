' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Collections.Immutable
Imports System.Windows.Forms.Analyzers.Diagnostics
Imports System.Windows.Forms.VisualBasic.Analyzers.AvoidPassingTaskWithoutCancellationToken
Imports Microsoft.CodeAnalysis
Imports Microsoft.CodeAnalysis.Testing
Imports Microsoft.CodeAnalysis.VisualBasic.Testing
Imports Xunit
Imports System.IO

Public Class AvoidPassingTaskWithoutCancellationTokenTests

    Private Const TestCode As String = "
Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows.Forms

Namespace VisualBasicControls

    Public Module Program

        Public Sub Main()

            Dim control As New Control()

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
        Yield {
            ReferenceAssemblies.Net.Net90.AddPackages(
                ImmutableArray.Create(New PackageIdentity("Microsoft.WindowsDesktop.App.Ref", "9.0.0"))
            ), ""
        }
        ' The latest public API surface area build from this repository.
        Yield {CurrentReferences.NetCoreAppReferences, CurrentReferences.WinFormsRefPath}
    End Function


    <Theory>
    <MemberData(NameOf(GetReferenceAssemblies))>
    Public Async Function VB_AvoidPassingFuncReturningTaskWithoutCancellationAnalyzer(referenceAssemblies As ReferenceAssemblies, pathToWinFormsAssembly As String) As Task
        Assert.NotNull(referenceAssemblies)
        Assert.NotNull(pathToWinFormsAssembly)
        Assert.True(pathToWinFormsAssembly = "" Or File.Exists(pathToWinFormsAssembly))

        Dim diagnosticId As String = DiagnosticIDs.AvoidPassingFuncReturningTaskWithoutCancellationToken

        Dim context As New VisualBasicAnalyzerTest(Of AvoidPassingTaskWithoutCancellationTokenAnalyzer, DefaultVerifier) With
            {
                .TestCode = TestCode,
                .ReferenceAssemblies = referenceAssemblies
            }

        context.TestState.OutputKind = OutputKind.WindowsApplication
        context.TestState.ExpectedDiagnostics.AddRange(
            {
                DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(40, 25, 40, 101),
                DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(43, 25, 43, 101),
                DiagnosticResult.CompilerWarning(diagnosticId).WithSpan(46, 25, 46, 102)
            })

        If pathToWinFormsAssembly <> "" Then
            context.TestState.AdditionalReferences.Add(pathToWinFormsAssembly)
        End If

        Await context.RunAsync().ConfigureAwait(continueOnCapturedContext:=True)
    End Function

End Class
