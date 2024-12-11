' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Runtime.InteropServices
Imports FluentAssertions
Imports Microsoft.VisualBasic.CompilerServices.NativeTypes
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class SecurityAttributesTests

        <WinFormsFact>
        Public Sub SECURITY_ATTRIBUTES_New_Success()
            Using securityAttributes As New SECURITY_ATTRIBUTES
                securityAttributes.nLength.Should.Be(Marshal.SizeOf(Of SECURITY_ATTRIBUTES)())
            End Using
        End Sub

    End Class
End Namespace
