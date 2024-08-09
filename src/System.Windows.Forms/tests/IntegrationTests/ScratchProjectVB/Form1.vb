' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Option Explicit On
Option Strict On

Imports System.IO

Public Class Form1

    Private Sub Form1_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Dim tmpFilePath = GetTempFolderGuid()
        My.Computer.Network.DownloadFile("https://raw.githubusercontent.com/dotnet/winforms/main/README.md", tmpFilePath)
    End Sub

    Private Function GetTempFolderGuid() As String
        Dim folder As String = Path.Combine(Path.GetTempPath, Guid.NewGuid.ToString)
        Do While Directory.Exists(folder) Or File.Exists(folder)
            folder = Path.Combine(Path.GetTempPath, Guid.NewGuid.ToString)
        Loop

        Return folder
    End Function

End Class
