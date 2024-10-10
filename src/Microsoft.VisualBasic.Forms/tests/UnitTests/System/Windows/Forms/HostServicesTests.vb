' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms
Imports FluentAssertions
Imports Microsoft.VisualBasic.CompilerServices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests
    Partial Public Class HostServicesTests
        Private Shared ReadOnly s_control As New Control()
        Private Shared ReadOnly s_title As String = GetUniqueText()

        <WinFormsFact>
        Public Sub InputHandlerTests_Success()
            Dim prompt As String = GetUniqueText()
            Dim title As String = GetUniqueText()
            Dim defaultResponse As String = GetUniqueText()
            Dim xPos As Integer = -1
            Dim yPos As Integer = -1
            Dim parentWindow As IWin32Window = Nothing
            Dim vbHost As IVbHost = HostServices.VBHost
            Dim inputHandler As New InputBoxHandler(prompt, title, defaultResponse, xPos, yPos, parentWindow)
            vbHost.Should.BeNull()
            inputHandler.Exception.Should.BeNull()
            inputHandler.Result.Should.BeNull()
        End Sub

        <WinFormsFact>
        Public Sub VbHostTests_Success()
            Dim vbHost As IVbHost = New TestVbHost
            CType(vbHost.GetParentWindow, Control).Should.Be(s_control)
            vbHost.GetWindowTitle.Should.Be(s_title)
        End Sub
    End Class
End Namespace
