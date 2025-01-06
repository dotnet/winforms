' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Net
Imports System.Net.Http
Imports System.Threading
Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.MyServices.Internal

Imports VbUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.Devices

    Partial Public Class Network

        ''' <summary>
        '''  Sends and receives a packet to and from the passed in Uri.
        '''  Maps older networkCredentials to HttpClientHandler.
        ''' </summary>
        ''' <param name="addressUri">Uri to the remote file.</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="networkCredentials">The credentials of the user performing the download.</param>
        ''' <param name="dialog">A ProgressDialog or Nothing.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">
        '''  Indicates whether or not the file should be overwritten if local file already exists.
        ''' </param>
        ''' <param name="onUserCancel"></param>
        ''' <param name="cancelToken"><see cref="CancellationToken"/></param>
        Friend Shared Function DownloadFileAsync(
            addressUri As Uri,
            destinationFileName As String,
            networkCredentials As ICredentials,
            dialog As ProgressDialog,
            connectionTimeout As Integer,
            overwrite As Boolean,
            onUserCancel As UICancelOption,
            Optional cancelToken As CancellationToken = Nothing) As Task

            Dim clientHandler As HttpClientHandler =
                If(networkCredentials Is Nothing,
                   New HttpClientHandler,
                   New HttpClientHandler With {.Credentials = networkCredentials})
            Return DownloadFileAsync(
                addressUri,
                destinationFileName,
                clientHandler,
                dialog,
                connectionTimeout,
                overwrite,
                onUserCancel,
                cancelToken)
        End Function

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="addressUri">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="userName">The name of the user performing the download.</param>
        ''' <param name="password">The user's password.</param>
        ''' <param name="dialog">A ProgressDialog or Nothing.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">
        '''  Indicates whether or not the file should be overwritten if local file already exists.
        ''' </param>
        ''' <param name="cancelToken"><see cref="CancellationToken"/></param>
        Friend Shared Async Function DownloadFileAsync(
            addressUri As Uri,
            destinationFileName As String,
            userName As String,
            password As String,
            dialog As ProgressDialog,
            connectionTimeout As Integer,
            overwrite As Boolean,
            Optional cancelToken As CancellationToken = Nothing) As Task

            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)
            Dim clientHandler As HttpClientHandler =
                If(networkCredentials Is Nothing,
                   New HttpClientHandler,
                   New HttpClientHandler With {.Credentials = networkCredentials})
            Await DownloadFileAsync(
                addressUri,
                destinationFileName,
                clientHandler,
                dialog,
                connectionTimeout,
                overwrite,
                onUserCancel:=UICancelOption.ThrowException,
                cancelToken).ConfigureAwait(continueOnCapturedContext:=False)
        End Function

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="addressUri">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="userName">The name of the user performing the download.</param>
        ''' <param name="password">The user's password.</param>
        ''' <param name="dialog">ProgressDialog or Nothing.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">
        '''  Indicates whether or not the file should be overwritten if local file already exists.
        ''' </param>
        ''' <param name="onUserCancel">
        '''  Indicates what to do if user cancels dialog (either throw or do nothing).
        ''' </param>
        ''' <param name="cancelToken"><see cref="CancellationToken"/></param>
        Friend Shared Async Function DownloadFileAsync(
            addressUri As Uri,
            destinationFileName As String,
            userName As String,
            password As String,
            dialog As ProgressDialog,
            connectionTimeout As Integer,
            overwrite As Boolean,
            onUserCancel As UICancelOption,
            Optional cancelToken As CancellationToken = Nothing) As Task

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            Await DownloadFileAsync(
                addressUri,
                destinationFileName,
                clientHandler:=If(
                    networkCredentials Is Nothing,
                    New HttpClientHandler,
                    New HttpClientHandler With {.Credentials = networkCredentials}),
                dialog,
                connectionTimeout,
                overwrite,
                onUserCancel,
                cancelToken).ConfigureAwait(continueOnCapturedContext:=False)
        End Function

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="userName">The name of the user performing the download.</param>
        ''' <param name="password">The user's password.</param>
        ''' <param name="dialog">A ProgressDialog or Nothing.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">
        '''  Indicates whether or not the file should be overwritten if local file already exists.
        ''' </param>
        ''' <param name="cancelToken"><see cref="CancellationToken"/></param>
        Friend Shared Async Function DownloadFileAsync(
            address As String,
            destinationFileName As String,
            userName As String,
            password As String,
            dialog As ProgressDialog,
            connectionTimeout As Integer,
            overwrite As Boolean,
            Optional cancelToken As CancellationToken = Nothing) As Task

            If String.IsNullOrWhiteSpace(address) Then
                Throw VbUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim addressUri As Uri = GetUri(address.Trim())

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            Dim clientHandler As HttpClientHandler =
                If(networkCredentials Is Nothing,
                   New HttpClientHandler,
                   New HttpClientHandler With {.Credentials = networkCredentials})
            Await DownloadFileAsync(
                addressUri,
                destinationFileName,
                clientHandler,
                dialog,
                connectionTimeout,
                overwrite,
                UICancelOption.ThrowException,
                cancelToken).ConfigureAwait(continueOnCapturedContext:=False)
        End Function

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="userName">The name of the user performing the download.</param>
        ''' <param name="password">The user's password.</param>
        ''' <param name="dialog">A ProgressDialog or Nothing.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">
        '''  Indicates whether or not the file should be overwritten if local file already exists.
        ''' </param>
        ''' <param name="onUserCancel">
        '''  Indicates what to do if user cancels dialog (either throw or do nothing).
        ''' </param>
        ''' <param name="cancelToken"><see cref="CancellationToken"/></param>
        Friend Shared Async Function DownloadFileAsync(
            address As String,
            destinationFileName As String,
            userName As String,
            password As String,
            dialog As ProgressDialog,
            connectionTimeout As Integer,
            overwrite As Boolean,
            onUserCancel As UICancelOption,
            Optional cancelToken As CancellationToken = Nothing) As Task

            If String.IsNullOrWhiteSpace(address) Then
                Throw VbUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim addressUri As Uri = GetUri(address.Trim())

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            Dim clientHandler As HttpClientHandler =
                If(networkCredentials Is Nothing,
                   New HttpClientHandler,
                   New HttpClientHandler With {.Credentials = networkCredentials})
            Await DownloadFileAsync(
                addressUri,
                destinationFileName,
                clientHandler,
                dialog,
                connectionTimeout,
                overwrite,
                onUserCancel,
                cancelToken).ConfigureAwait(continueOnCapturedContext:=False)
        End Function

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="addressUri">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="networkCredentials">The credentials of the user performing the download.</param>
        ''' <param name="dialog">A ProgressDialog or Nothing.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">
        '''  Indicates whether or not the file should be overwritten if local file already exists.
        ''' </param>
        ''' <param name="cancelToken"><see cref="CancellationToken"/></param>
        ''' <remarks>
        '''  Function will Throw on unhandled exceptions.
        ''' </remarks>
        Friend Shared Async Function DownloadFileAsync(
            addressUri As Uri,
            destinationFileName As String,
            networkCredentials As ICredentials,
            dialog As ProgressDialog,
            connectionTimeout As Integer,
            overwrite As Boolean,
            Optional cancelToken As CancellationToken = Nothing) As Task

            Dim clientHandler As HttpClientHandler =
                If(networkCredentials Is Nothing,
                   New HttpClientHandler,
                   New HttpClientHandler With {.Credentials = networkCredentials})
            Await DownloadFileAsync(
                addressUri,
                destinationFileName,
                clientHandler,
                dialog,
                connectionTimeout,
                overwrite,
                onUserCancel:=UICancelOption.ThrowException,
                cancelToken).ConfigureAwait(continueOnCapturedContext:=False)
        End Function

        ''' <summary>
        '''  Downloads a file from the network to the specified path.
        ''' </summary>
        ''' <param name="addressUri">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved.</param>
        ''' <param name="clientHandler">An HttpClientHandler of the user performing the download.</param>
        ''' <param name="dialog">Progress Dialog.</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection.</param>
        ''' <param name="overwrite">
        '''  Indicates whether or not the file should be overwritten if local file already exists.
        ''' </param>
        ''' <param name="onUserCancel">
        '''  Indicates what to do if user cancels dialog (either throw or do nothing).
        ''' </param>
        ''' <param name="cancelToken"><see cref="CancellationToken"/></param>
        ''' <remarks>Calls to all the other overloads will come through here.</remarks>
        Friend Shared Async Function DownloadFileAsync(
            addressUri As Uri,
            destinationFileName As String,
            clientHandler As HttpClientHandler,
            dialog As ProgressDialog,
            connectionTimeout As Integer,
            overwrite As Boolean,
            onUserCancel As UICancelOption,
            cancelToken As CancellationToken) As Task

            If cancelToken = Nothing Then
                cancelToken = New CancellationTokenSource().Token
            End If

            If connectionTimeout <= 0 Then
                Throw VbUtils.GetArgumentExceptionWithArgName(
                    argumentName:=NameOf(connectionTimeout),
                    resourceKey:=SR.Network_BadConnectionTimeout)
            End If

            If addressUri Is Nothing Then
                Throw VbUtils.GetArgumentNullException(NameOf(addressUri))
            End If

            ' Set credentials if we have any
            Dim client As HttpClient = If(clientHandler Is Nothing,
                                          New HttpClient(),
                                          New HttpClient(clientHandler)
                                         )

            client.Timeout = New TimeSpan(0, 0, 0, 0, connectionTimeout)

            ' Construct the local file. This will validate the full name and path
            Dim normalizedFilePath As String = FileSystemUtils.NormalizeFilePath(
                path:=destinationFileName,
                paramName:=NameOf(destinationFileName))

            ' Sometime a path that can't be parsed is normalized to the current directory.
            ' This makes sure we really have a file and path
            If IO.Directory.Exists(normalizedFilePath) Then
                Throw VbUtils.GetInvalidOperationException(SR.Network_DownloadNeedsFilename)
            End If

            ' Throw if the file exists and the user doesn't want to overwrite
            If Not overwrite AndAlso IO.File.Exists(normalizedFilePath) Then
                Throw New IO.IOException(
                    message:=VbUtils.GetResourceString(SR.IO_FileExists_Path, destinationFileName))
            End If

            ' Check to see if the target directory exists. If it doesn't, create it
            Dim targetDirectory As String = IO.Path.GetDirectoryName(normalizedFilePath)

            ' Make sure we have a meaningful directory. If we don't, the destinationFileName is suspect
            If String.IsNullOrEmpty(targetDirectory) Then
                Throw VbUtils.GetInvalidOperationException(SR.Network_DownloadNeedsFilename)
            End If

            If Not IO.Directory.Exists(targetDirectory) Then
                IO.Directory.CreateDirectory(targetDirectory)
            End If

            ' Create the copier
            Dim copier As New HttpClientCopy(client, dialog)

            ' Download the file
            Try
                Await copier.DownloadFileWorkerAsync(
                    addressUri,
                    normalizedFilePath,
                    externalToken:=cancelToken).ConfigureAwait(continueOnCapturedContext:=False)
            Catch ex As Exception
                If onUserCancel = UICancelOption.ThrowException OrElse Not dialog.UserCanceledTheDialog Then
                    CloseProgressDialog(dialog)
                    Throw
                End If
            End Try

        End Function

    End Class
End Namespace
