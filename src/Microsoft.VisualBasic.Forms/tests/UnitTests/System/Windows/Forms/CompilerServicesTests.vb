' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Option Strict Off

Imports System.Windows.Forms
Imports FluentAssertions
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Partial Public Class CompilerServicesTests

        <WinFormsFact>
        Public Sub TestVbHost_New_Success()
            Using vbHost As New TestVbHost
                vbHost.GetParentWindow.Should.NotBeNull()
                vbHost.GetParentWindow.Should.BeOfType(Of Control)()
                vbHost.GetWindowTitle.Should.Be(s_title)
            End Using
        End Sub

    End Class
End Namespace
