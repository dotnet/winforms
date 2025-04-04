' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows.Forms

Namespace TestNamespace

    Public Class MyForm
        Inherits Form

        Private Async Function DoWorkWithoutMe() As Task
            ' Case 1: Using InvokeAsync without 'Me' in a synchronous context
            ' This should be detected by the analyzer
            Await InvokeAsync(Function() DoSomethingAsync())

            ' Case 2: Using a variable instead of 'Me', but not triggering the analyzer
            Dim token As CancellationToken = New CancellationToken()
            Await Me.InvokeAsync(Function(ct) DoSomethingWithToken(ct), token)

        End Function

        Private Async Function DoWorkInNestedContext() As Task
            Await FunctionAsync()
        End Function

        Private Async Function FunctionAsync() As Task
            ' Case 3: Using InvokeAsync without 'Me' in a nested function
            ' This should be detected by the analyzer
            Await InvokeAsync(
                New ValueTask(DoSomethingIntAsync),
                CancellationToken.None)
        End Function

        ' Helper methods for the test cases
        Private Async Function DoSomethingAsync() As Task(Of Boolean)
            Await Task.Delay(100)
            Return True
        End Function

        Private Function DoSomethingWithToken(token As CancellationToken) As ValueTask
            Return New ValueTask(Task.CompletedTask)
        End Function

        Private Function DoSomethingIntAsync(token As CancellationToken) As ValueTask(Of Integer)
            Dim someTask = Task.Delay(100, token)
            Return New ValueTask(someTask)
        End Function
    End Class

    ' Testing in a derived class to ensure the analyzer works with inheritance
    Public Class DerivedForm
        Inherits Form

        Private Async Function DoWorkInDerivedClass() As Task
            ' Case 4: Using InvokeAsync without 'Me' in a derived class
            ' This should be detected by the analyzer
            Await InvokeAsync(Function(ct) New ValueTask(Of String)(DoSomethingStringAsync(ct)), CancellationToken.None)
        End Function

        Private Async Function DoSomethingStringAsync(token As CancellationToken) As Task(Of String)
            Await Task.Delay(100, token)
            Return "test"
        End Function
    End Class

End Namespace
