' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Windows.Forms
Imports Microsoft.VisualBasic.CompilerServices
Imports Xunit

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class HostServicesTests
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
            Assert.Equal(Nothing, inputHandler.Exception)
            Assert.Equal(Nothing, inputHandler.Result)
        End Sub

    End Class
End Namespace
