' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Text.Json
Imports System.Text.Json.Serialization

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class ServerConfiguration
        Private ReadOnly _options As New JsonSerializerOptions() With {.WriteIndented = True}

        Public Sub New()

        End Sub

        <JsonConstructor>
        Public Sub New(
            fileDownloadUrlPrefix As String,
            fileUploadUrlPrefix As String,
            serverDownloadAllowsAnonymousUser As Boolean,
            serverDownloadIgnoresPasswordErrors As Boolean,
            serverDownloadPassword As String,
            serverDownloadUserName As String,
            serverUploadAllowsAnonymousUser As Boolean,
            serverUploadIgnoresPasswordErrors As Boolean,
            serverUploadPassword As String,
            serverUploadUserName As String)

            Me.FileDownloadUrlPrefix = fileDownloadUrlPrefix
            Me.FileUploadUrlPrefix = fileUploadUrlPrefix
            Me.ServerDownloadAllowsAnonymousUser = serverDownloadAllowsAnonymousUser
            Me.ServerDownloadIgnoresPasswordErrors = serverDownloadIgnoresPasswordErrors
            Me.ServerDownloadPassword = serverDownloadPassword
            Me.ServerDownloadUserName = serverDownloadUserName
            Me.ServerUploadAllowsAnonymousUser = serverUploadAllowsAnonymousUser
            Me.ServerUploadIgnoresPasswordErrors = serverUploadIgnoresPasswordErrors
            Me.ServerUploadPassword = serverUploadPassword
            Me.ServerUploadUserName = serverUploadUserName
        End Sub

        Public Property FileDownloadUrlPrefix As String = "http://127.0.0.1:8080/"
        Public Property FileUploadUrlPrefix As String = "http://127.0.0.1:8080/"
        Public Property ServerDownloadAllowsAnonymousUser As Boolean = True
        Public Property ServerDownloadIgnoresPasswordErrors As Boolean
        Public Property ServerDownloadPassword As String = ""
        Public Property ServerDownloadUserName As String = ""
        Public Property ServerUploadAllowsAnonymousUser As Boolean = True
        Public Property ServerUploadIgnoresPasswordErrors As Boolean
        Public Property ServerUploadPassword As String = ""
        Public Property ServerUploadUserName As String = ""

        Friend Function GetAcceptsAnonymousLogin(uploading As Boolean) As Boolean
            If uploading Then
                Return ServerUploadAllowsAnonymousUser
            Else
                Return ServerDownloadAllowsAnonymousUser
            End If
        End Function

        Friend Function GetDefaultPassword(uploading As Boolean) As String
            If uploading Then
                Return ServerUploadPassword
            Else
                Return ServerDownloadPassword
            End If
        End Function

        Friend Function GetDefaultUserName(uploading As Boolean) As String
            If uploading Then
                Return ServerUploadUserName
            Else
                Return ServerDownloadUserName
            End If
        End Function

        Friend Function GetFileUrlPrefix(uploading As Boolean) As String
            If uploading Then
                Return FileUploadUrlPrefix
            Else
                Return FileDownloadUrlPrefix
            End If
        End Function

        Friend Function GetThrowsPasswordErrors(uploading As Boolean) As Boolean
            If uploading Then
                Return Not ServerUploadIgnoresPasswordErrors
            Else
                Return Not ServerDownloadIgnoresPasswordErrors
            End If
        End Function

        Public Shared Function ServerConfigurationLoad(jsonFilePath As String) As ServerConfiguration
            If File.Exists(jsonFilePath) Then
                Dim jsonString As String = File.ReadAllText(jsonFilePath)
                Return JsonSerializer.Deserialize(Of ServerConfiguration)(jsonString)
            End If
            Return New ServerConfiguration
        End Function

        Public Sub ServerConfigurationSave(jsonFilePath As String)
            Dim jsonString As String = JsonSerializer.Serialize(Me, _options)
            File.WriteAllText(jsonFilePath, jsonString)
        End Sub

    End Class
End Namespace
