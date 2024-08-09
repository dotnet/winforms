' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports FluentAssertions
Imports Microsoft.VisualBasic.ApplicationServices
Imports Xunit

Imports ExUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class ExceptionTests

        <WinFormsFact>
        Public Sub CantStartSingleInstanceExceptionTest()
            Dim ex As Exception = New CantStartSingleInstanceException()
            ex.Should.BeOfType(Of CantStartSingleInstanceException)()
            ex.Message.Should.Be(ExUtils.GetResourceString(SR.AppModel_SingleInstanceCantConnect))

            ex = New CantStartSingleInstanceException("Test")
            ex.Should.BeOfType(Of CantStartSingleInstanceException)()
            ex.Message.Should.Be("Test")

            ex = New CantStartSingleInstanceException("Test1", New DirectoryNotFoundException)
            ex.Should.BeOfType(Of CantStartSingleInstanceException)()
            ex.Message.Should.Be("Test1")
            ex.InnerException.Should.BeOfType(Of DirectoryNotFoundException)()
        End Sub

        <WinFormsFact>
        Public Sub NoStartupFormExceptionTest()
            Dim ex As Exception = New NoStartupFormException()
            ex.Should.BeOfType(Of NoStartupFormException)()
            ex.Message.Should.Be(ExUtils.GetResourceString(SR.AppModel_NoStartupForm))

            ex = New NoStartupFormException("Test")
            ex.Should.BeOfType(Of NoStartupFormException)()
            ex.Message.Should.Be("Test")

            ex = New NoStartupFormException("Test1", New DirectoryNotFoundException)
            ex.Should.BeOfType(Of NoStartupFormException)()
            ex.Message.Should.Be("Test1")
            ex.InnerException.Should.BeOfType(Of DirectoryNotFoundException)()
        End Sub

    End Class
End Namespace
