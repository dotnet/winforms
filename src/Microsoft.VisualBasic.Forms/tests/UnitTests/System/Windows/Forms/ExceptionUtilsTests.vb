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
        Public Sub ExceptionUtils_Test()
            Dim id As String = $"ID{CStr(vbErrors.FileNotFound)}"
            Assert.Equal(ExceptionUtils.VbMakeException(vbErrors.FileNotFound).Message, SR.GetResourceString(id, id))
        End Sub


    End Class
End Namespace
