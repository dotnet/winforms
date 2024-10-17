' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Option Strict Off

Imports System.Windows.Forms
Imports FluentAssertions
Imports Microsoft.VisualBasic.CompilerServices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class CompilerServicesTests

        <WinFormsFact>
        Public Sub TestVbHost_New_Success()
            Dim vbHost As IVbHost = New TestVbHost
            CType(vbHost.GetParentWindow, Control).Should.Be(s_control)
            vbHost.GetWindowTitle.Should.Be(s_title)
        End Sub

    End Class
End Namespace
