' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Net
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

            Using client As New WebClientExtended()
                client.Timeout = connectionTimeout

                ' Set credentials if we have any
                If networkCredentials IsNot Nothing Then
                    client.Credentials = networkCredentials
                End If

                Dim dialog As ProgressDialog = Nothing
                If InteractiveEnvironment(showUI) Then
                    dialog = New ProgressDialog With {
                        .Text = GetResourceString(SR.ProgressDialogUploadingTitle, sourceFileName),
                        .LabelText = GetResourceString(
                            resourceKey:=SR.ProgressDialogUploadingLabel,
                            sourceFileName,
                        address.AbsolutePath)
                    }
                End If

                ' Create the copier
                Dim copier As New WebClientCopy(client, dialog)

                ' Download the file
                copier.UploadFile(sourceFileName, address)

                ' Handle a dialog cancel
                If InteractiveEnvironment(showUI) Then
                    If onUserCancel = UICancelOption.ThrowException And dialog.UserCanceledTheDialog Then
                        Throw New OperationCanceledException()
                    End If
                End If
            End Using

        End Sub

    End Class
End Namespace
