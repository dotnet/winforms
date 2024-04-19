' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports Microsoft.VisualBasic.ApplicationServices
Imports Microsoft.VisualBasic.CompilerServices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class ExceptionTests

        <WinFormsFact>
        Public Sub CantStartSingleInstanceExceptionTest()
            Dim ex As Exception = New CantStartSingleInstanceException()
            Assert.IsType(Of CantStartSingleInstanceException)(ex)
            Dim expected As String = ExceptionUtils.GetResourceString(SR.AppModel_SingleInstanceCantConnect)
            Assert.Equal(expected, ex.Message)
            ex = New CantStartSingleInstanceException("Test")
            Assert.IsType(Of CantStartSingleInstanceException)(ex)
            Assert.Equal("Test", ex.Message)
            ex = New CantStartSingleInstanceException("Test1", New DirectoryNotFoundException)
            Assert.IsType(Of CantStartSingleInstanceException)(ex)
            Assert.Equal("Test1", ex.Message)
            Assert.IsType(Of DirectoryNotFoundException)(ex.InnerException)
        End Sub

        <WinFormsFact>
        Public Sub NoStartupFormExceptionTest()
            Dim ex As Exception = New NoStartupFormException()
            Assert.IsType(Of NoStartupFormException)(ex)
            Dim expected As String = ExceptionUtils.GetResourceString(SR.AppModel_NoStartupForm)
            Assert.Equal(expected, ex.Message)
            ex = New NoStartupFormException("Test")
            Assert.IsType(Of NoStartupFormException)(ex)
            Assert.Equal("Test", ex.Message)
            ex = New NoStartupFormException("Test1", New DirectoryNotFoundException)
            Assert.IsType(Of NoStartupFormException)(ex)
            Assert.Equal("Test1", ex.Message)
            Assert.IsType(Of DirectoryNotFoundException)(ex.InnerException)
        End Sub

    End Class
End Namespace
