' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports FluentAssertions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Xunit

Imports VbUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class ApplicationServicesExceptionsTests
        Private Const Test1Message As String = "Test1"
        Private Const TestMessage As String = "Test"
        Private ReadOnly _innerException As New DirectoryNotFoundException

        <WinFormsFact>
        Public Sub NewCantStartSingleInstanceException()

            Dim ex As New CantStartSingleInstanceException()
            Dim expected As String =
                VbUtils.GetResourceString(SR.AppModel_SingleInstanceCantConnect)
            ex.Message.Should.Be(expected)
            ex.InnerException.Should.Be(Nothing)

            ex = New CantStartSingleInstanceException(TestMessage)
            ex.Message.Should.Be(TestMessage)
            ex.InnerException.Should.Be(Nothing)

            ex = New CantStartSingleInstanceException(Test1Message, _innerException)
            ex.Message.Should.Be(Test1Message)
            ex.InnerException.Should.BeOfType(Of DirectoryNotFoundException)()
        End Sub

        <WinFormsFact>
        Public Sub NewNoStartupFormException()
            Dim ex As New NoStartupFormException()
            Dim expected As String =
                VbUtils.GetResourceString(SR.AppModel_NoStartupForm)
            ex.Message.Should.Be(expected)
            ex.InnerException.Should.Be(Nothing)

            ex = New NoStartupFormException(TestMessage)
            ex.Message.Should.Be(TestMessage)
            ex.InnerException.Should.Be(Nothing)

            ex = New NoStartupFormException(Test1Message, _innerException)
            ex.Message.Should.Be(Test1Message)
            ex.InnerException.Should.BeOfType(Of DirectoryNotFoundException)()
        End Sub

    End Class
End Namespace
