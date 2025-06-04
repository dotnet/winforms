' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms
Imports FluentAssertions
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class ControlInvokeTests

        Public Shared Function FaultingFunc(a As Integer) As Integer
            Return a \ 0
        End Function

        Public Shared Sub FaultingMethod()
            Throw New DivideByZeroException()
        End Sub

        <WinFormsFact>
        Public Sub Control_Invoke_Action_calls_correct_method()
            Using _control As New Control
                _control.CreateControl()

                Dim testCode As Action =
                    Sub()
                        _control.Invoke(AddressOf FaultingMethod)
                    End Sub

                Dim exception As Exception = Assert.Throws(Of DivideByZeroException)(testCode)

                '    Expecting something Like the following.
                '    The first frame must be the this method, followed by MarshaledInvoke at previous location.
                '
                '    at Microsoft.VisualBasic.Forms.Tests.ControlInvokeTests.FaultingMethod() in ...\winforms\src\Microsoft.VisualBasic.Forms\tests\UnitTests\ControlInvokeTests.vb:line 28
                '       at System.Windows.Forms.Control.InvokeMarshaledCallbackDo(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6511
                '       at System.Windows.Forms.Control.InvokeMarshaledCallbackHelper(Object obj) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6487
                '       at System.Windows.Forms.Control.InvokeMarshaledCallback(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6459
                '       at System.Windows.Forms.Control.InvokeMarshaledCallbacks() in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6563
                '    --- End of stack trace from previous location ---
                '       at System.Windows.Forms.Control.MarshaledInvoke(Control caller, Delegate method, Object[] args, Boolean synchronous) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6951
                '       at System.Windows.Forms.Control.Invoke(Delegate method, Object[] args) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6413
                '       at System.Windows.Forms.Control.Invoke(Delegate method) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6393
                '       at Microsoft.VisualBasic.Forms.Tests.ControlInvokeTests._Closure$__1-1._Lambda$__0() in ...\winforms\src\Microsoft.VisualBasic.Forms\tests\UnitTests\ControlInvokeTests.vb:line 18

                exception.StackTrace.Should.Contain(NameOf(FaultingMethod))
                exception.StackTrace.Should.Contain(" System.Windows.Forms.Control.Invoke(Action method) ")
            End Using
        End Sub

        <WinFormsFact>
        Public Sub Control_Invoke_Delegate_MethodInvoker_calls_correct_method()
            Using _control As New Control
                _control.CreateControl()

                Dim testCode As Action =
                    Sub()
                        _control.Invoke(New MethodInvoker(AddressOf FaultingMethod))
                    End Sub
                Dim exception As Exception = Assert.Throws(Of DivideByZeroException)(testCode)

                '    Expecting something Like the following.
                '    The first frame must be the this method, followed by MarshaledInvoke at previous location.
                '
                '    at Microsoft.VisualBasic.Forms.Tests.ControlInvokeTests.FaultingMethod() in ...\winforms\src\Microsoft.VisualBasic.Forms\tests\UnitTests\ControlInvokeTests.vb:line 28
                '       at System.Windows.Forms.Control.InvokeMarshaledCallbackDo(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6511
                '       at System.Windows.Forms.Control.InvokeMarshaledCallbackHelper(Object obj) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6487
                '       at System.Windows.Forms.Control.InvokeMarshaledCallback(ThreadMethodEntry tme) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6459
                '       at System.Windows.Forms.Control.InvokeMarshaledCallbacks() in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6563
                '    --- End of stack trace from previous location ---
                '       at System.Windows.Forms.Control.MarshaledInvoke(Control caller, Delegate method, Object[] args, Boolean synchronous) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6951
                '       at System.Windows.Forms.Control.Invoke(Delegate method, Object[] args) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6413
                '       at System.Windows.Forms.Control.Invoke(Delegate method) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6393
                '       at Microsoft.VisualBasic.Forms.Tests.ControlInvokeTests._Closure$__1-1._Lambda$__0() in ...\winforms\src\Microsoft.VisualBasic.Forms\tests\UnitTests\ControlInvokeTests.vb:line 18

                exception.StackTrace.Should.Contain(NameOf(FaultingMethod))
                exception.StackTrace.Should.Contain(" System.Windows.Forms.Control.Invoke(Delegate method) ")
            End Using
        End Sub

        <WinFormsFact>
        Public Sub Control_Invoke_Func_calls_correct_method()
            Using _control As New Control
                _control.CreateControl()

                Dim testCode As Action =
                    Sub()
                        Dim result As Integer =
                            _control.Invoke(Function() CType(AddressOf FaultingFunc, Func(Of Integer, Integer))(19))
                    End Sub

                Dim exception As Exception = Assert.Throws(Of DivideByZeroException)(testCode)

                '    Expecting something Like the following.
                '    The first frame must be the this method, followed by MarshaledInvoke at previous location.
                '
                '    at Microsoft.VisualBasic.Forms.Tests.ControlInvokeTests.FaultingFunc(Int32 a) in ...\winforms\src\Microsoft.VisualBasic.Forms\tests\UnitTests\ControlInvokeTests.vb:line 144
                '       at Microsoft.VisualBasic.Forms.Tests.ControlInvokeTests._Closure$__4-1._Lambda$__1() in ...\winforms\src\Microsoft.VisualBasic.Forms\tests\UnitTests\ControlInvokeTests.vb:line 112
                '    --- End of stack trace from previous location ---
                '       at System.Windows.Forms.Control.MarshaledInvoke(Control caller, Delegate method, Object[] args, Boolean synchronous) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6951
                '       at System.Windows.Forms.Control.Invoke(Delegate method, Object[] args) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6413
                '       at System.Windows.Forms.Control.Invoke[T](Func`1 method) in ...\winforms\src\System.Windows.Forms\src\System\Windows\Forms\Control.cs:line 6422
                '       at Microsoft.VisualBasic.Forms.Tests.ControlInvokeTests._Closure$__4-1._Lambda$__0() in ...\winforms\src\Microsoft.VisualBasic.Forms\tests\UnitTests\ControlInvokeTests.vb:line 111
                '       at Xunit.Assert.RecordException(Action testCode) in C:\Dev\xunit\xunit\src\xunit.assert\Asserts\Record.cs:line 27

                exception.StackTrace.Should.Contain(NameOf(FaultingFunc))
                exception.StackTrace.Should.Contain(" System.Windows.Forms.Control.Invoke[T](Func`1 method) ")
            End Using
        End Sub

    End Class
End Namespace
