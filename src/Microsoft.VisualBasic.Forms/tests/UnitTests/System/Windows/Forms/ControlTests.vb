' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests
    Partial Public Class ControlTests

        <WinFormsFact>
        Public Sub Control_Invoke_Action_calls_correct_method()
            Using _control As New Control
                _control.CreateControl()

                Dim invoker As Action = AddressOf FaultingMethod
                Dim exception = Assert.Throws(Of DivideByZeroException)(
                    Sub() _control.Invoke(invoker))

                '    Expecting something Like the following.
                '    The first frame must be the this method, followed by MarshaledInvoke at previous location.
                '
                '    at Microsoft.VisualBasic.Forms.Tests.Microsoft.VisualBasic.Forms.Tests.ControlTests.FaultingMethod() in ...\winforms\src\Microsoft.VisualBasic.Forms\tests\UnitTests\ControlTests.vb:line 28
                '       at System.Windows.Forms.Control.InvokeMarshaledCallbackDo(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6511
                '       at System.Windows.Forms.Control.InvokeMarshaledCallbackHelper(Object obj) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6487
                '       at System.Windows.Forms.Control.InvokeMarshaledCallback(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6459
                '       at System.Windows.Forms.Control.InvokeMarshaledCallbacks() in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6563
                '    --- End of stack trace from previous location ---
                '       at System.Windows.Forms.Control.MarshaledInvoke(Control caller, Delegate method, Object[] args, Boolean synchronous) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6951
                '       at System.Windows.Forms.Control.Invoke(Delegate method, Object[] args) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6413
                '       at System.Windows.Forms.Control.Invoke(Delegate method) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6393
                '       at Microsoft.VisualBasic.Forms.Tests.Microsoft.VisualBasic.Forms.Tests.ControlTests._Closure$__1-1._Lambda$__0() in ...\winforms\src\Microsoft.VisualBasic.Forms\tests\UnitTests\ControlTests.vb:line 18

                Assert.Contains(NameOf(FaultingMethod), exception.StackTrace)
                Assert.Contains(" System.Windows.Forms.Control.Invoke(Action method) ", exception.StackTrace)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub Control_Invoke_Delegate_MethodInvoker_calls_correct_method()
            Using _control As New Control
                _control.CreateControl()

                Dim invoker As New MethodInvoker(AddressOf FaultingMethod)
                Dim exception = Assert.Throws(Of DivideByZeroException)(
                    Sub() _control.Invoke(invoker))

                '    Expecting something Like the following.
                '    The first frame must be the this method, followed by MarshaledInvoke at previous location.
                '
                '    at Microsoft.VisualBasic.Forms.Tests.Microsoft.VisualBasic.Forms.Tests.ControlTests.FaultingMethod() in ...\winforms\src\Microsoft.VisualBasic.Forms\tests\UnitTests\ControlTests.vb:line 28
                '       at System.Windows.Forms.Control.InvokeMarshaledCallbackDo(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6511
                '       at System.Windows.Forms.Control.InvokeMarshaledCallbackHelper(Object obj) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6487
                '       at System.Windows.Forms.Control.InvokeMarshaledCallback(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6459
                '       at System.Windows.Forms.Control.InvokeMarshaledCallbacks() in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6563
                '    --- End of stack trace from previous location ---
                '       at System.Windows.Forms.Control.MarshaledInvoke(Control caller, Delegate method, Object[] args, Boolean synchronous) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6951
                '       at System.Windows.Forms.Control.Invoke(Delegate method, Object[] args) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6413
                '       at System.Windows.Forms.Control.Invoke(Delegate method) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6393
                '       at Microsoft.VisualBasic.Forms.Tests.Microsoft.VisualBasic.Forms.Tests.ControlTests._Closure$__1-1._Lambda$__0() in ...\winforms\src\Microsoft.VisualBasic.Forms\tests\UnitTests\ControlTests.vb:line 18

                Assert.Contains(NameOf(FaultingMethod), exception.StackTrace)
                Assert.Contains(" System.Windows.Forms.Control.Invoke(Delegate method) ", exception.StackTrace)
            End Using
        End Sub

        <WinFormsFact>
        Public Sub Control_Invoke_Func_calls_correct_method()
            Using _control As New Control
                _control.CreateControl()

                Dim invoker As Func(Of Integer, Integer) = AddressOf FaultingFunc
                Dim exception = Assert.Throws(Of DivideByZeroException)(
                    Sub()
                        Dim result = _control.Invoke(Function() invoker(19))
                    End Sub)

                '    Expecting something Like the following.
                '    The first frame must be the this method, followed by MarshaledInvoke at previous location.
                '
                '    at Microsoft.VisualBasic.Forms.Tests.Microsoft.VisualBasic.Forms.Tests.ControlTests.FaultingFunc(Int32 a) in ...\winforms\src\Microsoft.VisualBasic.Forms\tests\UnitTests\ControlTests.vb:line 144
                '       at Microsoft.VisualBasic.Forms.Tests.Microsoft.VisualBasic.Forms.Tests.ControlTests._Closure$__4-1._Lambda$__1() in ...\winforms\src\Microsoft.VisualBasic.Forms\tests\UnitTests\ControlTests.vb:line 112
                '    --- End of stack trace from previous location ---
                '       at System.Windows.Forms.Control.MarshaledInvoke(Control caller, Delegate method, Object[] args, Boolean synchronous) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6951
                '       at System.Windows.Forms.Control.Invoke(Delegate method, Object[] args) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6413
                '       at System.Windows.Forms.Control.Invoke[T](Func`1 method) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6422
                '       at Microsoft.VisualBasic.Forms.Tests.Microsoft.VisualBasic.Forms.Tests.ControlTests._Closure$__4-1._Lambda$__0() in ...\winforms\src\Microsoft.VisualBasic.Forms\tests\UnitTests\ControlTests.vb:line 111
                '       at Xunit.Assert.RecordException(Action testCode) in C:\Dev\xunit\xunit\src\xunit.assert\Asserts\Record.cs:line 27

                Assert.Contains(NameOf(FaultingFunc), exception.StackTrace)
                Assert.Contains(" System.Windows.Forms.Control.Invoke[T](Func`1 method) ", exception.StackTrace)
            End Using
        End Sub

        Shared Sub FaultingMethod()
            Throw New DivideByZeroException()
        End Sub

        Shared Function FaultingFunc(a As Integer) As Integer
            Return a \ 0
        End Function

    End Class

End Namespace

