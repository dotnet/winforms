' Licensed to the .NET Foundation under one or more agreements.
' The .NET Foundation licenses this file to you under the MIT license.

Imports System.Net
Imports System.Net.Http

Imports Microsoft.VisualBasic.CompilerServices
Imports Microsoft.VisualBasic.FileIO
Imports Microsoft.VisualBasic.MyServices.Internal

Imports ExUtils = Microsoft.VisualBasic.CompilerServices.ExceptionUtils

Namespace Microsoft.VisualBasic.Devices

    ''' <summary>
    ''' An object that allows easy access to some simple network properties and functionality.
    ''' </summary>
    Partial Public Class Network

        ' Password used in overloads where there is no password parameter
        Private Const DEFAULT_PASSWORD As String = ""

        ' Default timeout value
        Private Const DEFAULT_TIMEOUT As Integer = 100000

        ' UserName used in overloads where there is no userName parameter
        Private Const DEFAULT_USERNAME As String = ""

        ''' <summary>
        ''' Creates class and hooks up events
        ''' </summary>
        Public Sub New()
        End Sub

        ''' <summary>
        ''' Posts a message to close the progress dialog
        ''' </summary>
        Private Shared Sub CloseProgressDialog(dialog As ProgressDialog)
            ' Don't invoke unless dialog is up and running
            If dialog IsNot Nothing Then
                dialog.IndicateClosing()

                If dialog.IsHandleCreated Then
                    dialog.BeginInvoke(New System.Windows.Forms.MethodInvoker(AddressOf dialog.CloseDialog))
                Else
                    ' Ensure dialog is closed. If we get here it means the file was copied before the handle for
                    ' the progress dialog was created.
                    dialog.Close()
                End If
            End If
        End Sub

        ''' <summary>
        ''' Sends and receives a packet to and from the passed in Uri.
        ''' Maps older networkCredentials to HttpClientHandler
        ''' </summary>
        ''' <param name="addressUri">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="networkCredentials">The credentials of the user performing the download</param>
        ''' <param name="dialog">A ProgressDialog or Nothing</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <param name="onUserCancel"></param>
        ''' <returns></returns>
        Private Shared Function DownloadFileAsync(addressUri As Uri,
                                    destinationFileName As String,
                                    networkCredentials As ICredentials,
                                    dialog As ProgressDialog,
                                    connectionTimeout As Integer,
                                    overwrite As Boolean,
                                    onUserCancel As UICancelOption) As Task

            Dim clientHandler = If(networkCredentials Is Nothing,
                                   New HttpClientHandler,
                                   New HttpClientHandler With {.Credentials = networkCredentials}
                                  )
            Return DownloadFileAsync(addressUri,
                    destinationFileName,
                    clientHandler,
                    dialog,
                    connectionTimeout,
                    overwrite,
                    onUserCancel)
        End Function

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="dialog">A ProgressDialog or Nothing</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        Private Shared Async Function DownloadFileAsync(address As String,
                                        destinationFileName As String,
                                        userName As String,
                                        password As String,
                                        dialog As ProgressDialog,
                                        connectionTimeout As Integer,
                                        overwrite As Boolean) As Task

            Await DownloadFileAsync(address,
                    destinationFileName,
                    userName,
                    password,
                    dialog,
                    connectionTimeout,
                    overwrite,
                    onUserCancel:=UICancelOption.ThrowException).ConfigureAwait(continueOnCapturedContext:=False)
        End Function

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="dialog">A ProgressDialog or Nothing</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        Private Shared Async Function DownloadFileAsync(address As String,
                                        destinationFileName As String,
                                        userName As String,
                                        password As String,
                                        dialog As ProgressDialog,
                                        connectionTimeout As Integer,
                                        overwrite As Boolean,
                                        onUserCancel As UICancelOption) As Task

            ' We're safe from DownloadFile(Nothing, ...) due to overload failure (DownloadFile(String,...) vs. DownloadFile(Uri,...)).
            ' However, it is good practice to verify address before calling Trim.
            If String.IsNullOrWhiteSpace(address) Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim addressUri As Uri = GetUri(address.Trim())

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            Await DownloadFileAsync(addressUri,
                    destinationFileName,
                    networkCredentials,
                    dialog,
                    connectionTimeout,
                    overwrite,
                    onUserCancel).ConfigureAwait(continueOnCapturedContext:=False)
        End Function

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="addressUri">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="dialog">A ProgressDialog or Nothing</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        Private Shared Async Function DownloadFileAsync(addressUri As Uri,
                                        destinationFileName As String,
                                        userName As String,
                                        password As String,
                                        dialog As ProgressDialog,
                                        connectionTimeout As Integer,
                                        overwrite As Boolean) As Task

            Await DownloadFileAsync(addressUri,
                    destinationFileName,
                    userName,
                    password,
                    dialog,
                    connectionTimeout,
                    overwrite,
                    onUserCancel:=UICancelOption.ThrowException).ConfigureAwait(continueOnCapturedContext:=False)
        End Function

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="addressUri">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="dialog">ProgressDialog or Nothing</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        Private Shared Async Function DownloadFileAsync(addressUri As Uri,
                                        destinationFileName As String,
                                        userName As String,
                                        password As String,
                                        dialog As ProgressDialog,
                                        connectionTimeout As Integer,
                                        overwrite As Boolean,
                                        onUserCancel As UICancelOption) As Task

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            Await DownloadFileAsync(addressUri,
                    destinationFileName,
                    networkCredentials,
                    dialog,
                    connectionTimeout,
                    overwrite,
                    onUserCancel).ConfigureAwait(continueOnCapturedContext:=False)
        End Function

#If False Then ' Here in case DownloadFileAsync becomes public
        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="addressUri">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="networkCredentials">The credentials of the user performing the download</param>
        ''' <param name="dialog">A ProgressDialog or Nothing</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <remarks>Calls to all the other overloads will come through here</remarks>
        Private Shared Async Function DownloadFileAsync(addressUri As Uri,
                                        destinationFileName As String,
                                        networkCredentials As ICredentials,
                                        dialog As ProgressDialog,
                                        connectionTimeout As Integer,
                                        overwrite As Boolean) As Task

            Await DownloadFileAsync(addressUri,
                    destinationFileName,
                    networkCredentials,
                    dialog,
                    connectionTimeout,
                    overwrite,
                    onUserCancel:=UICancelOption.ThrowException).ConfigureAwait(continueOnCapturedContext:=False)
        End Function
#End If

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="addressUri">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="clientHandler">An HttpClientHandler of the user performing the download</param>
        ''' <param name="dialog">Progress Dialog</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        ''' <remarks>Calls to all the other overloads will come through here</remarks>
        Private Shared Async Function DownloadFileAsync(addressUri As Uri,
                                        destinationFileName As String,
                                        clientHandler As HttpClientHandler,
                                        dialog As ProgressDialog,
                                        connectionTimeout As Integer,
                                        overwrite As Boolean,
                                        onUserCancel As UICancelOption) As Task
            If connectionTimeout <= 0 Then
                Throw ExUtils.GetArgumentExceptionWithArgName(NameOf(connectionTimeout), SR.Network_BadConnectionTimeout)
            End If

            If addressUri Is Nothing Then
                Throw ExUtils.GetArgumentNullException(NameOf(addressUri))
            End If

            Dim client = If(clientHandler IsNot Nothing,
                            New HttpClient(clientHandler),
                            New HttpClient()
                           )

            ' Set credentials if we have any
            client.Timeout = New TimeSpan(0, 0, 0, 0, connectionTimeout)

            'Construct the local file. This will validate the full name and path
            Dim fullFilename As String = FileSystemUtils.NormalizeFilePath(destinationFileName, NameOf(destinationFileName))
            ' Sometime a path that can't be parsed is normalized to the current directory. This makes sure we really
            ' have a file and path
            If IO.Directory.Exists(fullFilename) Then
                Throw ExUtils.GetInvalidOperationException(SR.Network_DownloadNeedsFilename)
            End If

            'Throw if the file exists and the user doesn't want to overwrite
            If Not overwrite AndAlso IO.File.Exists(fullFilename) Then
                Throw New IO.IOException(Utils.GetResourceString(SR.IO_FileExists_Path, destinationFileName))
            End If

            'Check to see if the target directory exists. If it doesn't, create it
            Dim targetDirectory As String = IO.Path.GetDirectoryName(fullFilename)

            ' Make sure we have a meaningful directory. If we don't, the destinationFileName is suspect
            If String.IsNullOrEmpty(targetDirectory) Then
                Throw ExUtils.GetInvalidOperationException(SR.Network_DownloadNeedsFilename)
            End If

            If Not IO.Directory.Exists(targetDirectory) Then
                IO.Directory.CreateDirectory(targetDirectory)
            End If

            'Create the copier
            Dim copier As New HttpClientCopy(client, dialog)

            'Download the file
            Try
                Await copier.DownloadFileAsync(address:=addressUri,
                                destinationFileName:=fullFilename).ConfigureAwait(continueOnCapturedContext:=False)
            Catch ex As Exception
                If onUserCancel = UICancelOption.ThrowException OrElse Not dialog.UserCanceledTheDialog Then
                    Throw
                End If
            End Try

        End Function

        ''' <summary>
        ''' Gets network credentials from a userName and password
        ''' </summary>
        ''' <param name="userName">The name of the user</param>
        ''' <param name="password">The password of the user</param>
        ''' <returns>A NetworkCredentials</returns>
        Private Shared Function GetNetworkCredentials(userName As String, password As String) As ICredentials

            Return If(String.IsNullOrWhiteSpace(userName) OrElse String.IsNullOrWhiteSpace(password),
                      Nothing,
                      DirectCast(New NetworkCredential(userName, password), ICredentials)
                     )
        End Function

        ''' <summary>
        ''' Centralize setup a ProgressDialog to be used with FileDownload and FileUpload
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <returns></returns>
        Private Shared Function GetProgressDialog(address As String, destinationFileName As String, showUI As Boolean) As ProgressDialog
            If showUI AndAlso Environment.UserInteractive Then
                'Construct the local file. This will validate the full name and path
                Dim fullFilename As String = FileSystemUtils.NormalizeFilePath(Path:=destinationFileName, ParamName:=NameOf(destinationFileName))
                Return New ProgressDialog With {
                            .Text = Utils.GetResourceString(SR.ProgressDialogDownloadingTitle, address),
                            .LabelText = Utils.GetResourceString(SR.ProgressDialogDownloadingLabel, address, fullFilename)
                            }
            End If
            Return Nothing
        End Function

        ''' <summary>
        ''' Gets a Uri from a uri string. We also use this function to validate the UriString (remote file address)
        ''' </summary>
        ''' <param name="address">The remote file address</param>
        ''' <returns>A Uri if successful, otherwise it throws an exception</returns>
        Private Shared Function GetUri(address As String) As Uri
            Try
                Return New Uri(address)
            Catch ex As UriFormatException
                'Throw an exception with an error message more appropriate to our API
                Throw ExUtils.GetArgumentExceptionWithArgName(NameOf(address), SR.Network_InvalidUriString, address)
            End Try
        End Function

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        Public Sub DownloadFile(address As String,
                                destinationFileName As String)
            Try
                DownloadFileAsync(address,
                    destinationFileName,
                    userName:=DEFAULT_USERNAME,
                    password:=DEFAULT_PASSWORD,
                    dialog:=Nothing,
                    connectionTimeout:=DEFAULT_TIMEOUT,
                    overwrite:=False).Wait()
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        Public Sub DownloadFile(address As Uri,
                    destinationFileName As String)
            Try
                DownloadFileAsync(addressUri:=address,
                    destinationFileName,
                    userName:=DEFAULT_USERNAME,
                    password:=DEFAULT_PASSWORD,
                    dialog:=Nothing,
                    connectionTimeout:=DEFAULT_TIMEOUT,
                    overwrite:=False).Wait()
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        Public Sub DownloadFile(address As String,
                    destinationFileName As String,
                    userName As String,
                    password As String)
            Try
                DownloadFileAsync(address,
                    destinationFileName,
                    userName,
                    password,
                    dialog:=Nothing,
                    connectionTimeout:=DEFAULT_TIMEOUT,
                    overwrite:=False).Wait()
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            End Try
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        Public Sub DownloadFile(address As Uri,
                    destinationFileName As String,
                    userName As String,
                    password As String)
            Try
                DownloadFileAsync(addressUri:=address,
                    destinationFileName,
                    userName,
                    password,
                    dialog:=Nothing,
                    connectionTimeout:=DEFAULT_TIMEOUT,
                    overwrite:=False).Wait()
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    Throw ex.InnerException
                End If
                Throw
            End Try

        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        Public Sub DownloadFile(address As String,
                    destinationFileName As String,
                    userName As String,
                    password As String,
                    showUI As Boolean,
                    connectionTimeout As Integer,
                    overwrite As Boolean)
            Dim dialog As ProgressDialog = Nothing
            Try
                dialog = GetProgressDialog(address, destinationFileName, showUI)

                Dim t As Task = DownloadFileAsync(address,
                                    destinationFileName,
                                    userName,
                                    password,
                                    dialog,
                                    connectionTimeout,
                                    overwrite,
                                    onUserCancel:=UICancelOption.ThrowException)
                If t.IsFaulted Then ' This will be true if any parameters are bad
                    Throw t.Exception
                Else
                    dialog?.ShowProgressDialog()
                    t.Wait()
                    If t.IsFaulted Then
                        Throw t.Exception
                    End If
                End If
            Catch ex As Exception
                If ex.InnerException IsNot Nothing Then
                    If TryCast(ex.InnerException, OperationCanceledException) IsNot Nothing AndAlso Environment.UserInteractive Then
                        If showUI AndAlso Environment.UserInteractive Then
                            Try
                                IO.File.Delete(destinationFileName)
                            Catch
                                ' ignore error
                            End Try
                            Throw New OperationCanceledException()
                        End If
                    End If

                    Throw ex.InnerException
                End If
                Throw
            Finally
                CloseProgressDialog(dialog)
            End Try
        End Sub

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Address to the remote file, http, ftp etc...</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        Public Sub DownloadFile(address As String,
                    destinationFileName As String,
                    userName As String,
                    password As String,
                    showUI As Boolean,
                    connectionTimeout As Integer,
                    overwrite As Boolean,
                    onUserCancel As UICancelOption)

            ' We're safe from DownloadFile(Nothing, ...) due to overload failure (DownloadFile(String,...) vs. DownloadFile(Uri,...)).
            ' However, it is good practice to verify address before calling Trim.
            If String.IsNullOrWhiteSpace(address) Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim addressUri As Uri = GetUri(address.Trim())

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            Dim dialog As ProgressDialog = Nothing
            Try
                If showUI AndAlso Environment.UserInteractive Then
                    'Construct the local file. This will validate the full name and path
                    Dim fullFilename As String = FileSystemUtils.NormalizeFilePath(destinationFileName, NameOf(destinationFileName))
                    dialog = GetProgressDialog(address, destinationFileName, showUI)
                End If

                Dim t As Task = DownloadFileAsync(addressUri,
                                    destinationFileName,
                                    networkCredentials,
                                    dialog,
                                    connectionTimeout,
                                    overwrite,
                                    onUserCancel)
                If t.IsFaulted Then ' This will be true if any parameters are bad
                    Throw t.Exception
                Else
                    dialog?.ShowProgressDialog()
                    t.Wait()
                    If t.IsFaulted Then
                        Throw t.Exception
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

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        Public Sub DownloadFile(address As Uri,
                    destinationFileName As String,
                    userName As String,
                    password As String,
                    showUI As Boolean,
                    connectionTimeout As Integer,
                    overwrite As Boolean)
            If address Is Nothing Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim dialog As ProgressDialog = Nothing
            Try
                dialog = GetProgressDialog(address.AbsolutePath, destinationFileName, showUI)
                Dim t As Task = DownloadFileAsync(address,
                                    destinationFileName,
                                    userName,
                                    password,
                                    dialog,
                                    connectionTimeout,
                                    overwrite,
                                    onUserCancel:=UICancelOption.ThrowException)
                If t.IsFaulted Then ' This will be true if any parameters are bad
                    Throw t.Exception
                Else
                    dialog?.ShowProgressDialog()
                    t.Wait()
                    If t.IsFaulted Then
                        Throw t.Exception
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

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="userName">The name of the user performing the download</param>
        ''' <param name="password">The user's password</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        Public Sub DownloadFile(address As Uri,
                    destinationFileName As String,
                    userName As String,
                    password As String,
                    showUI As Boolean,
                    connectionTimeout As Integer,
                    overwrite As Boolean,
                    onUserCancel As UICancelOption)

            If address Is Nothing Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If

            ' Get network credentials
            Dim networkCredentials As ICredentials = GetNetworkCredentials(userName, password)

            Dim dialog As ProgressDialog = Nothing
            Try
                dialog = GetProgressDialog(address.AbsolutePath, destinationFileName, showUI)
                Dim t As Task = DownloadFileAsync(addressUri:=address,
                                    destinationFileName,
                                    networkCredentials,
                                    dialog,
                                    connectionTimeout,
                                    overwrite,
                                    onUserCancel)
                If t.IsFaulted Then ' This will be true if any parameters are bad
                    Throw t.Exception
                Else
                    dialog?.ShowProgressDialog()
                    t.Wait()
                    If t.IsFaulted Then
                        Throw t.Exception
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

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="networkCredentials">The credentials of the user performing the download</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <remarks>Calls to all the other overloads will come through here</remarks>
        Public Sub DownloadFile(address As Uri,
                                destinationFileName As String,
                                networkCredentials As ICredentials,
                                showUI As Boolean,
                                connectionTimeout As Integer,
                                overwrite As Boolean)

            If address Is Nothing Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim dialog As ProgressDialog = Nothing
            Try
                dialog = GetProgressDialog(address.AbsolutePath, destinationFileName, showUI)
                Dim t As Task = DownloadFileAsync(address,
                                    destinationFileName,
                                    networkCredentials,
                                    dialog,
                                    connectionTimeout,
                                    overwrite,
                                    onUserCancel:=UICancelOption.ThrowException)
                If t.IsFaulted Then ' This will be true if any parameters are bad
                    Throw t.Exception
                Else
                    dialog?.ShowProgressDialog()
                    t.Wait()
                    If t.IsFaulted Then
                        Throw t.Exception
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

        ''' <summary>
        ''' Downloads a file from the network to the specified path
        ''' </summary>
        ''' <param name="address">Uri to the remote file</param>
        ''' <param name="destinationFileName">Name and path of file where download is saved</param>
        ''' <param name="networkCredentials">The credentials of the user performing the download</param>
        ''' <param name="showUI">Indicates whether or not to show a progress bar</param>
        ''' <param name="connectionTimeout">Time allotted before giving up on a connection</param>
        ''' <param name="overwrite">Indicates whether or not the file should be overwritten if local file already exists</param>
        ''' <param name="onUserCancel">Indicates what to do if user cancels dialog (either throw or do nothing)</param>
        ''' <remarks>Calls to all the other overloads will come through here</remarks>
        Public Sub DownloadFile(address As Uri,
                    destinationFileName As String,
                    networkCredentials As ICredentials,
                    showUI As Boolean,
                    connectionTimeout As Integer,
                    overwrite As Boolean,
                    onUserCancel As UICancelOption)

            If connectionTimeout <= 0 Then
                Throw ExUtils.GetArgumentExceptionWithArgName(NameOf(connectionTimeout), SR.Network_BadConnectionTimeout)
            End If

            If address Is Nothing Then
                Throw ExUtils.GetArgumentNullException(NameOf(address))
            End If

            Dim dialog As ProgressDialog = Nothing
            Try
                dialog = GetProgressDialog(address.AbsolutePath, destinationFileName, showUI)
                Dim t As Task = DownloadFileAsync(address,
                                    destinationFileName,
                                    networkCredentials,
                                    dialog,
                                    connectionTimeout,
                                    overwrite,
                                    onUserCancel)
                If t.IsFaulted Then ' This will be true if any parameters are bad
                    Throw t.Exception
                Else
                    dialog?.ShowProgressDialog()
                    t.Wait()
                    If t.IsFaulted Then
                        Throw t.Exception
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
