' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms
Imports FluentAssertions
Imports Microsoft.VisualBasic.CompilerServices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class HostServicesTests
        Private Shared ReadOnly s_control As New Control()
        Private Shared ReadOnly s_title As String = GetUniqueText()

        <WinFormsFact>
        Public Sub GetUniqueIntegerTests_Success()
            Dim condition As Boolean = GetUniqueInteger(positiveOnly:=True) >= 0
            condition.Should.BeTrue()
            GetUniqueInteger(positiveOnly:=False).Should.NotBe(GetUniqueInteger(positiveOnly:=False))
        End Sub

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

        Private NotInheritable Class TestVbHost
            Implements IVbHost

            Public Function GetParentWindow() As IWin32Window Implements IVbHost.GetParentWindow
                Return s_control
            End Function

            Public Function GetWindowTitle() As String Implements IVbHost.GetWindowTitle
                Return s_title
            End Function

        End Class
    End Class
End Namespace
