' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Text.Json

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class ServerConfiguration
        Private ReadOnly _options As New JsonSerializerOptions() With {.WriteIndented = True}

        Public Sub New()

        End Sub

        Public Sub New(
            fileDownloadUrlPrefix As String,
            fileUploadUrlPrefix As String,
            passwordErrorsIgnored As Boolean,
            serverDownloadPassword As String,
            serverDownloadUserName As String,
            serverUploadPassword As String,
            serverUploadUserName As String)

            Me.FileDownloadUrlPrefix = fileDownloadUrlPrefix
            Me.FileUploadUrlPrefix = fileUploadUrlPrefix
            Me.PasswordErrorsIgnored = passwordErrorsIgnored
            Me.ServerDownloadPassword = serverDownloadPassword
            Me.ServerDownloadUserName = serverDownloadUserName
            Me.ServerUploadPassword = serverUploadPassword
            Me.ServerUploadUserName = serverUploadUserName
        End Sub

        Public Property FileDownloadUrlPrefix As String = "http://127.0.0.1:8080/"
        Public Property FileUploadUrlPrefix As String = "http://127.0.0.1:8080/"
        Public Property PasswordErrorsIgnored As Boolean
        Public Property ServerDownloadPassword As String = "DefaultPassword"
        Public Property ServerDownloadUserName As String = "DefaultUserName"
        Public Property ServerUploadPassword As String = "DefaultPassword"
        Public Property ServerUploadUserName As String = "DefaultUserName"

        Public Shared Function ServerConfigurationLoad(jsonFilePath As String) As ServerConfiguration
            If File.Exists(jsonFilePath) Then
                Dim jsonString As String = File.ReadAllText(jsonFilePath)
                Return JsonSerializer.Deserialize(Of ServerConfiguration)(jsonString)
            End If
            Return New ServerConfiguration
        End Function

        Public Function GetDefaultPassword(uploading As Boolean) As String
            If uploading Then
                Return ServerUploadPassword
            Else
                Return ServerDownloadPassword
            End If
        End Function

        Public Function GetDefaultUserName(uploading As Boolean) As String
            If uploading Then
                Return ServerUploadUserName
            Else
                Return ServerDownloadUserName
            End If
        End Function

        Public Function GetFileUrlPrefix(uploading As Boolean) As String
            If uploading Then
                Return FileUploadUrlPrefix
            Else
                Return FileDownloadUrlPrefix
            End If
        End Function

        Public Sub ServerConfigurationSave(jsonFilePath As String)
            Dim jsonString As String = JsonSerializer.Serialize(Me, _options)
            File.WriteAllText(jsonFilePath, jsonString)
        End Sub

    End Class
End Namespace
