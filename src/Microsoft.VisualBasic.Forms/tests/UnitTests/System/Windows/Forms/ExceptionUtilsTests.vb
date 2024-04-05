' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports Microsoft.Diagnostics.Runtime.ClrDiagnosticsException
Imports Microsoft.VisualBasic.CompilerServices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    ''' <summary>
    ''' These are just checking the Proxy functions, the underlaying functions are tested elsewhere
    ''' </summary>
    Partial Public Class ExceptionUtilsTests

        <WinFormsFact>
        Public Sub ExceptionUtilsTest_Fail()
            Const BadResourceId As Integer = 20
            Dim id As String = $"ID{CStr(BadResourceId)}"
            Assert.Equal(ExceptionUtils.VbMakeException(BadResourceId).Message, $"{SR.GetResourceString(id, id)}")
        End Sub

        <WinFormsTheory>
        <InlineData(vbErrors.FileNotFound)>
        <InlineData(vbErrors.PermissionDenied)>
        <InlineData(vbErrors.None)>
        Public Sub ExceptionUtilsTest_Succeed(errorCode As Integer)
            Dim id As String = $"ID{CStr(errorCode)}"
            Assert.Equal(ExceptionUtils.VbMakeException(errorCode).Message, $"{SR.GetResourceString(id, id)}")
        End Sub

    End Class
End Namespace
