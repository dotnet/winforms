' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Net
Imports System.Net.Http
Imports System.Threading
Imports System.Windows.Forms
Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.MyServices.Internal

Namespace Microsoft.VisualBasic.Devices

    Partial Public Class Network

        ''' <summary>
        '''  Uploads a file from the local machine to the specified host.
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded.</param>
        ''' <param name="address">The full name and path of the host destination.</param>
        Public Sub UploadFile(sourceFileName As String, address As String)
            UploadFile(
                sourceFileName,
                address,
                userName:=DEFAULT_USERNAME,
                password:=DEFAULT_PASSWORD,
                showUI:=False,
                connectionTimeout:=DEFAULT_TIMEOUT)
        End Sub

        ''' <summary>
        '''  Uploads a file from the local machine to the specified host.
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded.</param>
        ''' <param name="address">Uri representing the destination.</param>
        Public Sub UploadFile(sourceFileName As String, address As Uri)
            UploadFile(
                sourceFileName,
                address,
                userName:=DEFAULT_USERNAME,
                password:=DEFAULT_PASSWORD,
                showUI:=False,
                connectionTimeout:=DEFAULT_TIMEOUT)
        End Sub

        ''' <summary>
        '''  Uploads a file from the local machine to the specified host.
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded.</param>
        ''' <param name="address">The full name and path of the host destination.</param>
        ''' <param name="userName">The name of the user performing the upload.</param>
        ''' <param name="password">The user's password.</param>
        Public Sub UploadFile(
            sourceFileName As String,
            address As String,
            userName As String,
            password As String)

            UploadFile(
                sourceFileName,
                address,
                userName,
                password,
                showUI:=False,
                connectionTimeout:=DEFAULT_TIMEOUT)
        End Sub

        ''' <summary>
        '''  Uploads a file from the local machine to the specified host.
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded.</param>
        ''' <param name="address">Uri representing the destination.</param>
        ''' <param name="userName">The name of the user performing the upload.</param>
        ''' <param name="password">The user's password.</param>
        Public Sub UploadFile(
            sourceFileName As String,
            address As Uri,
            userName As String,
            password As String)

            UploadFile(
                sourceFileName,
                address,
                userName,
                password,
                showUI:=False,
                connectionTimeout:=DEFAULT_TIMEOUT)
        End Sub

        ''' <summary>
        '''  Uploads a file from the local machine to the specified host.
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded.</param>
        ''' <param name="address">The full name and path of the host destination.</param>
        ''' <param name="userName">The name of the user performing the upload.</param>
        ''' <param name="Password">The user's password.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        Public Sub UploadFile(
            sourceFileName As String,
            address As String,
            userName As String,
            password As String,
            showUI As Boolean,
            connectionTimeout As Integer)

            UploadFile(
                sourceFileName,
                address,
                userName,
                password,
                showUI,
                connectionTimeout,
                onUserCancel:=UICancelOption.ThrowException)
        End Sub

        ''' <summary>
        '''  Uploads a file from the local machine to the specified host.
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded.</param>
        ''' <param name="address">The full name and path of the host destination.</param>
        ''' <param name="userName">The name of the user performing the upload.</param>
        ''' <param name="Password">The user's password.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="onUserCancel">
        '''  Indicates what to do if user cancels dialog (either throw or do nothing).
        ''' </param>
        Public Sub UploadFile(
            sourceFileName As String,
            address As String,
            userName As String,
            password As String,
            showUI As Boolean,
            connectionTimeout As Integer,
            onUserCancel As UICancelOption)

            ' We're safe from UploadFile(Nothing, ...) due to overload failure (UploadFile(String,...)
            ' vs. UploadFile(Uri,...)).
            ' However, it is good practice to verify address before calling address.Trim.
            If String.IsNullOrWhiteSpace(address) Then
                Throw GetArgumentNullException(NameOf(address))
            End If

            ' Getting a uri will validate the form of the host address
            Dim addressUri As Uri = GetUri(address.Trim())

            ' For uploads, we need to make sure the address includes the filename
            If String.IsNullOrEmpty(IO.Path.GetFileName(addressUri.AbsolutePath)) Then
                Throw GetInvalidOperationException(SR.Network_UploadAddressNeedsFilename)
            End If

            UploadFile(
                sourceFileName,
                address:=addressUri,
                userName,
                password,
                showUI,
                connectionTimeout,
                onUserCancel)

        End Sub

        ''' <summary>
        '''  Uploads a file from the local machine to the specified host.
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded.</param>
        ''' <param name="address">Uri representing the destination.</param>
        ''' <param name="userName">The name of the user performing the upload.</param>
        ''' <param name="password">The user's password.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        Public Sub UploadFile(
            sourceFileName As String,
            address As Uri,
            userName As String,
            password As String,
            showUI As Boolean,
            connectionTimeout As Integer)

            UploadFile(
                sourceFileName,
                address,
                userName,
                password,
                showUI,
                connectionTimeout,
                onUserCancel:=UICancelOption.ThrowException)
        End Sub

        ''' <summary>
        '''  Uploads a file from the local machine to the specified host.
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded.</param>
        ''' <param name="address">Uri representing the destination.</param>
        ''' <param name="userName">The name of the user performing the upload.</param>
        ''' <param name="password">The user's password.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="onUserCancel">
        '''  Indicates what to do if user cancels dialog (either throw or do nothing).
        ''' </param>
        Public Sub UploadFile(
            sourceFileName As String,
            address As Uri,
            userName As String,
            password As String,
            showUI As Boolean,
            connectionTimeout As Integer,
            onUserCancel As UICancelOption)

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            UploadFile(
                sourceFileName,
                address,
                networkCredentials,
                showUI,
                connectionTimeout,
                onUserCancel)

        End Sub

        ''' <summary>
        '''  Uploads a file from the local machine to the specified host.
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded.</param>
        ''' <param name="address">Uri representing the destination.</param>
        ''' <param name="networkCredentials">The credentials of the user performing the upload.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        Public Sub UploadFile(
            sourceFileName As String,
            address As Uri,
            networkCredentials As ICredentials,
            showUI As Boolean,
            connectionTimeout As Integer)

            UploadFile(
                sourceFileName,
                address,
                networkCredentials,
                showUI,
                connectionTimeout,
                onUserCancel:=UICancelOption.ThrowException)
        End Sub

        ''' <summary>
        '''  Uploads a file from the local machine to the specified host.
        ''' </summary>
        ''' <param name="sourceFileName">The file to be uploaded.</param>
        ''' <param name="address">Uri representing the destination.</param>
        ''' <param name="networkCredentials">The credentials of the user performing the upload.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="onUserCancel">
        '''  Indicates what to do if user cancels dialog (either throw or do nothing).
        ''' </param>
        Public Sub UploadFile(
            sourceFileName As String,
            address As Uri,
            networkCredentials As ICredentials,
            showUI As Boolean,
            connectionTimeout As Integer,
            onUserCancel As UICancelOption)

            ' Construct the local file. This will validate the full name and path
            sourceFileName = FileSystemUtils.NormalizeFilePath(sourceFileName, NameOf(sourceFileName))

            ' Make sure the file exists
            If Not IO.File.Exists(sourceFileName) Then
                Dim message As String = GetResourceString(SR.IO_FileNotFound_Path, sourceFileName)
                Throw New IO.FileNotFoundException(message)
            End If

            If connectionTimeout <= 0 Then
                Throw GetArgumentExceptionWithArgName(
                    argumentName:=NameOf(connectionTimeout),
                    resourceKey:=SR.Network_BadConnectionTimeout)
            End If

            If address Is Nothing Then
                Throw GetArgumentNullException(NameOf(address))
            End If

            Dim dialog As ProgressDialog = Nothing
            Try
                ' Get network credentials
                Dim clientHandler As HttpClientHandler =
                    If(networkCredentials Is Nothing,
                        New HttpClientHandler,
                        New HttpClientHandler With {.Credentials = networkCredentials})
                dialog = GetProgressDialog(
                    address:=address.AbsolutePath,
                    fileNameWithPath:=sourceFileName,
                    showUI)

                Dim t As Task = UploadFileAsync(
                    sourceFileName,
                    addressUri:=address,
                    clientHandler,
                    dialog,
                    connectionTimeout,
                    onUserCancel)

                If t.IsFaulted Then
                    ' IsFaulted will be true if any parameters are bad
                    Throw t.Exception
                Else
                    dialog?.ShowProgressDialog()
                    Do While Not (t.IsCompleted OrElse t.IsFaulted OrElse t.IsCanceled)
                        'prevent UI freeze
                        Thread.Sleep(10)
                        Application.DoEvents()
                    Loop
                    If t.IsFaulted Then
                        Throw t.Exception
                    End If
                    If t.IsCanceled Then
                        Throw New OperationCanceledException
                    End If
                End If
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            Finally
                CloseProgressDialog(dialog)
            End Try

        End Sub

    End Class
End Namespace
