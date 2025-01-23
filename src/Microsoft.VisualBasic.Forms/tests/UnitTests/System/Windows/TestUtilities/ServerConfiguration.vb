' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.IO
Imports System.Text.Json
Imports System.Text.Json.Serialization

Namespace Microsoft.VisualBasic.Forms.Tests

    Public Class ServerConfiguration
        Private Const JsonDefaultFileName As String = "ServerConfiguration.JSON"

        Private Shared ReadOnly s_deserializerOptions As New JsonSerializerOptions() With {
            .TypeInfoResolver = New PrivateSetterContractResolver()}

        Private Shared ReadOnly s_jsonFilePathBase As String = Path.Combine(
            My.Application.Info.DirectoryPath,
            "System\Windows\TestUtilities\TestData")

        Private ReadOnly _serializerOptions As New JsonSerializerOptions With {
            .WriteIndented = True}

        Private _fileDownloadUrlPrefix As String = "http://127.0.0.1:8080/"
        Private _fileUploadUrlPrefix As String = "http://127.0.0.1:8080/"
        Private _serverDownloadAllowsAnonymousUser As Boolean = True
        Private _serverDownloadIgnoresPasswordErrors As Boolean
        Private _serverDownloadPassword As String = ""
        Private _serverDownloadUserName As String = ""
        Private _serverUploadAllowsAnonymousUser As Boolean = True
        Private _serverUploadIgnoresPasswordErrors As Boolean
        Private _serverUploadPassword As String = ""
        Private _serverUploadUserName As String = ""

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

        <JsonInclude>
        Public Property FileDownloadUrlPrefix As String
            Private Get
                Return _fileDownloadUrlPrefix
            End Get
            Set
                _fileDownloadUrlPrefix = Value
            End Set
        End Property

        <JsonInclude>
        Public Property FileUploadUrlPrefix As String
            Private Get
                Return _fileUploadUrlPrefix
            End Get
            Set
                _fileUploadUrlPrefix = Value
            End Set
        End Property

        <JsonInclude>
        Public Property ServerDownloadAllowsAnonymousUser As Boolean
            Private Get
                Return _serverDownloadAllowsAnonymousUser
            End Get
            Set
                _serverDownloadAllowsAnonymousUser = Value
            End Set
        End Property

        <JsonInclude>
        Public Property ServerDownloadIgnoresPasswordErrors As Boolean
            Private Get
                Return _serverDownloadIgnoresPasswordErrors
            End Get
            Set
                _serverDownloadIgnoresPasswordErrors = Value
            End Set
        End Property

        <JsonInclude>
        Public Property ServerDownloadPassword As String
            Private Get
                Return _serverDownloadPassword
            End Get
            Set
                _serverDownloadPassword = Value
            End Set
        End Property

        <JsonInclude>
        Public Property ServerDownloadUserName As String
            Private Get
                Return _serverDownloadUserName
            End Get
            Set
                _serverDownloadUserName = Value
            End Set
        End Property

        <JsonInclude>
        Public Property ServerUploadAllowsAnonymousUser As Boolean
            Private Get
                Return _serverUploadAllowsAnonymousUser
            End Get
            Set
                _serverUploadAllowsAnonymousUser = Value
            End Set
        End Property

        <JsonInclude>
        Public Property ServerUploadIgnoresPasswordErrors As Boolean
            Private Get
                Return _serverUploadIgnoresPasswordErrors
            End Get
            Set
                _serverUploadIgnoresPasswordErrors = Value
            End Set
        End Property

        <JsonInclude>
        Public Property ServerUploadPassword As String
            Private Get
                Return _serverUploadPassword
            End Get
            Set
                _serverUploadPassword = Value
            End Set
        End Property

        <JsonInclude>
        Public Property ServerUploadUserName As String
            Private Get
                Return _serverUploadUserName
            End Get
            Set
                _serverUploadUserName = Value
            End Set
        End Property

        Private Shared Function GetJsonFilePath(jsonFilePathBase As String, jsonFileName As String) As String
            If String.IsNullOrWhiteSpace(jsonFilePathBase) Then
                jsonFilePathBase = s_jsonFilePathBase
            End If
            Return Path.Combine(jsonFilePathBase, jsonFileName)
        End Function

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

        Public Shared Function ServerConfigurationLoad(
            Optional jsonFilePathBase As String = "",
            Optional jsonFileName As String = JsonDefaultFileName) As ServerConfiguration

            Dim jsonFileNameWithPath As String = GetJsonFilePath(jsonFilePathBase, jsonFileName)
            If File.Exists(jsonFileNameWithPath) Then
                Dim jsonString As String = File.ReadAllText(jsonFileNameWithPath)
                Return JsonSerializer.Deserialize(Of ServerConfiguration)(jsonString, s_deserializerOptions)
            End If
            Return New ServerConfiguration
        End Function

        Public Function ServerConfigurationSave(
            Optional jsonFilePathBase As String = "",
            Optional jsonFileName As String = JsonDefaultFileName) As String

            Dim jsonFileNameWithPath As String = GetJsonFilePath(jsonFilePathBase, jsonFileName)
            Dim jsonString As String = JsonSerializer.Serialize(Me, _serializerOptions)
            File.WriteAllText(jsonFileNameWithPath, jsonString)
            Return jsonFileNameWithPath
        End Function

    End Class
End Namespace
