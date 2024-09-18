' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports FluentAssertions
Imports Xunit

Imports VbUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.Forms.Tests

    ''' <summary>
    '''  These are just checking the Proxy functions, the underlying functions are tested elsewhere.
    ''' </summary>
    Public Class ExceptionUtilsTests

        <WinFormsFact>
        Public Sub GetArgumentExceptionWithArgNameTest_Succeed()
            Const ArgumentName As String = "MainForm"
            Dim resourceID As String = SR.General_PropertyNothing
            Dim ex As Exception = VbUtils.GetArgumentExceptionWithArgName(
                ArgumentName,
                resourceID,
                ArgumentName
)
            ex.Should.BeOfType(Of ArgumentException)()
            CType(ex, ArgumentException).ParamName.Should.Be(ArgumentName)
            ex.Message.Should.StartWith(VbUtils.GetResourceString(resourceID, ArgumentName), ArgumentName)
        End Sub

        <WinFormsFact>
        Public Sub GetArgumentNullExceptionTest_Succeed()
            Const ArgumentName As String = "MainForm"
            Dim ex As Exception = VbUtils.GetArgumentNullException(ArgumentName)
            ex.Should.BeOfType(Of ArgumentNullException)()
            CType(ex, ArgumentNullException).ParamName.Should.Be(ArgumentName)
            ex.Message.Should.StartWith($"{VbUtils.GetResourceString(SR.General_ArgumentNullException)}")
        End Sub

        <WinFormsFact>
        Public Sub GetArgumentNullExceptionWithAllParametersTest_Succeed()
            Const ArgumentName As String = "MainForm"
            Dim resourceID As String = SR.General_PropertyNothing
            Dim ex As Exception = VbUtils.GetArgumentNullException(
                ArgumentName,
                resourceID,
                ArgumentName
                )
            ex.Should.BeOfType(Of ArgumentNullException)()
            CType(ex, ArgumentNullException).ParamName.Should.Be(ArgumentName)
            ex.Message.Should.StartWith(VbUtils.GetResourceString(resourceID, ArgumentName))
        End Sub

        <WinFormsFact>
        Public Sub GetDirectoryNotFoundExceptionTest_Succeed()
            Dim resourceString As String = VbUtils.GetResourceString(CompilerServices.VbErrors.FileNotFound)
            Dim ex As Exception = VbUtils.GetDirectoryNotFoundException(resourceString)
            ex.Should.BeOfType(Of IO.DirectoryNotFoundException)()
            ex.Message.Should.Be("File not found.")
        End Sub

        <WinFormsFact>
        Public Sub GetFileNotFoundExceptionTest_Succeed()
            Dim resourceString As String = VbUtils.GetResourceString(CompilerServices.VbErrors.FileNotFound)
            Dim ex As Exception = VbUtils.GetFileNotFoundException("Test", resourceString)
            ex.Should.BeOfType(Of IO.FileNotFoundException)()
            ex.Message.Should.Be("File not found.")
            CType(ex, IO.FileNotFoundException).FileName.Should.Be("Test")
        End Sub

        <WinFormsFact>
        Public Sub GetInvalidOperationExceptionTest_Succeed()
            VbUtils.GetInvalidOperationException(SR.Mouse_NoMouseIsPresent).
                Should.BeOfType(Of InvalidOperationException)()
        End Sub

        <WinFormsFact>
        Public Sub GetIOExceptionTest_Succeed()
            Dim ex As Exception = VbUtils.GetIOException(SR.IO_FileExists_Path, IO.Path.GetTempPath)
            ex.Should.BeOfType(Of IO.IOException)()
            ex.Message.Should.Be(
                $"Could not complete operation since a file already exists in this path '{IO.Path.GetTempPath}'.")
        End Sub

        <WinFormsFact>
        Public Sub GetWin32ExceptionTest_Succeed()
            Dim ex As Exception = VbUtils.GetWin32Exception(SR.DiagnosticInfo_Memory)
            ex.Should.BeOfType(Of ComponentModel.Win32Exception)()
        End Sub

        <WinFormsTheory>
        <InlineData(-1)>
        <InlineData(0)>
        Public Sub VbMakeExceptionInvalidValuesTest_Succeed(BadResourceId As Integer)
            Dim id As String = $"ID{BadResourceId}"
            Dim expected As String = $"{SR.GetResourceString(id, id)}"
            VbUtils.VbMakeException(BadResourceId).Message.Should.Be(expected)
        End Sub

        <WinFormsTheory>
        <InlineData(CompilerServices.VbErrors.FileNotFound, "File not found.")>
        <InlineData(CompilerServices.VbErrors.PermissionDenied, "Permission denied.")>
        <InlineData(CompilerServices.VbErrors.None, "")>
        Public Sub VbMakeExceptionTest_Succeed(errorCode As Integer, expected As String)
            VbUtils.VbMakeException(errorCode).Message.Should.Be(expected)
        End Sub

    End Class
End Namespace
