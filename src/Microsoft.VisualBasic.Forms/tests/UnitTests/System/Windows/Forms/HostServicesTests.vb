' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Option Strict Off

Imports System.Windows.Forms
Imports FluentAssertions
Imports Microsoft.VisualBasic.CompilerServices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests
    Partial Public Class HostServicesTests

        <WinFormsFact>
        Public Sub InputHandlerTests_Success()
            Dim prompt As String = GetUniqueText()
            Dim title As String = GetUniqueText()
            Dim defaultResponse As String = GetUniqueText()
            Dim xPos As Integer = -1
            Dim yPos As Integer = -1
            Dim inputHandler As New InputBoxHandler(prompt, title, defaultResponse, xPos, yPos, ParentWindow:=Nothing)
            CType(inputHandler.TestAccessor.Dynamic()._prompt, String).Should.Be(prompt)
            CType(inputHandler.TestAccessor.Dynamic()._title, String).Should.Be(title)
            CType(inputHandler.TestAccessor.Dynamic()._defaultResponse, String).Should.Be(defaultResponse)
            CType(inputHandler.TestAccessor.Dynamic()._xPos, String).Should.Be(xPos)
            CType(inputHandler.TestAccessor.Dynamic()._yPos, String).Should.Be(yPos)
            CType(inputHandler.TestAccessor.Dynamic()._parentWindow, IWin32Window).Should.Be(Nothing)
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
