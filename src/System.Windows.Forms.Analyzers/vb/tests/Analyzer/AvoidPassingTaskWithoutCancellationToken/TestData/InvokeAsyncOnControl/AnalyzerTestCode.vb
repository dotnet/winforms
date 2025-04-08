Imports System
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Windows.Forms

Option Strict On
Option Explicit On

Namespace VisualBasicControls

    Public Module Program
        Public Sub Main()
            Dim control As New Control()

            ' A sync Action delegate is always fine.
            Dim okAction As New Action(Sub() control.Text = "Hello, World!")

            ' A sync Func delegate is also fine.
            Dim okFunc As New Func(Of Integer)(Function() 42)

            ' Just a Task - we will get in trouble since it's handled as a fire and forget.
            Dim notOkAsyncFunc As New Func(Of Task)(
                Function()
                    control.Text = "Hello, World!"
                    Return Task.CompletedTask
                End Function)

            ' A Task returning a value will also get us in trouble since it's handled as a fire and forget.
            Dim notOkAsyncFunc2 As New Func(Of Task(Of Integer))(
                Function()
                    control.Text = "Hello, World!"
                    Return Task.FromResult(42)
                End Function)

            ' OK.
            Dim task1 = control.InvokeAsync(okAction)

            ' Also OK.
            Dim task2 = control.InvokeAsync(okFunc)

            ' Concerning. - Most likely fire and forget by accident. We should warn about this.
            Dim task3 = control.InvokeAsync(notOkAsyncFunc, CancellationToken.None)

            ' Again: Concerning. - Most likely fire and forget by accident. We should warn about this.
            Dim task4 = control.InvokeAsync(notOkAsyncFunc, CancellationToken.None)

            ' And again concerning. - We should warn about this, too.
            Dim task5 = control.InvokeAsync(notOkAsyncFunc2, CancellationToken.None)

            ' This is OK, since we're passing a cancellation token.
            Dim okAsyncFunc As New Func(Of CancellationToken, ValueTask)(
                Function(cancellation)
                    control.Text = "Hello, World!"
                    Return ValueTask.CompletedTask
                End Function)

            ' This is also OK, again, because we're passing a cancellation token.
            Dim okAsyncFunc2 As New Func(Of CancellationToken, ValueTask(Of Integer))(
                Function(cancellation)
                    control.Text = "Hello, World!"
                    Return ValueTask.FromResult(42)
                End Function)

            ' And let's test that, too:
            Dim task6 = control.InvokeAsync(okAsyncFunc, CancellationToken.None)

            ' And that, too:
            Dim task7 = control.InvokeAsync(okAsyncFunc2, CancellationToken.None)
        End Sub
    End Module

End Namespace
