' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports Microsoft.VisualBasic.CompilerServices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    ''' <summary>
    ''' These are just checking the Proxy functions, the underlaying functions are tested elsewhere
    ''' </summary>
    Partial Public Class ExceptionUtilsTests

        <WinFormsFact>
        Public Sub GetArgumentNullExceptionTest_Succeed()
            Dim ex As Exception = ExceptionUtils.GetArgumentNullException("MainForm", SR.General_PropertyNothing, "MainForm")
            Assert.IsType(Of ArgumentNullException)(ex)
            Assert.Equal($"Property MainForm cannot be set to Nothing. (Parameter 'MainForm')", ex.Message)
        End Sub

        <WinFormsFact>
        Public Sub GetDirectoryNotFoundExceptionTest_Succeed()
            Dim resourceId As String = $"ID{CStr(vbErrors.FileNotFound)}"
            Dim ex As Exception = ExceptionUtils.GetDirectoryNotFoundException(resourceId)
            Assert.IsType(Of IO.DirectoryNotFoundException)(ex)
            Assert.Equal(resourceId, ex.Message)
        End Sub

        <WinFormsFact>
        Public Sub GetFileNotFoundExceptionTest_Succeed()
            Dim resourceId As String = $"ID{CStr(vbErrors.FileNotFound)}"
            Dim ex As Exception = ExceptionUtils.GetFileNotFoundException("Test", resourceId)
            Assert.IsType(Of IO.FileNotFoundException)(ex)
            Assert.Equal(resourceId, ex.Message)
        End Sub

        <WinFormsFact>
        Public Sub GetIOExceptionTest_Succeed()
            Dim ex As Exception = ExceptionUtils.GetInvalidOperationException(SR.Mouse_NoMouseIsPresent)
            Assert.IsType(Of InvalidOperationException)(ex)
        End Sub

        <WinFormsFact>
        Public Sub GetWin32ExceptionTest_Succeed()
            Dim ex As Exception = ExceptionUtils.GetWin32Exception(SR.DiagnosticInfo_Memory)
            Assert.IsType(Of ComponentModel.Win32Exception)(ex)
        End Sub

        <WinFormsTheory>
        <InlineData(-1)>
        <InlineData(0)>
        <InlineData(20)>
        Public Sub VbMakeExceptionInvalidValuesTest_Succeed(BadResourceId As Integer)
            Dim id As String = $"ID{CStr(BadResourceId)}"
            Assert.Equal($"{SR.GetResourceString(id, id)}", ExceptionUtils.VbMakeException(BadResourceId).Message)
        End Sub

        <WinFormsTheory>
        <InlineData(vbErrors.FileNotFound)>
        <InlineData(vbErrors.PermissionDenied)>
        <InlineData(vbErrors.None)>
        Public Sub VbMakeExceptionTest_Succeed(errorCode As Integer)
            Dim id As String = $"ID{CStr(errorCode)}"
            Assert.Equal($"{SR.GetResourceString(id, id)}", ExceptionUtils.VbMakeException(errorCode).Message)
        End Sub

    End Class
End Namespace
