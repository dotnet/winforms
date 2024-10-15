' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports FluentAssertions
Imports Microsoft.VisualBasic.CompilerServices.NativeTypes
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class ProcessInformationTests

        <WinFormsFact>
        Public Sub PROCESS_INFORMATION_New_Success()
            Dim processInformation As New PROCESS_INFORMATION
            processInformation.dwProcessId.Should.Be(0)
            processInformation.dwThreadId.Should.Be(0)
            processInformation.hProcess.Should.Be(IntPtr.Zero)
            processInformation.hThread.Should.Be(IntPtr.Zero)
        End Sub

    End Class
End Namespace
