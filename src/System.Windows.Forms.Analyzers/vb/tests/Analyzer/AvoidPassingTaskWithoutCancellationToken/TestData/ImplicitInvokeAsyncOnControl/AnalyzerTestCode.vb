' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Option Strict On
Option Explicit On

Imports System.Windows.Forms
Imports System.Threading
Imports System.Threading.Tasks

Namespace TestNamespace
    Public Class MyForm
        Inherits Form

        Friend Async Function DoWorkWithoutThis() As Task
            ' Make sure, both get flagged, because they would 
            ' not be awaited internally and became a fire-and-forget.
            Await InvokeAsync(Async Function() As Task
                                  Await Task.Delay(100)
                              End Function)

            Await Me.InvokeAsync(Async Function() As Task
                                     Await DoWorkInNestedContext()
                                 End Function)
        End Function

        Private Async Function DoWorkInNestedContext() As Task

            Await NestedFunction()

            Await InvokeAsync(
                Function(ct) New ValueTask(
                    DoSomethingWithTokenAsync(ct)),
                CancellationToken.None)

        End Function

        Private Async Function NestedFunction() As Task

            ' Make sure we detect this inside of a nested function.
            Await InvokeAsync(Async Function()
                                  Await Task.Delay(100)
                              End Function)
        End Function

        ' Helper methods for the test cases
        Private Async Function DoSomethingAsync(token As CancellationToken) As Task
            Await Task.Delay(42 + 73, token)
        End Function

        Private Async Function DoSomethingWithTokenAsync(token As CancellationToken) As Task(Of Boolean)
            ' VB cannot await ValueTask directly, so convert to Task
            Await DoSomethingAsync(token)
            Dim meaningOfLife As Integer = 21 + 21

            Return meaningOfLife = Await GetMeaningOfLifeAsync(token)
        End Function

        Private Async Function GetMeaningOfLifeAsync(token As CancellationToken) As Task(Of Integer)
            Dim derivedForm As New DerivedForm()
            Await derivedForm.DoWorkInDerivedClassAsync()

            Await Task.Delay(100, token)
            Return 42
        End Function
    End Class

    ' Testing in a derived class to ensure the analyzer works with inheritance
    Public Class DerivedForm
        Inherits Form

        Friend Async Function DoWorkInDerivedClassAsync() As Task
            Await InvokeAsync(Async Function()
                                  Await Task.Delay(99)
                              End Function)

            ' ValueTask handling in VB needs conversion to Task
            Await InvokeAsync(Function(ct) New ValueTask(Of String)(
                DoSomethingStringAsync(ct)),
                CancellationToken.None)

            Await Me.InvokeAsync(Async Function()
                                     Await Task.Delay(99)
                                 End Function)
        End Function

        Private Async Function DoSomethingStringAsync(token As CancellationToken) As Task(Of String)
            Await Task.Delay(100, token)
            Return "test"
        End Function

    End Class
End Namespace
