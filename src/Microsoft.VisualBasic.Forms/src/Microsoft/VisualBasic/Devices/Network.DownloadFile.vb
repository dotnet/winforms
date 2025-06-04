' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Net
Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.MyServices.Internal

Imports VbUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.Devices

    ''' <summary>
    '''  An object that allows easy access to some simple network properties and functionality.
    ''' </summary>
    Partial Public Class Network

        Public Sub New()
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        Public Sub DownloadFile(address As String, destinationFileName As String)
            DownloadFile(
                address,
                destinationFileName,
                userName:=DEFAULT_USERNAME,
                password:=DEFAULT_PASSWORD,
                showUI:=False,
                connectionTimeout:=DEFAULT_TIMEOUT,
                overwrite:=False)
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address"><see cref="Uri"/> to the remote file.</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        Public Sub DownloadFile(address As Uri, destinationFileName As String)
            DownloadFile(
                address,
                destinationFileName,
                userName:=DEFAULT_USERNAME,
                password:=DEFAULT_PASSWORD,
                showUI:=False,
                connectionTimeout:=DEFAULT_TIMEOUT,
                overwrite:=False)
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="userName">The name of the user performing the download.</param>
        ''' <param name="password">The user's password</param>
        Public Sub DownloadFile(
            address As String,
            destinationFileName As String,
            userName As String,
            password As String)

            DownloadFile(
                address,
                destinationFileName,
                userName,
                password,
                showUI:=False,
                connectionTimeout:=DEFAULT_TIMEOUT,
                overwrite:=False)
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address"><see cref="Uri"/> to the remote file.</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="userName">The name of the user performing the download.</param>
        ''' <param name="password">The user's password.</param>
        Public Sub DownloadFile(
            address As Uri,
            destinationFileName As String,
            userName As String,
            password As String)

            DownloadFile(
                address,
                destinationFileName,
                userName,
                password,
                showUI:=False,
                connectionTimeout:=DEFAULT_TIMEOUT,
                overwrite:=False)
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="userName">The name of the user performing the download.</param>
        ''' <param name="password">The user's password.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">
        '''  Indicates whether or not the file should be overwritten if local file already exists.
        ''' </param>
        Public Sub DownloadFile(
            address As String,
            destinationFileName As String,
            userName As String,
            password As String,
            showUI As Boolean,
            connectionTimeout As Integer,
            overwrite As Boolean)

            DownloadFile(
                address,
                destinationFileName,
                userName,
                password,
                showUI,
                connectionTimeout,
                overwrite,
                UICancelOption.ThrowException)
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="userName">The name of the user performing the download.</param>
        ''' <param name="password">The user's password.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">
        '''  Indicates whether or not the file should be overwritten
        '''  if local file already exists.
        ''' </param>
        ''' <param name="onUserCancel">
        '''  Indicates what to do if user cancels dialog (either <see langword="Throw"/> or do nothing).
        ''' </param>
        Public Sub DownloadFile(
            address As String,
            destinationFileName As String,
            userName As String,
            password As String,
            showUI As Boolean,
            connectionTimeout As Integer,
            overwrite As Boolean,
            onUserCancel As UICancelOption)

            ' We're safe from DownloadFile(Nothing, ...) due to overload failure (DownloadFile(String,...)
            ' vs. DownloadFile(Uri,...)).
            ' However, it is good practice to verify address before calling Trim.
            If String.IsNullOrWhiteSpace(address) Then
                Throw VbUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim addressUri As Uri = GetUri(address.Trim())

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            DownloadFile(
                address:=addressUri,
                destinationFileName,
                networkCredentials,
                showUI,
                connectionTimeout,
                overwrite,
                onUserCancel)
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address"><see cref="Uri"/> to the remote file,</param>
        ''' <param name="destinationFileName">
        '''  Name and path of file where download is saved.
        ''' </param>
        ''' <param name="userName">The name of the user performing the download.</param>
        ''' <param name="password">The user's password.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">
        '''  Indicates whether or not the file should be overwritten if local file already exists.
        ''' </param>
        Public Sub DownloadFile(
            address As Uri,
            destinationFileName As String,
            userName As String,
            password As String,
            showUI As Boolean,
            connectionTimeout As Integer,
            overwrite As Boolean)

            DownloadFile(
                address,
                destinationFileName,
                userName,
                password,
                showUI,
                connectionTimeout,
                overwrite,
                UICancelOption.ThrowException)
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address"><see cref="Uri"/> to the remote file.</param>
        ''' <param name="destinationFileName">
        '''  Name and path of file where download is saved.
        ''' </param>
        ''' <param name="userName">The name of the user performing the download.</param>
        ''' <param name="password">The user's password.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">
        '''  Time allotted before giving up on a connection.
        ''' </param>
        ''' <param name="overwrite">
        '''  Indicates whether or not the file should be overwritten
        '''  if local file already exists.
        ''' </param>
        ''' <param name="onUserCancel">
        '''  Indicates what to do if user cancels dialog (either <see langword="Throw"/> or do nothing).
        ''' </param>
        Public Sub DownloadFile(
            address As Uri,
            destinationFileName As String,
            userName As String,
            password As String,
            showUI As Boolean,
            connectionTimeout As Integer,
            overwrite As Boolean,
            onUserCancel As UICancelOption)

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            DownloadFile(
                address,
                destinationFileName,
                networkCredentials,
                showUI,
                connectionTimeout,
                overwrite,
                onUserCancel)
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address"><see cref="Uri"/> to the remote file.</param>
        ''' <param name="destinationFileName">
        '''  Name and path of file where download is saved.
        ''' </param>
        ''' <param name="networkCredentials">
        '''  The credentials of the user performing the download.
        ''' </param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">
        '''  Indicates whether or not the file should be overwritten
        '''  if local file already exists.
        ''' </param>
        ''' <remarks>Calls to all the other overloads will come through here.</remarks>
        Public Sub DownloadFile(
            address As Uri,
            destinationFileName As String,
            networkCredentials As ICredentials,
            showUI As Boolean,
            connectionTimeout As Integer,
            overwrite As Boolean)

            DownloadFile(
                address,
                destinationFileName,
                networkCredentials,
                showUI,
                connectionTimeout,
                overwrite,
                UICancelOption.ThrowException)
        End Sub

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address"><see cref="Uri"/> to the remote file.</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="networkCredentials">The <see cref="ICredentials"/> of the user performing the download.</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">
        '''  Indicates whether or not the file should be overwritten
        '''  if local file already exists.
        ''' </param>
        ''' <param name="onUserCancel">
        '''  Indicates what to do if user cancels dialog (either <see langword="Throw"/> or do nothing).
        ''' </param>
        ''' <remarks>Calls to all the other overloads will come through here.</remarks>
        Public Sub DownloadFile(
            address As Uri,
            destinationFileName As String,
            networkCredentials As ICredentials,
            showUI As Boolean,
            connectionTimeout As Integer,
            overwrite As Boolean,
            onUserCancel As UICancelOption)

            If connectionTimeout <= 0 Then
                Throw VbUtils.GetArgumentExceptionWithArgName(NameOf(connectionTimeout), SR.Network_BadConnectionTimeout)
            End If

            If address Is Nothing Then
                Throw VbUtils.GetArgumentNullException(NameOf(address))
            End If

            Using client As New WebClientExtended
                client.Timeout = connectionTimeout

                ' Don't use passive mode if we're showing UI
                client.UseNonPassiveFtp = showUI

                ' Construct the local file. This will validate the full name and path
                Dim fullFilename As String = FileSystemUtils.NormalizeFilePath(destinationFileName, NameOf(destinationFileName))

                ' Sometime a path that can't be parsed is normalized to the current directory. This makes sure we really
                ' have a file and path
                If IO.Directory.Exists(fullFilename) Then
                    Throw VbUtils.GetInvalidOperationException(SR.Network_DownloadNeedsFilename)
                End If

                ' Throw if the file exists and the user doesn't want to overwrite
                If IO.File.Exists(fullFilename) And Not overwrite Then
                    Throw New IO.IOException(VbUtils.GetResourceString(SR.IO_FileExists_Path, destinationFileName))
                End If

                ' Set credentials if we have any
                If networkCredentials IsNot Nothing Then
                    client.Credentials = networkCredentials
                End If

                Dim dialog As ProgressDialog = GetProgressDialog(address.AbsolutePath, fullFilename, showUI)

                ' Check to see if the target directory exists. If it doesn't, create it
                Dim targetDirectory As String = IO.Path.GetDirectoryName(fullFilename)

                ' Make sure we have a meaningful directory. If we don't, the destinationFileName is suspect
                If String.IsNullOrEmpty(targetDirectory) Then
                    Throw VbUtils.GetInvalidOperationException(SR.Network_DownloadNeedsFilename)
                End If

                If Not IO.Directory.Exists(targetDirectory) Then
                    IO.Directory.CreateDirectory(targetDirectory)
                End If

                ' Create the copier
                Dim copier As New WebClientCopy(client, dialog)

                ' Download the file
                copier.DownloadFile(address, fullFilename)

                ' Handle a dialog cancel
                If showUI _
                   AndAlso Environment.UserInteractive _
                   AndAlso onUserCancel = UICancelOption.ThrowException _
                   AndAlso dialog.UserCanceledTheDialog Then
                    Throw New OperationCanceledException()
                End If
            End Using
        End Sub

    End Class
End Namespace
