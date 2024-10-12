﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Runtime.InteropServices
Imports FluentAssertions
Imports Microsoft.VisualBasic.CompilerServices.NativeTypes
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class NativeTypesTests

        <WinFormsFact>
        Public Sub SECURITY_ATTRIBUTES_New_Test_Success()
            Using securityAttributes As New SECURITY_ATTRIBUTES
                securityAttributes.nLength.Should.Be(Marshal.SizeOf(GetType(SECURITY_ATTRIBUTES)))
            End Using
        End Sub

        <WinFormsFact>
        Public Sub STARTUPINFO_New_Test_Success()
            Using startupInfo As New STARTUPINFO
                With startupInfo
                    .dwY.Should.Be(0)
                    .cb.Should.Be(0)
                    .lpReserved.Should.Be(IntPtr.Zero)
                    .lpDesktop.Should.Be(IntPtr.Zero)
                    .lpTitle.Should.Be(IntPtr.Zero)
                    .dwX.Should.Be(0)
                    .dwY.Should.Be(0)
                    .dwXSize.Should.Be(0)
                    .dwYSize.Should.Be(0)
                    .dwXCountChars.Should.Be(0)
                    .dwYCountChars.Should.Be(0)
                    .dwFillAttribute.Should.Be(0)
                    .dwFlags.Should.Be(0)
                    .wShowWindow.Should.Be(0)
                    .cbReserved2.Should.Be(0)
                    .lpReserved2.Should.Be(IntPtr.Zero)
                    .hStdInput.Should.Be(IntPtr.Zero)
                    .hStdOutput.Should.Be(IntPtr.Zero)
                    .hStdError.Should.Be(IntPtr.Zero)
                End With
            End Using
        End Sub

    End Class
End Namespace
