﻿' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms
Imports Microsoft.VisualBasic.CompilerServices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class HostServicesTests
        Private Shared ReadOnly s_title As String = GetUniqueText()
        Private Shared ReadOnly s_Control As New Control()
        Private NotInheritable Class TestVbHost
            Implements IVbHost


            Public Function GetParentWindow() As IWin32Window Implements IVbHost.GetParentWindow
                Return s_Control
            End Function

            Public Function GetWindowTitle() As String Implements IVbHost.GetWindowTitle
                Return s_title
            End Function

        End Class


        <WinFormsFact>
        Public Sub InputHandlerTests_Success()
            Dim prompt As String = GetUniqueText()
            Dim title As String = GetUniqueText()
            Dim defaultResponse As String = GetUniqueText()
            Dim xPos As Integer = GetUniqueInteger(positiveOnly:=True)
            Dim yPos As Integer = GetUniqueInteger(positiveOnly:=True)
            Dim parentWindow As IWin32Window = Nothing
            Dim vbHost As IVbHost = HostServices.VBHost
            Dim inputHandler As New InputBoxHandler(prompt, title, defaultResponse, xPos, yPos, parentWindow)
            Assert.Equal(Nothing, vbHost)
            Assert.Equal(Nothing, inputHandler.Exception)
            Assert.Equal(Nothing, inputHandler.Result)
        End Sub

        <WinFormsFact>
        Public Sub VbHostTests_Success()
            Dim vbHost As IVbHost = New TestVbHost
            Assert.Equal(s_Control, vbHost.GetParentWindow)
            Assert.Equal(s_title, vbHost.GetWindowTitle)
        End Sub

    End Class
End Namespace
