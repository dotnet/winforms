' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports Microsoft.VisualBasic.CompilerServices
Imports Xunit
Imports ExUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.Forms.Tests

    ''' <summary>
    '''  These are just checking the Proxy functions, the underlaying functions are tested elsewhere
    ''' </summary>
    Partial Public Class ExceptionUtilsTests

        <WinFormsFact>
        Public Sub GetArgumentNullExceptionTest_Succeed()
            Dim ex As Exception = ExUtils.GetArgumentNullException("MainForm", SR.General_PropertyNothing, "MainForm")
            Assert.IsType(Of ArgumentNullException)(ex)
            Assert.Equal($"Property MainForm cannot be set to Nothing. (Parameter 'MainForm')", ex.Message)
            Assert.Equal("MainForm", CType(ex, ArgumentNullException).ParamName)
        End Sub

        <WinFormsFact>
        Public Sub GetDirectoryNotFoundExceptionTest_Succeed()
            Dim resourceString As String = ExUtils.GetResourceString(vbErrors.FileNotFound)
            Dim ex As Exception = ExUtils.GetDirectoryNotFoundException(resourceString)
            Assert.IsType(Of IO.DirectoryNotFoundException)(ex)
            Assert.Equal("File not found.", ex.Message)
        End Sub

        <WinFormsFact>
        Public Sub GetFileNotFoundExceptionTest_Succeed()
            Dim resourceString As String = ExUtils.GetResourceString(vbErrors.FileNotFound)
            Dim ex As Exception = ExUtils.GetFileNotFoundException("Test", resourceString)
            Assert.IsType(Of IO.FileNotFoundException)(ex)
            Assert.Equal("File not found.", ex.Message)
            Assert.Equal("Test", CType(ex, IO.FileNotFoundException).FileName)
        End Sub

        <WinFormsFact>
        Public Sub GetInvalidOperationExceptionTest_Succeed()
            Dim ex As Exception = ExUtils.GetInvalidOperationException(SR.Mouse_NoMouseIsPresent)
            Assert.IsType(Of InvalidOperationException)(ex)
        End Sub

        <WinFormsFact>
        Public Sub GetIOExceptionTest_Succeed()
            Dim ex As Exception = ExUtils.GetIOException(SR.IO_FileExists_Path, IO.Path.GetTempPath)
            Assert.IsType(Of IO.IOException)(ex)
            Assert.Equal($"Could not complete operation since a file already exists in this path '{IO.Path.GetTempPath}'.", ex.Message)
        End Sub

        <WinFormsFact>
        Public Sub GetWin32ExceptionTest_Succeed()
            Dim ex As Exception = ExUtils.GetWin32Exception(SR.DiagnosticInfo_Memory)
            Assert.IsType(Of ComponentModel.Win32Exception)(ex)
        End Sub

        <WinFormsTheory>
        <InlineData(-1)>
        <InlineData(0)>
        Public Sub VbMakeExceptionInvalidValuesTest_Succeed(BadResourceId As Integer)
            Dim id As String = $"ID{CStr(BadResourceId)}"
            Assert.Equal($"{SR.GetResourceString(id, id)}", ExUtils.VbMakeException(BadResourceId).Message)
        End Sub

        <WinFormsTheory>
        <InlineData(vbErrors.FileNotFound, "File not found.")>
        <InlineData(vbErrors.PermissionDenied, "Permission denied.")>
        <InlineData(vbErrors.None, "")>
        Public Sub VbMakeExceptionTest_Succeed(errorCode As Integer, expected As String)
            Dim id As String = $"ID{CStr(errorCode)}"
            Dim message As String = ExUtils.VbMakeException(errorCode).Message
            Assert.Equal(expected, message)
        End Sub

    End Class
End Namespace
